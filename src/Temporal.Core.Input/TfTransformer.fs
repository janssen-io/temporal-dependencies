module Temporal.Core.Input.TfTransformer

open System
open Temporal.Core.Domain.Helpers

let isEdited (line:string) =
    line.IndexOf('$') > line.IndexOf("edit")

let toFilename (line:string) =
    line.IndexOf('$')
    |> line.Substring

let getEditedFilenames =
    List.fold (fun acc line ->
        if isEdited line
            then toFilename line :: acc 
            else acc
    ) []

/// Assumes the detailed TFS history format
/// 
/// (`tf history /format:detailed`)
let groupByChangeset incl excl =
    split (fun line -> line.StartsWith "Changeset:")
    >> List.map (List.filter (Common.isIncludedAndNotExcluded incl excl))
    >> List.map getEditedFilenames
    >> List.filter (not << List.isEmpty)
