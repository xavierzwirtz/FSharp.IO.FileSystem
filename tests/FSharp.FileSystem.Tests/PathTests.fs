module FSharp.FileSystem.Tests.PathTests

open FSharp.FileSystem
open FSharp.FileSystem.Path
open Xunit

let pathEqual expected actual =
    Assert.Equal(expected |> normalize, actual |> normalize)
[<Fact>]
let ``combine`` () =
    combine "foo" "bar"
    |> pathEqual ("foo\\bar")
    "foo" @@ "bar"
    |> pathEqual ("foo\\bar")