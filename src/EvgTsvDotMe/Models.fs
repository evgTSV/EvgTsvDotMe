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

[<RequireQualifiedAccess>]
type PRStatus =
    | Open
    | Closed
    | Merged

type PullRequest = {
    id: int
    title: string
    url: string
    status: PRStatus
}
