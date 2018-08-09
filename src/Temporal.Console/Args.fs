namespace Temporal.Console

open System
module Args =
    type Vcs =
        | Git
        | Tfs
    
    type Method =
        | LogFile of string
        | Process

    type Options = {
        vcs: Vcs;
        method: Method;
        ignore: string list;
        min: int
    }

    let private tryParse (s:string) =
        match Int32.TryParse s with
        | true, v  -> Ok v
        | false, _ -> Error (sprintf "Invalid integer '%s'" s)

    let rec private parseRec args options =
        match args with
        | [] -> Ok options
        | "--min" :: xs ->
            match xs with
            | num :: xs' ->
                Result.bind (fun n -> parseRec xs' {options with min = n}) (tryParse num)
            | []         -> Error "--min requires an integer argument."
        | "--vcs" :: xs ->
            match xs with
            | "git" :: xs' -> parseRec xs' {options with vcs = Vcs.Git }
            | "tfs" :: xs' -> parseRec xs' {options with vcs = Vcs.Tfs }
            | _            -> Error "Invalid argument for --vcs. Expected 'git' or 'tfs'."
        | "--file" :: xs ->
            match xs with
            | log :: xs' -> parseRec xs' {options with method = Method.LogFile log}
            | []         -> Error "--file requires an argument."
        | "--ignore" :: xs ->
            match xs with
            | extensions :: xs' ->
                parseRec xs' {options with ignore = List.ofArray <| extensions.Split(',')}
            | _ -> Error "--ignore requires comma separated extensions as argument."
        | other -> Error <| "Unknown argument: " + (String.Join(" ", other))

    let parse args =
        let defaultOptions = { 
            vcs = Vcs.Git;
            method = Method.Process;
            ignore = [];
            min = 3;
        }
        parseRec args defaultOptions
        