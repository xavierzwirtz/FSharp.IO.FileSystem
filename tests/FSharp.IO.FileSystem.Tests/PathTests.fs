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

[<Fact>]
let ``isRooted && isRelative`` () =
    let root path = 
        Assert.True(path |> Path.isRooted)
        Assert.False(path |> Path.isRelative)
    let rel path = 
        Assert.False(path |> Path.isRooted)
        Assert.True(path |> Path.isRelative)
    
    root "\\foo"
    root "\\foo\\bar"
    root "\\foo\\bar..\\baz"

    rel "./relative"
    rel "../relative"
    rel "..\\relative"

