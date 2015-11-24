namespace FSharp.IO.FileSystem

/// Provides methods for manipulating paths.
module Path = 

    type FileOrDirectory =
    | File of string
    | Directory of string

    let normalize (path : string) = 
        path.Replace("\\", IOPath.DirectorySeparatorChar.ToString())
            .Replace("/", IOPath.DirectorySeparatorChar.ToString())
            .TrimEnd(IOPath.DirectorySeparatorChar)

    let combine path1 path2 =
        IOPath.Combine(normalize path1, normalize path2)

    let (@@) path1 path2 =
        combine path1 path2

    /// Returns all but the last piece of a path. Different behavior that System.IO.Path.GetDirectoryName
    let directoryName path =
        path
        |> normalize
        |> IOPath.GetDirectoryName

    /// Returns the last piece of a path. Different behavior that System.IO.Path.GetFileName
    let fileName path =
        path
        |> normalize
        |> IOPath.GetFileName
    
    let split path =
        (normalize path).Split(IOPath.DirectorySeparatorChar)

    let isRooted path =
        IOPath.IsPathRooted(normalize path)

    let isRelative path =
        path
        |> normalize
        |> isRooted
        |> not

    let tempFile() =
        IOPath.GetTempFileName()

    let tempPath() =
        IOPath.GetTempPath()

    let fileOrDir path =
        let path = normalize path
        if IOFile.Exists path then
            File path
        else if IODirectory.Exists path then
            Directory path
        else
            failwith "shouldnt be here"
            