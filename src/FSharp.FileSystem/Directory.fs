namespace FSharp.FileSystem

open Path

/// Provides methods for accessing directories.
module Directory = 
    
    let exists path =
        IODirectory.Exists path

    let create path =
        IODirectory.CreateDirectory path |> ignore

    let getFiles path = 
        IODirectory.GetFiles(path)
        |> Seq.ofArray

    let getDirectories path = 
        IODirectory.GetDirectories(path) 
        |> Seq.ofArray
    
    let getFilesWithGlob pattern path =
        let matcher = Globbing.compileMatch pattern
        let rec checkDir path =
            let files = 
                seq {
                    for file in getFiles path do
                        if matcher file then
                            yield file
                }

            let children =
                seq {
                    for dir in getDirectories path do
                        //if matcher dir then
                        yield! checkDir dir
                }

            Seq.concat [files; children]
        checkDir path

    let moveToDir source destination =
        IODirectory.Move(source, destination)

    /// copyToDir "/foo" "/bar" copy the contents of "/foo" into "/bar"
    let rec copyContentsToDir source destination existingHandling =
        if not (exists destination) then
            create destination
        for file in getFiles source do
            let file = Path.fileName file
            File.copyTo (source @@ file) (destination @@ file) existingHandling
        for dir in getDirectories source do
            let dir = Path.fileName dir
            copyContentsToDir (source @@ dir) (destination @@ dir) existingHandling
                       
    /// copyToDir "/foo" "/bar" would create "/bar/foo" and copy the contents of "/foo" into it.
    let copyToDir source destination existingHandling =
        let name = source |> Path.fileName

        copyContentsToDir source (destination @@ name) existingHandling