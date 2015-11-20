namespace FSharp.IO.FileSystem

open System.Text.RegularExpressions

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