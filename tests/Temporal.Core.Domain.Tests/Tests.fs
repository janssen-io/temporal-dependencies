module Tests

open Xunit
open FsCheck
open FsCheck.Xunit
open Temporal.Core.Domain

type Dep = Map<string, int>
let sumMap = Map.fold (fun acc _ value -> acc + value) 0

[<Property>]
let ``Adding a dependency increments the total count`` dep (deps:Dep) =
    addDependency dep deps
    |> sumMap = (sumMap deps + 1)

[<Property>]
let ``The sum of a merged mapping is equal to the individual sums`` (groupA:Dep) (groupB:Dep) =
    let groupAB = mergeDependencies groupA groupB
    sumMap groupAB = (sumMap groupA + sumMap groupB)