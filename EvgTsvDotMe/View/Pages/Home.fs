module EvgTsvDotMe.View.Pages.Home

open EvgTsvDotMe.Utils
open EvgTsvDotMe.Env.View
open EvgTsvDotMe.View.Layout
open Microsoft.AspNetCore.Http
open Oxpecker.ViewEngine
open Oxpecker.Htmx

let html (ctx: HttpContext) =
    let lang = (language ctx).Data
    
    main(class' = "pt-20 pb-10 px-4 max-w-4xl mx-auto font-mono") {
        section(class' = "mb-12") {
            h1(class' = "text-5xl font-black text-white tracking-tighter mb-2") { lang.FullName }
            p(class' = "text-green-500 text-lg") { "|> F# Senior. Architect. Slop Decoder." }
        }

        div(class' = "grid gap-6") {
            div(class' = "p-6 bg-zinc-900 border border-zinc-800 overflow-x-auto w-full") {
                h2(class' = "text-zinc-100 text-xl mb-4") { "[ RECENT_ACTIVITY ]" }
                div(id = "activity-log", class' = "text-zinc-500 text-sm") { "Loading chunks..." }
                
                button(
                    hxGet = "/api/activity", 
                    hxTarget = "#activity-log",
                    hxTrigger = "load",
                    class' = "hidden"
                ) { "" }
            }

            button(
                class' = "w-full py-4 border-2 border-green-600 text-green-500 hover:bg-green-600 hover:text-black transition-all font-bold",
                hxPost = "/api/deploy-steel"
            ) {
                "EXECUTE: DEPLOY_CLEAN_CODE"
            }
        }
    }
    |> (swap pageLayout) ctx
