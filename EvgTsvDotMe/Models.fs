module EvgTsvDotMe.Models

open System.Net

[<CLIMutable>]
type ApiError = {
    code: HttpStatusCode
    message: string
    traceId: string
    timestamp: string
    path: string
    details: obj option
}