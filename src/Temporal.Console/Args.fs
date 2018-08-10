module Temporal.Console.Args

open System
open System.Text

type Vcs =
    | Git
    | Tfs

type Method =
    | LogFile of string
    | Process

type Options = {
    vcs: Vcs
    method: Method
    ignore: string list
    min: int
    top: int option
}

let private tryParse (s:string) =
    match Int32.TryParse s with
    | true, v  -> Ok v
    | false, _ -> Error (sprintf "Invalid integer '%s'" s)

let private append (s:string) (sb:StringBuilder) =
    sb.AppendLine s

let helpMessage =
    let message = new StringBuilder()
    message.AppendLine "--help\t\tShow this message."
    |> append "--vcs (git|tfs)\tUse Git or TFS to view changes. (Default: git)"
    |> append "--min n\t\tOnly show dependencies that occur n times or more. (Default: 3)"
    |> append "--top n\t\tOnly show the top n dependencies. (Default: all)"
    |> append "--ignore a,b,c\tIgnored extensions (comma separated). (Default: nothing)"
    |> append "--file\t\tRead changes from file instead of calling tfs/git. (Default: nothing)"
    |> (fun (s:StringBuilder) -> s.ToString())

let rec private parseRec args options =
    match args with
    | [] -> Ok options
    | "--help" :: _ -> Error helpMessage
    | "--top" :: xs ->
        match xs with
        | num :: xs' ->
            Result.bind (fun n -> parseRec xs' {options with top = Some n}) (tryParse num)
        | []         -> Error "--top requires an integer argument."
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
        | []         -> Error "--file requires an argument. Please use the --file argument[."
    | "--ignore" :: xs ->
        match xs with
        | extensions :: xs' ->
            parseRec xs' {options with ignore = List.ofArray <| extensions.Split(',')}
        | _ -> Error "--ignore requires comma separated extensions as argument."
    | other -> Error <| "Unknown argument: " + (String.Join(" ", other))

let parse args =
    let defaultOptions = { 
        vcs = Vcs.Git
        method = Method.Process
        ignore = []
        min = 3
        top = None
    }
    parseRec args defaultOptions
    