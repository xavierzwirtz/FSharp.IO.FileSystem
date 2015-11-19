namespace FSharp.FileSystem

/// Provides methods for manipulating paths.
module Path = 

    let combine path1 path2 =
        IOPath.Combine(path1, path2)

    let (@@) path1 path2 =
        combine path1 path2
    let normalize (path : string) = 
        path.Replace("\\", IOPath.DirectorySeparatorChar.ToString())
            .Replace("/", IOPath.DirectorySeparatorChar.ToString())
            .TrimEnd(IOPath.DirectorySeparatorChar).ToLower()

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

    let tempFile() =
        IOPath.GetTempFileName()

    let tempPath() =
        IOPath.GetTempPath()