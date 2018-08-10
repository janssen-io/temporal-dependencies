module Temporal.Console.Main

open System
open System.IO
open Temporal.Core.Domain.Computation
open Temporal.Core.Input

open Args

let groupChanges (options:Options) changes =
    match options.vcs with
    | Vcs.Git -> GitTransformer.groupByCommit options.excluded changes
    | Vcs.Tfs -> TfTransformer.groupByChangeset options.excluded changes

let private takeSome (n:int option) xs =
    let l = List.length xs
    match n with
    | Some number -> List.take (Math.Min (number,l)) xs
    | None        -> xs

let orderDependencies (options:Options) =
    Map.toList 
    >> List.sortByDescending snd
    >> List.takeWhile (fun (_, c) -> c >= options.min)
    >> takeSome options.top

let computeWithOptions (options:Options) =
    File.ReadAllLines options.file
    |> Array.toList
    |> groupChanges options
    |> computeTemporalDependencies
    |> orderDependencies options

let printDependencies =
    List.iter (fun ((a,b), c) ->
        printfn "%i" c
        printfn "%s" a
        printfn "%s" b
        printfn ""
    )

let printOptions (options:Args.Options) =
    printfn "File:\t%s (file)" options.file
    printfn "VCS:\t%A" options.vcs
    printfn "Include:\t[%s]" (String.Join ("; ", options.included))
    printfn "Exclude:\t[%s]" (String.Join ("; ", options.excluded))
    printfn "min:\t%i" options.min
    match options.top with
    | Some number -> printfn "top:\t%i" number
    | None        -> printfn "top:\t%s" "all"

let bypass f x =
    f x
    x

[<EntryPoint>]
let main argv =
    Args.parse argv
    |> bypass printOptions
    |> computeWithOptions
    |> printDependencies
    0
