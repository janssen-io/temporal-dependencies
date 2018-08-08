namespace Temporal.Core.Input

open System.IO
open Temporal.Core.Domain.Helpers
open Temporal.Core.Domain.Computation

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

    let groupByChangeset =
        split (fun line -> line.StartsWith "Changeset:")
        >> List.map getEditedFilenames

    // Assumes format similar to: tf history /recurive /format:detailed
    let getDependencies = 
        File.ReadAllLines
        >> List.ofArray 
        >> groupByChangeset
        >> computeTemporalDependencies