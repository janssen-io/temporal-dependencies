module Temporal.Core.Domain.Helpers

/// Split a list on items that fulfill a predicate.
let split (predicate:string -> bool) =
    List.fold (fun acc x ->
        match x with
            | _ when predicate x -> [] :: acc
            | x' -> (x' :: List.head acc) :: List.tail acc 
    ) [[]]
    >> List.rev

/// Pair all items of a list.
/// 
/// Example: pair [1,2,3] = [(1,2);(1;3);(2;3)]
let rec pair xs =
    match xs with
    | [] -> []
    | x :: xs' -> 
        List.fold (fun acc y -> (x,y) :: acc) [] xs'
        |> (fun r -> List.append r (pair xs'))
