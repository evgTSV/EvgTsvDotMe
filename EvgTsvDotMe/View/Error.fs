module EvgTsvDotMe.View.Error

open EvgTsvDotMe.View.Layout
open Oxpecker.ViewEngine
open Oxpecker.Htmx
open EvgTsvDotMe.Models

let html (error: ApiError) =
    main(class' = "pt-20 pb-10 px-4 max-w-4xl mx-auto font-mono") {
        section(class' = "mb-12") {
            h1(class' = "text-5xl font-black text-red-500 tracking-tighter mb-2") { "Error" }
            p(class' = "text-zinc-500 text-lg") { error.message }
        }

        div(class' = "grid gap-6") {
            div(class' = "p-6 bg-zinc-900 border border-zinc-800") {
                h2(class' = "text-zinc-100 text-xl mb-4") { "[ ERROR_DETAILS ]" }
                pre(class' = "text-zinc-500 text-sm overflow-x-auto") { raw $"%A{error}" }
            }
        }
        
        div(class' = "mt-10 text-center") {
            a(
                hxGet = "/",
                hxTarget = "#main-content",
                hxPushUrl = "true"
            ) { raw "Go Back Home" }
        }
    }
    |> pageLayout
