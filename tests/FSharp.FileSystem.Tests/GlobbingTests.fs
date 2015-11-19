module FSharp.FileSystem.Tests.GlobbingTests

open FSharp.FileSystem
open NUnit.Framework

let shouldMatch pattern paths =
    for path in paths do
        if not(Globbing.isMatch pattern path) then
            failwithf "%s should match %s" pattern path


let shouldNotMatch pattern paths =
    for path in paths do
        if Globbing.isMatch pattern path then
            failwithf "%s should not match %s" pattern path

[<Test>]
let ``should exact match``() =
    shouldMatch "foo.bar" ["foo.bar"]

[<Test>]
let ``should not match``() =
    shouldNotMatch "foo.bar" ["baz.bar"; "baz/foo.bar"]

[<Test>]
let ``should match any in root``() =
    shouldMatch "*.*" ["foo.bar"; "foo.baz"]

[<Test>]
let ``should not match in sub dir``() =
    shouldNotMatch "*.*" ["foo/bar/baz.bill"; "foo/bar.baz"]

[<Test>]
let ``should match filename``() =
    shouldMatch "*.bar" ["foo.bar"; "baz.bar"]

[<Test>]
let ``should not match filename``() =
    shouldNotMatch "*.bar" ["foo.baz"; "baz.foo"]
      
[<Test>]
let ``should not match different subdir``() =
    shouldNotMatch "foo/*.bar" ["baz/foo.bar"; "foo/bar/baz.bar"]
      
[<Test>]
let ``should match subdir``() =
    shouldMatch "foo/*.bar" ["foo/baz.bar"; "foo/foo.bar"]
      
[<Test>]
let ``should match any``() =
    shouldMatch "**/*.*" [ "foo"
                           "foo.bar"
                           "foo/baz"
                           "foo/baz.bill"
                           "foo/baz/x.bill" ]
      
[<Test>]
let ``should match any with extension``() =
    shouldMatch "**/*.bar" [ "foo.bar"
                             "foo/baz.bar"
                             "foo/bar/bill.bar" ]
      
[<Test>]
let ``should not match any with wrong extension``() =
    shouldNotMatch "**/*.bar" [ "foo.baz"; 
                                "foo/baz.baz"; 
                                "foo/bar/bill.baz" ]
      
[<Test>]
let ``should match any subdir with extension``() =
    shouldMatch "foo/**/*.bar" [ "foo/baz.bar" 
                                 "foo/baz/bill.bar" ]

[<Test>]
let ``should not match any in wrong subdir with extension ``() =
    shouldNotMatch "foo/**/*.bar" [ "bill/baz.bar"
                                    "baz/foo/bill.bar" ]

[<Test>]
let ``should match particular subdir``() =
    shouldMatch "**/subdir/more/*.bar" [ "subdir/more/foo.bar"
                                         "root/subdir/more/foo.bar"
                                         "root/another/subdir/more/foo.bar" ]

[<Test>]
let ``should not match particular subdir``() =
    shouldNotMatch "**/subdir/more/*.bar" [ "wrong/foo.bar"
                                            "foo.bar"
                                            "subdir/more/wrong/foo.bar" ]

[<Test>]
let ``should match particular subdir then recursive``() =
    shouldMatch "**/subdir/more/**/*.bar" [ "root/subdir/more/foo.bar"
                                            "root/subdir/more/another/foo.bar"
                                            "root/another/subdir/more/foo.bar" ]

[<Test>]
let ``should match any file type``() =
    shouldMatch 
        "/some/random/path/**/Properties/*" 
        ["/some/random/path/somewhere/Properties/AssemblyInfo.cs"]