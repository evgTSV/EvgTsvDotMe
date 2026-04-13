open EvgTsvDotMe
open EvgTsvDotMe.ErrorHandler
open EvgTsvDotMe.Handlers
open Microsoft.Extensions.DependencyInjection
open Oxpecker
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

open type Env.Pages

let inline (~%) x = x |> ignore

let endpoints = [
    GET [
        route "/" <| redirectTo Home.Path true
        subRoute "/api/v1" [ route "ping" <| pingHandler ]

        // Pages
        route Home.Path <| getMainPage
    ]
]

let configureServices (services: IServiceCollection) = %services.AddRouting().AddOxpecker()

let configureApp (app: WebApplication) =
    %app.UseStaticFiles().Use(errorHandler).UseRouting().UseOxpecker(endpoints).Run(notFoundHandler)

let run () =
    let builder = WebApplication.CreateBuilder()
    configureServices builder.Services

    let app = builder.Build()
    configureApp app

    app.Run()

run ()
