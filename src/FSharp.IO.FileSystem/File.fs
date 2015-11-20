namespace FSharp.IO.FileSystem

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

    let readAllTextWithEncoding path encoding =
        IOFile.ReadAllText(path, encoding)

    let writeAllText path contents =
        IOFile.WriteAllText(path, contents)

    let writeAllTextWithEncoding path contents encoding =
        IOFile.WriteAllText(path, contents, encoding)

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