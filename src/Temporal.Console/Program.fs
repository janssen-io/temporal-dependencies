namespace Temporal.Console

module Main =
    open System
    open System.IO
    open Temporal.Core.Domain.Computation
    open Temporal.Core.Input
    open Temporal.Core.Input

    let getChanges (options:Args.Options) =
        match options.method with
        | Args.Method.LogFile file -> Result.Ok <| (List.ofArray <| File.ReadAllLines file)
        | Args.Method.Process -> Result.Error <| "Process is not implemented yet."

    let transformChanges (options:Args.Options) changes =
        match options.vcs with
        | Args.Vcs.Git -> GitTransformer.groupByCommit changes
        | Args.Vcs.Tfs -> TfTransformer.groupByChangeset changes

    let orderDependencies =
        Map.toList >> List.sortByDescending (fun (_,count) -> count)

    let computeWithOptions (options:Args.Options) =
        getChanges options
        |> Result.map (transformChanges options)
        |> Result.map computeTemporalDependencies

    let printDeps =
        List.iter (fun ((a,b), c) ->
            printfn "%i" c
            printfn "%s" a
            printfn "%s" b
            printfn ""
        )

    [<EntryPoint>]
    let main argv =
        let dependencies = 
            Args.parse <| List.ofArray argv
            |> Result.bind computeWithOptions
            |> Result.map orderDependencies
        match dependencies with
            | Ok dependencies -> 
                printDeps dependencies
                0
            | Error message   -> 
                printf "%s" message
                1
