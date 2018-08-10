module Temporal.Core.Domain.Computation

let addCount (dep,count) deps =
    if Map.containsKey dep deps then
        Map.add dep (deps.[dep] + count) deps
    else
        Map.add dep count deps

let addDependency dep = addCount (dep, 1)

let mergeDependencies source destination =
    let addFolder deps dep count = addCount (dep,count) deps
    Map.fold addFolder source destination

let computeDependencies filenames =
    Helpers.pair filenames
    |> List.map (fun x -> (x, 1))
    |> Map.ofList

let computeTemporalDependencies filePairs =
    List.map computeDependencies filePairs
    |> List.fold mergeDependencies Map.empty
    