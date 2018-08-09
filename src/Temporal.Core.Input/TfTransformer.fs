namespace Temporal.Core.Input

open System
open Temporal.Core.Domain.Helpers

module TfTransformer =
    let toFilename (line:string) =
        line.IndexOf('$')
        |> line.Substring

    let isEdited (line:string) =
        line.IndexOf('$') > line.IndexOf("edit")

    let getEditedFilenames =
        List.fold (fun acc line ->
            if isEdited line then toFilename line :: acc else acc
        ) []

    let ignoredString ignoredExtensions x =
        (String.IsNullOrWhiteSpace x || hasExtensions ignoredExtensions x)

    let groupByChangeset ignoredExtensions =
        List.map (fun (s:string) -> s.Trim())
        >> List.filter (not << ignoredString ignoredExtensions)
        >> split (fun line -> line.StartsWith "Changeset:")
        >> List.map getEditedFilenames

    // Assumes format similar to: tf history /recurive /format:detailed