namespace FSharp.IO.FileSystem

open Path

open Chessie.ErrorHandling

/// Provides methods for accessing directories.
module Directory = 
    
    let exists path =
        IODirectory.Exists path

    let create path =
        tryCatch (fun() -> 
            IODirectory.CreateDirectory path |> ignore )

    let getFiles path = 
        tryCatch (fun() -> 
            IODirectory.GetFiles(path)
            |> Seq.ofArray)

    let getDirectories path = 
        tryCatch (fun() -> 
            IODirectory.GetDirectories(path) 
            |> Seq.ofArray)
    
    let getFilesWithGlob pattern path =
        let matcher = Globbing.compileMatch pattern
        let rec checkDir path =
            trial {
                let! matchingFiles = getFiles path
                let files = 
                    seq {
                        for file in matchingFiles do
                            if matcher file then
                                yield file
                    }

                let! dirs = getDirectories path
                let! children = 
                    dirs 
                    |> Seq.map checkDir
                    |> collect
                let children = children |> Seq.concat
                return Seq.concat [files; children]
            }

            
        checkDir path

    let moveToDir source destination =
        tryCatch(fun () ->
            IODirectory.Move(source, destination))

    /// copyToDir "/foo" "/bar" copy the contents of "/foo" into "/bar"
    let rec copyContentsToDir source destination existingHandling =
        trial {
            if not (exists destination) then
                do! create destination

            let! files = getFiles source
            let! dirs = getDirectories source
            
            for file in files do
                let file = Path.fileName file
                do! File.copyTo (source @@ file) (destination @@ file) existingHandling
            for dir in dirs do
                let dir = Path.fileName dir
                do! copyContentsToDir (source @@ dir) (destination @@ dir) existingHandling
        }
                       
    /// copyToDir "/foo" "/bar" would create "/bar/foo" and copy the contents of "/foo" into it.
    let copyToDir source destination existingHandling =
        let name = source |> Path.fileName

        copyContentsToDir source (destination @@ name) existingHandling