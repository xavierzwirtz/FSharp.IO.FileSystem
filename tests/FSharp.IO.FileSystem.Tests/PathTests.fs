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

[<Fact>]
let ``resolve`` () =
    let res root path = 
        Path.resolve root path
    
    let eq expected root path =
        let expected = normalize expected
        let actual = Path.resolve root path
        Assert.Equal(expected, actual)

    eq "/foo" "/bar" "/foo"
    eq "/bar/foo" "/bar" "foo"
    eq "/bar/baz" "/bar/baz/foo" "../"

    eq "C:/bar/baz" "C:/bar/baz/foo" "../"