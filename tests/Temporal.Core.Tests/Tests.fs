module Tests

open System
open FsCheck.Xunit
open Temporal.Core.Domain.Computation

let filterEmpty = List.filter (not << String.IsNullOrWhiteSpace)
let depCount n = n * (n-1) / 2
let maxMap = Map.fold (fun acc _ count -> if count > acc then count else acc) 0

[<Property>]
let ``Makes pairs of all different file names`` xs =
    let xs' = (List.distinct >> filterEmpty) xs
    let n = List.length xs'
    computeTemporalDependencies [xs']
    |> Map.count = depCount n

[<Property>]
let ``Maximum count is at most equal to number of commits`` commits =
    computeTemporalDependencies commits
    |> maxMap <= List.length commits 