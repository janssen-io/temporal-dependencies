#load ".fake/build.fsx/intellisense.fsx"
open Fake.DotNet
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

let buildConfig
  = let config = Environment.environVarOrDefault "config" "Debug"
    match config.ToLower() with
    | "debug"   -> DotNet.BuildConfiguration.Debug
    | "release" -> DotNet.BuildConfiguration.Release
    | _         -> DotNet.BuildConfiguration.Custom config
let runtime
  = let config = Environment.environVarOrDefault "runtime" "windows"
    match config.ToLower() with
    | "windows" -> "win10-x64"
    | "linux"   -> "linux-x64"
    | other     -> other

let projects = "src/**/"
let tests = "tests/**/"

let build dir = 
    !! (dir + "*.*proj")
    |> Seq.iter (DotNet.build (fun p ->
        { p with Configuration = buildConfig }
    ))

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
            Configuration = buildConfig
        }
    ))
)

Target.create "JustTest" (fun _ ->
    !! "tests/**/*.*proj"
    |> Seq.iter (DotNet.test (fun ps ->
        { ps with 
            NoBuild = true
            Configuration = buildConfig
        }
    ))
)

Target.create "Publish" (fun _ ->
  DotNet.publish (fun p ->
    { p with 
        NoBuild = false;
        Configuration = buildConfig;
        Runtime = Some runtime
    }) "src/Temporal.Console/Temporal.Console.fsproj"
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "BuildTests"
  ==> "Test"
  ==> "Publish"
  ==> "All"

Target.runOrDefault "All"
