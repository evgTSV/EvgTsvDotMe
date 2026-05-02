module Utils

module Args =
    open System
    open Fake.Core

    let argValOrNone name (args: DocoptMap) =
        DocoptResult.tryGetArgument name args
        |> Option.bind (fun v -> if String.IsNullOrWhiteSpace(v) then None else Some v)

    let argValOrDefault name defaultValue (args: DocoptMap) =
        DocoptResult.tryGetArgument name args |> Option.defaultValue defaultValue

    let argValBool name (args: DocoptMap) = DocoptResult.hasFlag name args
