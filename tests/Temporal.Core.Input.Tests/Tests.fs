module Tests

open System
open FsCheck.Xunit

open Temporal.Core.Input

let filterEmpty = List.filter (not << String.IsNullOrWhiteSpace)
let trimAll = List.map (fun (s:string) -> s.Trim())
let depCount n = n * (n-1) / 2
let sumMap : Map<string * string, int> -> int = Map.fold (fun acc _ count -> acc + count) 0

let filterValidFiles = filterEmpty >> trimAll >> List.distinct

[<Property>]
let ``git - Splits on "new commit"`` groupA groupB =
    // prepare filenames to meet precondition
    let xs = filterValidFiles groupA
    let ys = filterValidFiles groupB

    // intersperse "new commit"
    let zs = List.collect id [["new commit"];xs;["new commit"];ys]

    let result = GitTransformer.groupByCommit [] [] zs
    let expected =  List.map List.rev <| List.filter (not << List.isEmpty) [xs;ys]
    result = expected

[<Property>]
let ``tfs - Splits on "Changeset: "`` groupA groupB =
    // prepare filenames to meet precondition
    let filesA = List.map (sprintf "$/%s") <| filterValidFiles groupA
    let filesB = List.map (sprintf "$/%s") <| filterValidFiles groupB

    // mark as 'edited'
    let xs = List.map (sprintf "edit %s") filesA
    let ys = List.map (sprintf "edit %s") filesB

    // intersperse "changeset:"
    let zs = List.collect id [["Changeset:"];xs;["Changeset:"];ys]
    let result = TfTransformer.groupByChangeset [] [] zs
    let expected = List.filter (not << List.isEmpty) [filesA;filesB]
    result = expected

[<Property>]
let ``isIncludedAndNotExcluded filters out all excluded extensions`` ext =
    String.IsNullOrEmpty ext || (
        let files = ["foo.cs"; "bar.cs"; "foobar." + ext]
        let result = List.filter (Common.isIncludedAndNotExcluded [] [ext]) files
        let expected = ["foo.cs"; "bar.cs"]
        result = expected
    )

[<Property>]
let ``isIncludedAndNotExcluded filters out all non-included extensions`` ext =
    String.IsNullOrEmpty ext || (
        let files = ["foo.cs"; "bar.cs"; "foobar." + ext]
        let result = List.filter (Common.isIncludedAndNotExcluded [ext] []) files
        let expected = ["foobar." + ext]
        result = expected
    )
    
[<Property>]
let ``Excluding takes precedence over including`` ext =
    String.IsNullOrEmpty ext || (
        let files = ["baz.fs"; "foo.cs"; "bar.cs"; "foobar." + ext]
        let result = List.filter (Common.isIncludedAndNotExcluded [".cs"; "." + ext] ["foobar." + ext]) files
        let expected = ["foo.cs"; "bar.cs"]
        result = expected
    )

