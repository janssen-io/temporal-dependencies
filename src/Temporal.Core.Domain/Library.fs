namespace Temporal.Core.Domain

module Computation =
    let addCount (dep,count) deps =
        if Map.containsKey dep deps then
            Map.add dep (deps.[dep] + count) deps
        else
            Map.add dep count deps

    let addDependency dep = addCount (dep, 1)

    let mergeDependencies source destination =
        let addFolder deps dep count = addCount (dep,count) deps
        Map.fold addFolder source destination

    let computeDependencies changedFiles =
        List.allPairs changedFiles changedFiles
        |> List.filter (fun (a,b) -> a <> b)
        |> List.distinctBy (fun (a,b) -> if a > b then (a,b) else (b,a))
        |> List.map (fun x -> (x, 1))
        |> Map.ofList

    let computeTemporalDependencies: string list list -> Map<(string * string), int> =
        List.map computeDependencies
        >> List.fold mergeDependencies Map.empty
        