namespace Temporal.Core.Input

open System 
open System.IO
open Temporal.Core.Helpers
open Temporal.Core.Computation

module GitTransformer =
    let newCommit (x:string) = x.StartsWith "new commit"

    let groupByCommit =
        List.filter (not << String.IsNullOrWhiteSpace)
        >> List.map (fun (s:string) -> s.Trim())
        >> split newCommit
        >> List.filter (not << List.isEmpty)

    let countToString ((a:string, b:string), count:int) = count.ToString() + ": " + a + ", " + b

    let getDependenciesFromFile = 
        File.ReadAllLines
        >> List.ofArray
        >> groupByCommit
        >> computeTemporalDependencies