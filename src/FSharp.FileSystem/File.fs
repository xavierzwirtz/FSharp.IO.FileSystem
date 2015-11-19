namespace FSharp.FileSystem

/// Provides methods for accessing files.
module File = 

    type ExistingHandling =
    | Overwrite
    | Fail
    | Skip


    let exists path =
        IOFile.Exists path

    let readAllText path =
        IOFile.ReadAllText path
    let writeAllText path contents=
        IOFile.WriteAllText(path, contents)

    let copyTo source destination existingHandling =
        match existingHandling with
        | Overwrite ->
            IOFile.Copy(source, destination, true)
        | Fail ->
            IOFile.Copy(source, destination, false)
        | Skip ->
            if not (exists destination) then
                IOFile.Copy(source, destination)

    let moveTo source destination =
        IOFile.Move(source, destination)