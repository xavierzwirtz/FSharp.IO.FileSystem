namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSharp.FileSystem")>]
[<assembly: AssemblyProductAttribute("FSharp.FileSystem")>]
[<assembly: AssemblyDescriptionAttribute("FSharp'y methods of reading and writing to the file system.")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
