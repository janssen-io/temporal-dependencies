module Temporal.Core.Input.GitTransformer

open System
open Temporal.Core.Domain.Helpers

let newCommit (x:string) = x.StartsWith "new commit"

let groupByCommit incl excl =
    split newCommit
    >> List.map (List.filter (Common.isIncludedAndNotExcluded incl excl))
    >> List.filter (not << List.isEmpty)
