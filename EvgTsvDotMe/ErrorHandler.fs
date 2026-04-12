module EvgTsvDotMe.ErrorHandler

open System
open System.Net
open System.Threading.Tasks
open EvgTsvDotMe.Models
open EvgTsvDotMe.Utils
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Oxpecker

open type Microsoft.AspNetCore.Http.TypedResults

let issueError code msg (details: obj option) (ctx: HttpContext) =
    {
        code = code
        message = msg
        traceId = ctx.TraceIdentifier
        timestamp = DateTime.UtcNow.ToString("O")
        path = ctx.Request.Path.ToString()
        details = details
    }

let notFoundHandler: EndpointHandler =
    fun ctx ->
        let html =
            issueError HttpStatusCode.NotFound ("Invalid route:" + ctx.Request.Path) None ctx
            |> View.Error.html
            
        ctx |> writeHtml html

let errorHandler (ctx: HttpContext) (next: RequestDelegate) =
    let logger = Env.logger ctx
    task {
        try
            return! next.Invoke(ctx)
        with
        | :? RouteParseException as ex ->
            logger.LogWarning(ex, "Route parse error")
        
            let html =
                issueError HttpStatusCode.BadRequest ("Invalid route:" + ex.Message) None ctx
                |> View.Error.html
                
            return! ctx |> writeHtml html
        | ex ->
            logger.LogError(ex, "Unhandled error")

            let html =
                issueError HttpStatusCode.InternalServerError "Internal server error: an unexpected error occurred" None ctx
                |> View.Error.html
                
            return! ctx |> writeHtml html
    }
    :> Task    
