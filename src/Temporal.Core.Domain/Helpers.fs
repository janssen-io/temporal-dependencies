namespace Temporal.Core.Domain

module Helpers = 
    type DependencyList = ((string * string) * int) list
    let split (predicate:string -> bool) =
        List.fold (fun acc x ->
            match x with
                | _ when predicate x -> [] :: acc
                | x' -> (x' :: List.head acc) :: List.tail acc 
        ) [[]]
        >> List.rev

    let forAll predicate =
        List.fold (fun acc x -> acc && predicate x) true
    
    let forAny =
        List.fold (||) false

    let hasExtension extension (filename: string)
        = filename.EndsWith extension

    let hasExtensions xs (s:string) =
        match xs with
        | [] -> false
        | _  -> forAny (List.map (s.EndsWith) xs)

    let takeSome (n:int): DependencyList -> DependencyList =
        (List.sortByDescending (fun (_,count) -> count)
        >> List.takeWhile (fun (_, count) -> count > n))

    let mapToList : Map<(string * string), int> -> DependencyList =
        Map.fold (fun acc key value -> (key, value) :: acc) []