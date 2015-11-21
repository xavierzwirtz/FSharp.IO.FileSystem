[<AutoOpen>]
module FSharp.IO.FileSystem.InternalTypes

open Chessie.ErrorHandling

type IOFile = System.IO.File
type IODirectory = System.IO.Directory
type IOPath = System.IO.Path


let tryCatch func =
    try
        ok (func())
    with
    | ex -> fail ex