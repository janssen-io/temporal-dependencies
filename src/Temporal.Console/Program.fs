module Temporal.Console.Main

open System
open System.IO
open Temporal.Core.Domain.Computation
open Temporal.Core.Input

let getChanges (options:Args.Options) =
    match options.method with
    | Args.Method.LogFile file -> Result.Ok <| (List.ofArray <| File.ReadAllLines file)
    | Args.Method.Process -> Result.Error <| "Process is not implemented yet."

let groupChanges (options:Args.Options) changes =
    match options.vcs with
    | Args.Vcs.Git -> GitTransformer.groupByCommit options.ignore changes
    | Args.Vcs.Tfs -> TfTransformer.groupByChangeset options.ignore changes

let private takeSome (n:int option) xs =
    let l = List.length xs
    match n with
    | Some number -> List.take (Math.Min (number,l)) xs
    | None        -> xs

let orderDependencies (options:Args.Options) =
    Map.toList 
    >> List.sortByDescending snd
    >> List.takeWhile (fun (_, c) -> c >= options.min)
    >> takeSome options.top

let computeWithOptions (options:Args.Options) =
    getChanges options 
    |> Result.map (
      (groupChanges options)
      >> computeTemporalDependencies
      >> (orderDependencies options))

let printDeps =
    List.iter (fun ((a,b), c) ->
        printfn "%i" c
        printfn "%s" a
        printfn "%s" b
        printfn ""
    )

let printOptions (options:Args.Options) =
    match options.method with
    | Args.Method.LogFile f -> Console.WriteLine("Method:\t{0} (file)", f)
    | Args.Method.Process -> Console.WriteLine("Method:\tProcess")
    Console.WriteLine("vcs:\t{0}", options.vcs)
    Console.WriteLine("ignore:\t[{0}]", String.Join("; ", options.ignore))
    Console.WriteLine("min:\t{0}", options.min)
    match options.top with
    | Some number -> Console.WriteLine("top:\t{0}", number)
    | None        -> Console.WriteLine("top:\tall")
    
    options

[<EntryPoint>]
let main argv =
    let dependencies = 
        Args.parse <| List.ofArray argv
        |> Result.map printOptions
        |> Result.bind computeWithOptions
    match dependencies with
        | Ok dependencies -> 
            do printDeps dependencies
            0
        | Error message   -> 
            printf "%s" message
            1
