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
        let mutable n = 0
        if Int32.TryParse(s, ref n) then
            Result.Ok(n)
        else
            Error (String.Format("Unparsable int: {0}", [n]))

    let rec private parseRec args options =
        match args with
        | [] -> Result.Ok options
        | "--min" :: xs ->
            match xs with
            | num :: xs' ->
                Result.bind (fun n -> parseRec xs' {options with min = n}) (tryParse num)
            | []         -> Result.Error "--file requires an argument."
        | "--vcs" :: xs ->
            match xs with
            | "git" :: xs' -> parseRec xs' {options with vcs = Vcs.Git }
            | "tfs" :: xs' -> parseRec xs' {options with vcs = Vcs.Tfs }
            | _            -> Result.Error "Invalid argument for --vcs. Expected 'git' or 'tfs'."
        | "--file" :: xs ->
            match xs with
            | log :: xs' -> parseRec xs' {options with method = Method.LogFile log}
            | []         -> Result.Error "--file requires an argument."
        | "--ignore" :: xs ->
            match xs with
            | extensions :: xs' ->
                parseRec xs' {options with ignore = List.ofArray <| extensions.Split(',')}
            | _ -> Result.Error "--ignore requires comma separated extensions as argument."
        | other -> Result.Error <| "Unknown argument: " + (String.Join(" ", other))

    let parse args =
        let defaultOptions = { 
            vcs = Vcs.Git;
            method = Method.Process;
            ignore = [];
            min = 3;
        }
        parseRec args defaultOptions
        