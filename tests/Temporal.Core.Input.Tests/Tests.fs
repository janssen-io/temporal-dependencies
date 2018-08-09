module Tests

open System
open System.Diagnostics
open Xunit
open FsCheck.Xunit

open Temporal.Core.Domain.Computation
open Temporal.Core.Input

let filterEmpty = List.filter (not << String.IsNullOrWhiteSpace)
let trimAll = List.map (fun (s:string) -> s.Trim())
let depCount n = n * (n-1) / 2
let tmap f (a, b) = (f a, f b)
let sumMap : Map<string * string, int> -> int = Map.fold (fun acc _ count -> acc + count) 0

[<Property>]
let ``git - Splits on "new commit"`` xs ys =
    let (xs', ys') = tmap (filterEmpty >> trimAll >> List.distinct) (xs, ys)
    let zs = List.collect id [["new commit"];xs';["new commit"];ys']
    GitTransformer.groupByCommit [] zs
    |> computeTemporalDependencies
    |> sumMap = List.sumBy (depCount << List.length) [xs'; ys']

let prependEdit =
    List.map (fun (s:string) -> sprintf "edit $/%s" s)

let bypass f x =
    f x
    x

[<Property>]
let ``tfs - Splits on "Changeset: "`` xs ys =
    let (xs', ys') = tmap (filterEmpty >> trimAll >> List.distinct >> prependEdit) (xs, ys)
    let zs = List.collect id [["Changeset:"];xs';["Changeset:"];ys']
    TfTransformer.groupByChangeset [] zs
    |> computeTemporalDependencies
    |> sumMap = List.sumBy (depCount << List.length) [xs'; ys']


let forAll = List.fold (&&) true
let hasNoExtension (ext:string) xs = forAll (List.map (fun (s:string) -> not (s.EndsWith ext)) xs)

[<Property>]
let ``tfs - Ignores the given file extensions`` (ext:string) =
    (String.IsNullOrWhiteSpace ext) || (
        let xs = ["Changeset:"; "edit $/foo.cs"; "edit $/bar.cs"; "edit $/foobar" + ext]
        TfTransformer.groupByChangeset [ext] xs
        |> (List.last >> hasNoExtension ext))

[<Property>]
let ``git - Ignores the given file extensions`` (ext:string) =
    (String.IsNullOrWhiteSpace ext) || (
        let xs = ["new commit:"; "foo.cs"; "bar.cs"; "foobar" + ext]
        GitTransformer.groupByCommit [ext] xs
        |> (List.last >> hasNoExtension ext))