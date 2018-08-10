module Tests

open System
open FsCheck.Xunit
open Temporal.Core.Domain.Computation

type Dep = Map<string, int>
let sumMap = Map.fold (fun acc _ value -> acc + value) 0

let filterEmpty = List.filter (not << String.IsNullOrWhiteSpace)
let depCount n = n * (n-1) / 2
let maxMap: Map<string * string, int> -> int =
    Map.fold (fun acc _ count -> if count > acc then count else acc) 0

[<Property>]
let ``Adding a dependency increments the total count`` dep (deps:Dep) =
    addDependency dep deps
    |> sumMap = (sumMap deps + 1)

[<Property>]
let ``The sum of a merged mapping is equal to the individual sums`` (groupA:Dep) (groupB:Dep) =
    let groupAB = mergeDependencies groupA groupB
    sumMap groupAB = (sumMap groupA + sumMap groupB)

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