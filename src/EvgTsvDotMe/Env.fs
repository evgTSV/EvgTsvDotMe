module EvgTsvDotMe.Env

open EvgTsvDotMe.Localization.Translations
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open EvgTsvDotMe.PageResolving

let inline logger (ctx: HttpContext) =
    ctx.RequestServices.GetRequiredService<ILogger>()

type Pages = PagesProvider<PagesDir="View/Pages">

module View =

    let inline language (ctx: HttpContext) =
        let acceptLang = ctx.Request.Headers["Accept-Language"].ToString()
        let lang = acceptLang.Split(',').[0].Trim()
        tryLoadLanguage lang
