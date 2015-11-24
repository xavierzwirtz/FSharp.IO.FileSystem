module FSharp.IO.FileSystem.Tests.GlobbingTests

open FSharp.IO.FileSystem.Tests.DirectoryTests
open FSharp.IO.FileSystem
open FSharp.IO.FileSystem.Path
open Xunit

open Chessie.ErrorHandling

[<Fact>]
let ``search`` () =
    let spec = [ "foo/bar1/baz" 
                 "foo/bar2/baz"
                 "foo/bar2/bill.bar"
                 "bog/bid" ]
    let source = createDir spec

    let files = Globbing.search source "**/*.bar" |> Seq.toList

    Assert.Equal<list<string>>([source @@ "foo/bar2/bill.bar"], files)

[<Fact>]
let ``search rooted pattern`` () =
    let spec = [ "foo/bar1/baz" 
                 "foo/bar2/baz"
                 "foo/bar2/bill.bar"
                 "bog/bid" ]
    let source = createDir spec
    printfn "%s" source
    let files = Globbing.search source (source @@ "**/*.bar") |> Seq.toList

    Assert.Equal<list<string>>([source @@ "foo/bar2/bill.bar"], files)


[<Fact>]
let ``copyTo`` () =
    let spec = [ "foo/bar1/baz" 
                 "foo/bar2/baz"
                 "foo/bar2/bill.bar"
                 "bog/bid" ]
    let source = createDir spec
    let dest = tempFolder()
    Directory.create dest |> returnOrFail

    Globbing.copyTo source dest "**/*.bar" File.Overwrite |> returnOrFail
   
    let results = Globbing.search dest "**/*.*"

    let expected = [dest @@ "foo/bar2/bill.bar"]

    Assert.Equal<list<string>>(expected, results)