module EvgTsvDotMe.PagesProviderImplementation

open System
open System.IO
open System.Reflection
open FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes

[<AutoOpen>]
module Resolver =
    
    [<Literal>]
    let maxDepthOfFiles = 10
    
    type PageInfo =
        | PageInfo of name: string * path: string
        | SubModule of name: string * inners: PageInfo list
        
    let getAllPages (config: TypeProviderConfig) pagesDir (basePath: string) =
        let rootPath = Path.GetFullPath(Path.Combine(config.ResolutionFolder, pagesDir))
        
        if not (Directory.Exists rootPath) then
            failwithf $"Failed to find the pages directory! Dir: %s{rootPath}"

        let cleanBasePath = basePath.TrimEnd('/')

        let rec traverse currentDir depth =
            [
                if depth > maxDepthOfFiles then
                    ()
                else
                    
                let files = Directory.EnumerateFiles(currentDir, "*.fs")
                for file in files do
                    let pageName = Path.GetFileNameWithoutExtension(file)
                    
                    let relativeDir = 
                        if currentDir.Length <= rootPath.Length then ""
                        else currentDir.Substring(rootPath.Length).Replace(Path.DirectorySeparatorChar, '/')
                        + $"/{pageName}"
                            
                    let pagePath = cleanBasePath + relativeDir
                    yield PageInfo(pageName, pagePath.ToLower())

                let subDirs = Directory.EnumerateDirectories(currentDir)
                for dir in subDirs do
                    yield SubModule(Path.GetFileName(dir) + "'", traverse dir (depth + 1))
            ]

        traverse rootPath 0
        
        
[<TypeProvider>]
type PagesProvider(config: TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces (
        config, assemblyReplacementMap=[ ("EvgTsvDotMe.PagesProvider.DesignTime",
                                          "EvgTsvDotMe.PagesProvider.Runtime") ], addDefaultProbingLocation=true)
    
    let ns = "EvgTsvDotMe.PageResolving"
    let asm = Assembly.GetExecutingAssembly()

    // check we contain a copy of runtime files, and are not referencing the runtime DLL
    // do assert (typeof<Page>.Assembly.GetName().Name = asm.GetName().Name)
    
    let rec createPageTy (info: PageInfo) =
        match info with
        | PageInfo(name, path) ->
            let pageTy = ProvidedTypeDefinition(asm, ns, name, Some typeof<obj>)
                
            let pathProp =
                    ProvidedProperty("Path", typeof<string>,
                                             isStatic=true,
                                             getterCode = fun _args -> <@@ path @@>)
            pageTy.AddMember pathProp
            
            pageTy
        | SubModule(name, inners) ->
            let modTy = ProvidedTypeDefinition(asm, ns, name, Some typeof<obj>)
            inners |> List.map createPageTy |> List.iter modTy.AddMember
            modTy

    let createTypes (typeName, pagesDir, basePath) =
        let t = ProvidedTypeDefinition(asm, ns, typeName, Some typeof<obj>)
        
        getAllPages config pagesDir basePath
        |> List.map createPageTy
        |> List.iter t.AddMember
        
        t

    do
        let pagesContainer = ProvidedTypeDefinition(asm, ns, "PagesProvider", Some typeof<obj>)
        let staticParams = [
            ProvidedStaticParameter("PagesDir", typeof<string>)
            ProvidedStaticParameter("BasePath", typeof<string>,
                                    parameterDefaultValue = "/pages")
        ]

        pagesContainer.DefineStaticParameters(
            staticParams, 
            fun typeName args ->
                let pagesDir = args[0] :?> string
                let basePath = args[1] :?> string
                createTypes(typeName, pagesDir, basePath))
        
        this.AddNamespace(ns, [pagesContainer])