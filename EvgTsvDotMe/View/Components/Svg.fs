module EvgTsvDotMe.View.Components.Svg

open System
open Oxpecker.ViewEngine

type svg(
    content: string,
    ?class': string,
    ?width: string,
    ?height: string,
    ?viewBox: string,
    ?version: string,
    ?xmlns: string,
    ?xmlnsXlink: string,
    ?xmlSpace: string,
    ?xmlnsSerif: string,
    ?style: string
    ) =
    interface HtmlElement with
        member _.Render(sb) =
            let attrs =
                [
                    class' |> Option.map (fun v -> $"class=\"{v}\"")
                    width |> Option.map (fun v -> $"width=\"{v}\"")
                    height |> Option.map (fun v -> $"height=\"{v}\"")
                    viewBox |> Option.map (fun v -> $"viewBox=\"{v}\"")
                    version |> Option.map (fun v -> $"version=\"{v}\"")
                    xmlns |> Option.map (fun v -> $"xmlns=\"{v}\"")
                    xmlnsXlink |> Option.map (fun v -> $"xmlns:xlink=\"{v}\"")
                    xmlSpace |> Option.map (fun v -> $"xml:space=\"{v}\"")
                    xmlnsSerif |> Option.map (fun v -> $"xmlns:serif=\"{v}\"")
                    style |> Option.map (fun v -> $"style=\"{v}\"")
                ] |> List.choose id
            attrs |> String.concat " " |> sprintf "<svg %s>" |> sb.Append |> ignore
            sb.Append(content).Append("</svg>") |> ignore

let fsharpLogoDarkContent =
        """
<g transform="matrix(0.587493,0.587493,-0.587493,0.587493,46.7625,-10.9012)">
    <path d="M14.587,14L81.074,14L81.074,80.487L64.717,64.13L64.717,30.357L30.944,30.357L14.587,14Z" style="fill:rgb(184,69,252);"/>
</g>
<g transform="matrix(0.587493,0.587493,-0.587493,0.587493,46.7625,-10.9012)">
    <path d="M56.048,62.635L32.439,62.635L32.439,39.026L56.048,62.635ZM77.487,84.074L11,84.074L11,17.587L27.357,33.944L27.357,67.717L61.13,67.717L77.487,84.074Z" style="fill:url(#_Linear1);"/>
</g>
<defs>
    <linearGradient id="_Linear1" x1="0" y1="0" x2="1" y2="0" gradientUnits="userSpaceOnUse" gradientTransform="matrix(103.572,0,0,103.572,-22.543,50.531)"><stop offset="0" style="stop-color:rgb(184,69,252);stop-opacity:1"/><stop offset="1" style="stop-color:rgb(68,28,129);stop-opacity:1"/></linearGradient>
</defs>
"""

type Logos =
    static member DefaultSize = "100%"
    
    static member FSharpDark(?width, ?height) =
        svg(
            content = fsharpLogoDarkContent,
            width = (width |> Option.defaultValue Logos.DefaultSize),
            height = (height |> Option.defaultValue Logos.DefaultSize),
            viewBox = "0 0 90 90",
            version = "1.1",
            xmlns = "http://www.w3.org/2000/svg",
            xmlnsXlink = "http://www.w3.org/1999/xlink",
            xmlSpace = "preserve",
            style = "fill-rule:evenodd;clip-rule:evenodd;stroke-linejoin:round;stroke-miterlimit:2;"
        )
    
