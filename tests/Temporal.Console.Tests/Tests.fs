module Tests

open Xunit
open Temporal.Console

[<Theory>]
[<InlineData("--vcs", "git")>]
[<InlineData("--file", "commits.log")>]
[<InlineData("--ignore", ".fsproj,.sln")>]
let ``Returns an Ok result`` arg1 arg2 =
    match Args.parse [arg1; arg2] with
    | Ok _ -> Assert.True(true)
    | _    -> failwith "Unexpected Error Result"
  
[<Theory>]
[<InlineData("--vcs")>]
[<InlineData("--file")>]
[<InlineData("--ignore")>]
let ``Returns an Error result1`` arg1 =
    match Args.parse [arg1] with
    | Error _ -> Assert.True(true)
    | _    -> failwith "Unexpected Ok Result"

[<Fact>]
let ``Parses vcs option`` =
    match Args.parse ["--vcs"; "git"] with
    | Ok options -> Assert.Equal(Args.Vcs.Git, options.vcs)
    | _ -> failwith "Unexpected Error Result"

    match Args.parse ["--vcs"; "tfs"] with
    | Ok options -> Assert.Equal(Args.Vcs.Tfs, options.vcs)
    | _ -> failwith "Unexpected Error Result"

[<Fact>]
let ``Parses ignore option`` =
    match Args.parse ["--ignore"; ".sln,.csproj,.fsproj"] with
    | Ok options -> Assert.True ([".sln";".csproj";".fsproj"] = options.ignore)
    | _ -> failwith "Unexpected Error Result"

[<Fact>]
let ``Parses min option`` =
    match Args.parse ["--min"; "5"] with
    | Ok options -> Assert.True (5 = options.min)
    | _ -> failwith "Unexpected Error Result"

[<Fact>]
let ``Parses top option`` =
    match Args.parse ["--top"; "5"] with
    | Ok options -> Assert.True (Some 5 = options.top)
    | _ -> failwith "Unexpected Error Result"