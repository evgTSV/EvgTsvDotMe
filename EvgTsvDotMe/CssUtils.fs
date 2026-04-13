module EvgTsvDotMe.CssUtils

open System
open System.IO
open System.Security.Cryptography

let private getCssHash () =
    let cssPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "css", "style.css")
    if File.Exists cssPath then
        let hash = 
            File.ReadAllBytes cssPath
            |> SHA256.HashData
            |> BitConverter.ToString
            |> _.Replace("-", "").ToLower()

        hash[..7]  // First 8 characters
    else
        "dev"

let cssVersion = lazy (getCssHash ())

let versionedCssPath () =
    $"/css/style.css?v=%s{cssVersion.Value}"

