module Temporal.Core.Input.Common

open System

let isExcluded xs (s:string) = List.exists (s.EndsWith) xs
let isIncluded xs (s:string) = List.isEmpty xs || List.exists (s.EndsWith) xs

let isIncludedAndNotExcluded incl excl x =
    (not (String.IsNullOrEmpty x))
    && (not (isExcluded excl x))
    && (isIncluded incl x)
