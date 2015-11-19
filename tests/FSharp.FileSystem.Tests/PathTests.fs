module FSharp.FileSystem.Tests.PathTests

open FSharp.FileSystem
open FSharp.FileSystem.Path
open NUnit.Framework

let pathEqual expected actual =
    Assert.AreEqual(expected |> normalize, actual |> normalize)
[<Test>]
let ``combine`` () =
    combine "foo" "bar"
    |> pathEqual ("foo\\bar")
    "foo" @@ "bar"
    |> pathEqual ("foo\\bar")