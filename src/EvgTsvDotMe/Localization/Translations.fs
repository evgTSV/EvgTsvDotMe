module EvgTsvDotMe.Localization.Translations

open System
open System.IO
open EvgTsvDotMe.Utils
open FSharp.Data
open FsToolkit.ErrorHandling

[<Literal>]
let LangPath = "Localization/Translations/"

type Provider = JsonProvider<"Localization/Translations/en-US.json">

type Language = { Name: string; Data: Provider.Root }

let AvailableLanguages = [
    for file in Directory.EnumerateFiles(LangPath, "*.json") do
        file |> Path.GetFileNameWithoutExtension
]

let loadTranslation =
    memo StringComparer.InvariantCultureIgnoreCase (fun name ->
        if AvailableLanguages |> List.contains name then
            let lang = Provider.Load(Path.Combine(LangPath, $"{name}.json"))
            Some <| { Name = name; Data = lang }
        else
            None)

let DefaultLanguage =
    AvailableLanguages
    |> List.tryFind _.Equals("en-US", StringComparison.InvariantCultureIgnoreCase)
    |> Option.bind loadTranslation
    |> Option.orElseWith (fun () ->
        option {
            let! n = AvailableLanguages |> List.tryHead
            return! loadTranslation n
        })
    |> Option.defaultWith (fun () ->
        failwith
            "No available languages found! Please add at least one translation file to the project.")

let tryLoadLanguage name =
    if String.IsNullOrWhiteSpace name then
        DefaultLanguage
    else
        let lang = loadTranslation name
        lang |> Option.defaultValue DefaultLanguage
