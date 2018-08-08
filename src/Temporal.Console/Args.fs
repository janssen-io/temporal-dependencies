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
    }

    let rec private parseRec args options =
        match args with
        | [] -> Result.Ok options
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
            ignore = []
        }
        parseRec args defaultOptions
        