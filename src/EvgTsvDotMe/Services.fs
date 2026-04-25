module EvgTsvDotMe.Services

open EvgTsvDotMe.Utils

module GitHubService =

    open Octokit

    type GHClient() =

        let client = GitHubClient(ProductHeaderValue("EvgTsvDotMe"))

        do
            tryGetEnv "GITHUB_TOKEN"
            |> Option.iter (fun token -> client.Credentials <- Credentials(token))

        member _.Client = client

    let client = GitHubClient(ProductHeaderValue("EvgTsvDotMe"))

    [<Literal>]
    let username = "evgTsv"

    let getPullRequest (page: int, offset: int) =
        task {
            let! user = client.User.Get(username)
            user
        }

module ActivityService =

    let getLatestPullRequest () = ()
