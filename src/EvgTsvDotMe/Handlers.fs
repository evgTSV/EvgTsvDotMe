module EvgTsvDotMe.Handlers

open EvgTsvDotMe.Utils
open EvgTsvDotMe.View.Pages
open Oxpecker

let pingHandler = setStatusCode 200 >=> text "PONG!"

let getMainPage: EndpointHandler = fun ctx -> ctx |> writeHtml Home.html
