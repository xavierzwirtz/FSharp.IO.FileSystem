namespace FSharp.IO.FileSystem

open System.Text.RegularExpressions
open Chessie.ErrorHandling

open Path

/// Provides glob matching

module Globbing =

    let private patternMatchCache = 
        System.Collections.Concurrent.ConcurrentDictionary<string, string -> bool>()

    let private compileGlobToRegex pattern =
        let pattern = Path.normalize pattern

        let escapedPattern = (Regex.Escape pattern)
        let regexPattern = 
            let xTOy = 
                [
                    "dirwildcard", (@"\\\*\\\*(/|\\\\)", @"(.*(/|\\))?")
                    "stardotstar", (@"\\\*\\.\\\*", @"([^\\/]*)")
                    "wildcard", (@"\\\*", @"([^\\/]*)")
                ] |> List.map(fun (key, reg) ->
                    let pattern, replace = reg
                    let pattern = sprintf "(?<%s>%s)" key pattern
                    key, (pattern, replace)
                )
            let xTOyMap = xTOy |> Map.ofList
            let replacePattern = xTOy |> List.map(fun x -> x |> snd |> fst) |> String.concat("|")
            let replaced = Regex(replacePattern).Replace(escapedPattern, fun m -> 
                let matched = xTOy |> Seq.map(fst) |> Seq.find(fun n -> 
                    m.Groups.Item(n).Success
                )
                (xTOyMap |> Map.tryFind matched).Value |> snd
            )
            "^" + replaced + "$"

        Regex(regexPattern)

    let compileMatch pattern =
        let outMatcher : ref<_> = ref (fun _ -> false)
        if patternMatchCache.TryGetValue(pattern, outMatcher) then
            !outMatcher
        else
            let regex = compileGlobToRegex pattern
            let matcher path =
                let path = Path.normalize path
                regex.IsMatch(path)
                
            patternMatchCache.TryAdd(pattern, matcher) |> ignore
            matcher

    let isMatch pattern path : bool = 
        let matcher = compileMatch pattern
        matcher path

    
    type private SearchOption = 
        | Directory of string
        | Drive of string
        | Recursive
        | FilePattern of string

    let private checkSubDirs absolute (dir : string) root = 
        if dir.Contains "*" then 
            System.IO.Directory.EnumerateDirectories(root, dir, System.IO.SearchOption.TopDirectoryOnly) |> Seq.toList
        else 
            let path = Path.combine root dir
        
            let dir =
                if absolute then dir
                else path
            if Directory.exists dir then
                [Directory.fullName dir |> returnOrFail]
            else 
                []

    let rec private buildPaths acc (input : SearchOption list) = 
        match input with
        | [] -> acc
        | Directory(name) :: t -> 
            let subDirs = 
                acc
                |> List.map (checkSubDirs false name)
                |> List.concat
            buildPaths subDirs t
        | Drive(name) :: t -> 
            let subDirs = 
                acc
                |> List.map (checkSubDirs true name)
                |> List.concat
            buildPaths subDirs t
        | Recursive :: [] -> 
            let dirs = 
                Seq.collect (fun dir -> System.IO.Directory.EnumerateFileSystemEntries(dir, "*", System.IO.SearchOption.AllDirectories)) acc 
                |> Seq.toList
            buildPaths (acc @ dirs) []
        | Recursive :: t -> 
            let dirs = 
                Seq.collect (fun dir -> System.IO.Directory.EnumerateDirectories(dir, "*", System.IO.SearchOption.AllDirectories)) acc 
                |> Seq.toList
            buildPaths (acc @ dirs) t
        | FilePattern(pattern) :: t -> 
             Seq.collect (fun dir -> 
                                if Directory.exists(Path.combine dir pattern)
                                then seq { yield Path.combine dir pattern }
                                else 
                                    try
                                        System.IO.Directory.EnumerateFiles(dir, pattern)
                                    with
                                        | :? System.IO.PathTooLongException as ex ->
                                            Array.toSeq [| |]
                                ) acc |> Seq.toList

    let private driveRegex = Regex(@"^[A-Za-z]:$", RegexOptions.Compiled)
    let search (baseDir : string) (pattern : string) = 
        let baseDir = normalize baseDir
        let input = normalize pattern
        let input = 
            if input.StartsWith baseDir then
                input.Remove(0, baseDir.Length)
            else
                input
        let filePattern = Path.fileName(input)
        input.Split([| '/'; '\\' |], System.StringSplitOptions.RemoveEmptyEntries)
        |> Seq.map (function 
               | "**" -> Recursive
               | a when a = filePattern -> FilePattern(a)
               | a when driveRegex.IsMatch a -> Directory(a + "\\")
               | a -> Directory(a))
        |> Seq.toList
        |> buildPaths [ baseDir ]
        |> List.map normalize

    
    let copyTo sourceDir destinationDir pattern existingHandling =
        let sourceDir = Path.normalize sourceDir
        let destinationDir = Path.normalize destinationDir
        let files = search sourceDir pattern

        // cant use trial expression here. Causes issues with stack overflows
        let res =
            seq {
                for source in files do
                    let rel = source.Substring(sourceDir.Length + 1)
                    let dest = Path.combine destinationDir rel
                    let destDir = Path.directoryName dest

                    let res = Directory.create destDir
                    yield
                        res >>= (fun (_) ->
                            File.copyTo source dest existingHandling)
            }
        res |> collect >>= (fun (_) -> ignore() |> ok)
