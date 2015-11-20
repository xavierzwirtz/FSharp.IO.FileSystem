module FSharp.IO.FileSystem.Tests.PathTests

open FSharp.IO.FileSystem
open FSharp.IO.FileSystem.Path
open Xunit

let pathEqual expected actual =
    Assert.Equal(expected |> normalize, actual |> normalize)
[<Fact>]
let ``combine`` () =
    combine "foo" "bar"
    |> pathEqual ("foo\\bar")
    "foo" @@ "bar"
    |> pathEqual ("foo\\bar")