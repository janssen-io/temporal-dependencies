#load ".fake/build.fsx/intellisense.fsx"
open Fake.DotNet
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

let projects = "src/**/"
let tests = "tests/**/"

let build dir = 
    !! (dir + "*.*proj")
    |> Seq.iter (DotNet.build id)

let clean dir = 
    !! (dir + "bin")
    ++ (dir + "obj")
    |> Shell.cleanDirs 

Target.create "Clean" (fun _ ->
    clean projects
    clean tests
)

Target.create "Build" (fun _ ->
    build projects
)

Target.create "BuildTests" (fun _ ->
    build tests
)

Target.create "Test" (fun _ ->
    !! "tests/**/*.*proj"
    |> Seq.iter (DotNet.test (fun ps ->
        { ps with 
            NoBuild = true
            Configuration = DotNet.BuildConfiguration.Release
        }
    ))
)

Target.create "JustTest" (fun _ ->
    !! "tests/**/*.*proj"
    |> Seq.iter (DotNet.test (fun ps ->
        { ps with 
            NoBuild = true
            Configuration = DotNet.BuildConfiguration.Release
        }
    ))
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "BuildTests"
  ==> "Test"
  ==> "All"

Target.runOrDefault "All"
