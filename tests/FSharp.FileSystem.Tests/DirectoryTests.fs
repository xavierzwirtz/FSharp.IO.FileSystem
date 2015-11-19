module FSharp.FileSystem.Tests.DirectoryTests

open FSharp.FileSystem
open FSharp.FileSystem.Path
open Xunit

let tempFolder() = 
    tempPath() @@ System.Guid.NewGuid().ToString()

let createDir (spec : string seq) =
    let root = tempFolder()
    for file in spec do
        Directory.create (root @@ (file |> Path.directoryName))
        File.writeAllText (root @@ file) file
    root

let checkDir (spec : string seq) path =
    printfn "path %s" path
    for file in spec do
        printfn "file %s" file
        let contents = (path @@ file) |> File.readAllText
        if contents <> file then
            failwith "contents dont match"

[<Fact>]
let ``copyToDir`` () =
    let spec = [ "foo/bar1/baz" 
                 "foo/bar2/baz"
                 "foo/bar2/bill"
                 "bog/bid" ]
    let source = createDir spec
    let dest = tempFolder()
    let dirName = source |> Path.fileName
    Directory.copyToDir source dest File.Fail
    checkDir spec (dest @@ dirName)


[<Fact>]
let ``copyContentsToDir`` () =
    let spec = [ "foo/bar1/baz" 
                 "foo/bar2/baz"
                 "foo/bar2/bill"
                 "bog/bid" ]
    let source = createDir spec
    let dest = tempFolder()
    Directory.copyContentsToDir source dest File.Fail
    checkDir spec (dest)


[<Fact>]
let ``glob`` () =
    let spec = [ "foo/bar1/baz" 
                 "foo/bar2/baz"
                 "foo/bar2/bill.bar"
                 "bog/bid" ]
    let source = createDir spec

    let files = source |> Directory.getFilesWithGlob "**/*.bar" |> Seq.toList
    printfn "%A" files
    Assert.Equal<list<string>>([source @@ "foo/bar2/bill.bar"], files)
