namespace Temporal.Core

open Temporal.Core.Domain

module Computation =
    let private computeDependencies changedFiles =
        List.allPairs changedFiles changedFiles
        |> List.filter (fun (a,b) -> a <> b)
        |> List.distinctBy (fun (a,b) -> if a > b then (a,b) else (b,a))
        |> List.map (fun x -> (x, 1))
        |> Map.ofList

    let computeTemporalDependencies: string list list -> Map<(string * string), int> =
        List.map computeDependencies
        >> List.fold mergeDependencies Map.empty