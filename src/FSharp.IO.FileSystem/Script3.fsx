#I @"C:\Projects\FSharp.IO.FileSystem\src\FSharp.IO.FileSystem"
#I @"C:\Projects\FSharp.IO.FileSystem\src\FSharp.IO.FileSystem\bin\debug"
#r "Chessie"

#load "InternalTypes.fs"
#load "Path.fs"
#load "File.fs"
#load "Directory.fs"
#load "Globbing.fs"

open FSharp.IO.FileSystem

let sourceDir = @"C:\Users\xavier\AppData\Local\NCrunch\12692\6\tests\scenarios\deploy\temp\packages/Main"
let destDir = @"C:\Users\xavier\AppData\Local\NCrunch\12692\6\tests\scenarios\deploy\temp\dest\"
let pattern = @"C:\Users\xavier\AppData\Local\NCrunch\12692\6\tests\scenarios\deploy\temp\packages\Main\misc\**\*.txt"

//Globbing.search sourceDir pattern

Globbing.copyTo sourceDir destDir pattern File.Overwrite