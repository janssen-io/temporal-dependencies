module Temporal.Console.Args

open System
open Argu

type Vcs =
    | Git
    | Tfs

type Options = {
    vcs: Vcs
    file: string
    excluded: string list
    included: string list
    min: int
    top: int option
}

type Args =
    | [<Mandatory>] Vcs of Vcs
    | [<MainCommand>] File of string
    | [<AltCommandLine("-x")>] Exclude of string list
    | [<AltCommandLine("-i")>] Include of string list
    | Min of int
    | Top of int
with
    static member Entered opt (options:Options) =
        match opt with
        | Vcs _     -> sprintf "VCS:\t%A" options.vcs
        | File _    -> sprintf "File:\t%s" options.file
        | Min _     -> sprintf "Min:\t%i" options.min
        | Top _     -> sprintf "Top:\t%s" <| options.top.ToString ()
        | Exclude _ -> sprintf "Exclude:\t[%s]" <| String.Join ("; ", options.excluded)
        | Include _ -> sprintf "Include:\t[%s]" <| String.Join ("; ", options.included)

    interface IArgParserTemplate with
        member opt.Usage =
            match opt with
            | Vcs _     -> "Use Git or TFS to retrieve changes. (Default: git)"
            | File _    -> "Read changes from file instead of calling tfs/git."
            | Min _     -> "Only show dependencies that occur n times or more."
            | Top _     -> "Only show the top n dependencies."
            | Exclude _ -> "Space separated list of excluded file extensions."
            | Include _ -> "Space separated list of file extensions that are accepted."
        
    
let private errorHandler =
    ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
let private parser =
    ArgumentParser.Create<Args>(programName = "Temporal.Console.exe", errorHandler = errorHandler)

let parse argv = 
    let results = parser.Parse argv
    {
        vcs = results.GetResult(Vcs)
        file = results.GetResult(File)
        excluded = results.GetResult(Exclude, defaultValue = [])
        included = results.GetResult(Include, defaultValue = [])
        min = results.GetResult(Min, defaultValue = 1)
        top = results.TryGetResult(Top)
    }