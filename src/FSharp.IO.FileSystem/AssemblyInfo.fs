namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSharp.IO.FileSystem")>]
[<assembly: AssemblyProductAttribute("FSharp.IO.FileSystem")>]
[<assembly: AssemblyDescriptionAttribute("FSharp'y methods of reading and writing to the file system.")>]
[<assembly: AssemblyVersionAttribute("2.0.0")>]
[<assembly: AssemblyFileVersionAttribute("2.0.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "2.0.0"
