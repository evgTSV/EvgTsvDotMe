module EvgTsvDotMe.View.Layout

open System
open EvgTsvDotMe.CssUtils
open EvgTsvDotMe.View.Components
open Microsoft.AspNetCore.Http
open Oxpecker.ViewEngine
open Oxpecker.Htmx
open EvgTsvDotMe.Env.View

let navLink (text: string) (url: string) =
    a(
        class' = "px-4 py-2 text-sm font-mono tracking-tighter text-zinc-400 hover:text-green-500 hover:bg-zinc-900 transition-all duration-200 cursor-pointer",
        hxGet = url,
        hxTarget = "#main-content",
        hxPushUrl = "true"
    ) { 
        raw text 
    }

let topNavBar (ctx: HttpContext) =
    let lang = (language ctx).Data
    
    nav(
        class' = "sticky top-0 z-50 w-full border-b border-zinc-800 bg-zinc-950/90 backdrop-blur-md"
    ) {
        div( class' = "max-w-7xl mx-auto px-4 sm:px-6 lg:px-8" ) {
            div( class' = "flex items-center justify-between h-14" ) {
                
                div( class' = "flex items-center gap-4" ) {
                    span(
                        class' = "text-lg font-bold font-mono text-white tracking-widest",
                        hxGet = "/",
                        hxTarget = "#main-content",
                        hxPushUrl = "true"
                    ) {
                        b() { raw "EVG_TSV" }
                        span( class' = "text-green-600" ) { raw ":" }
                        span( class' = "text-zinc-500 text-sm" ) { raw (Guid.NewGuid().ToString()[..4]) }
                    }
                    
                    div( class' = "hidden md:flex ml-10 space-x-1" ) {
                        navLink lang.NavBar.Home "/"
                        navLink lang.NavBar.Projects "/projects"
                        navLink lang.NavBar.CurriculumVitae "/cv"
                    }
                }
                
                a(
                    href = "https://fsharp.org",
                    target = "_blank",
                    rel = "noopener noreferrer",
                    class' = "flex items-center gap-2 px-3 py-1 bg-zinc-900 border border-zinc-700 rounded-md hover:bg-zinc-800 hover:border-green-500/50 transition-all duration-300 group"
                ) {
                    div( class' = "w-5 h-5 text-zinc-400 transition-all duration-300 group-hover:text-green-400 group-hover:drop-shadow-[0_0_4px_rgba(225,24,192,0.5)]" ) {
                        Svg.Logos.FSharpDark()
                    }
                    span( class' = "text-[10px] font-mono text-zinc-400 uppercase tracking-widest group-hover:text-green-400 transition-all duration-300" ) { 
                        raw "FSharp.Optimized" 
                    }
                    
                    div( class' = "w-2 h-2 rounded-full bg-green-500 animate-pulse" ) {}
                }
            }
        }
    }
    
let pageLayout (content: HtmlElement) (ctx: HttpContext) =
    let isHtmxRequest = 
        ctx.Request.Headers.ContainsKey "HX-Request" &&
        ctx.Request.Headers["HX-Request"].ToString() = "true"
    
    if isHtmxRequest then
        content
    else
        
    html() {
        head() {
            meta(charset = "utf-8")
            meta(name = "viewport", content = "width=device-width, initial-scale=1.0")
            title() { "EvgTsv.me" }
            link(rel = "stylesheet", href = versionedCssPath())
            script(src = "/js/htmx.min.js") {}
        }
        body(hxBoost = true) {
            topNavBar ctx
            div(id = "main-content") {
                content
            }
            script(src = "/js/htmx.min.js") {}
        }
    }
