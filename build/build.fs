module Build

#nowarn 1182
#nowarn 20

open System
open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript

let argValOrNone name (args: DocoptMap) =
    DocoptResult.tryGetArgument name args
    |> Option.bind (fun v -> if String.IsNullOrWhiteSpace(v) then None else Some v)

let argValOrDefault name defaultValue (args: DocoptMap) =
    DocoptResult.tryGetArgument name args |> Option.defaultValue defaultValue

let argValBool name (args: DocoptMap) = DocoptResult.hasFlag name args

let buildscript () =
    Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ </> ".."

    let (!!) includes =
        (!!includes).SetBaseDirectory(__SOURCE_DIRECTORY__ </> "..")

    let argv = Target.getArguments () |> Option.defaultValue [||]

    let argParser =
        Docopt(
            """
usage: build.fsx [options]

options:
    --PublishOutput <path>  Specify output directory for Publish target
    --Arch <arch>           Specify target architecture for Publish (e.g. win-x64, linux-x64, osx-x64)
    --TargetProject <path>  Specify target project or solution for Build and Publish (default: EvgTsvDotMe.sln)
    --SkipRestore           Skip restoring dependencies
    """
        )

    let args = argParser.Parse(argv)

    let skipRestore = args |> argValBool "--SkipRestore"

    let publishOutput = args |> argValOrNone "--PublishOutput"
    let targetArch = args |> argValOrNone "--Arch"
    let targetProject = args |> argValOrDefault "--TargetProject" "EvgTsvDotMe.sln"
    
    let workflowScriptPath = "./Scripts/buildAndDeploy.fsx"

    // --------------------------------------------------------------------------------------
    // Information about the project
    // --------------------------------------------------------------------------------------

    let project = "EvgTsvDotMe"
    let authors = "Evgenii Tsvetkov"
    let summary = "My personal website and blog built with F# and Oxpecker"

    let description =
        """
    The EvgTsvDotMe project is a personal website and blog built with F# and the Oxpecker web framework.
    It serves as a platform for sharing insights, tutorials, and projects related to F# programming, software development,
    and technology in general. The website features a clean and modern design, blazing fast performance, and a focus on content quality.
    It includes a blog section where I regularly post articles on various topics, as well as a portfolio showcasing my projects and contributions to the F# community.
    The project is open source and available on GitHub, inviting collaboration and contributions from other developers interested in F# and web development.

    * EvgTsvDotMe -- main web application project
    * EvgTsvDotMe.PagesProvider -- F# Type Provider for loading page paths depending on the files in the `View/Pages` directory
    * Tailwind CSS -- used for styling the website with a utility-first approach
    * Htmx -- used for making the website more dynamic and responsive without needing a full"""

    let gitOwner = "evgTsv"
    let gitHome = "https://github.com/" + gitOwner
    let gitName = "EvgTsvDotMe"

    let repositoryType = "git"
    let repositoryUrl = "https://github.com/evgTsv/EvgTsvDotMe"
    let license = "MIT"

    let isCI = Environment.GetEnvironmentVariable("CI") <> null

    // --------------------------------------------------------------------------------------
    // Restore project dependencies

    Target.create "Restore" (fun _ ->
        if skipRestore then
            Trace.log "Skipping restore because SkipRestore=true"
        else
            let result = DotNet.exec id "paket" "restore"

            if not result.OK then
                Trace.log "Paket restore failed with messages:"
                result.Messages |> Seq.iter (Trace.logf "  %s")
                failwith $"Paket restore failed with message: %A{result.Messages}"

            Trace.log "Paket restore completed successfully")

    // --------------------------------------------------------------------------------------
    // Clean build results

    Target.create "Clean" (fun _ ->
        seq {
            yield! !!"src/**/bin"
            yield! !!"tests/**/bin"
            yield! !!"src/**/obj"
            yield! !!"tests/**/obj"

            if publishOutput.IsSome then
                yield publishOutput.Value
        }
        |> Shell.cleanDirs)

    // --------------------------------------------------------------------------------------
    // Build library & test projects

    Target.create "Build" (fun _ ->
        targetProject
        |> DotNet.build (fun o -> {
            o with
                Configuration = DotNet.BuildConfiguration.Release
                DotNet.BuildOptions.MSBuildParams.DisableInternalBinLog = true
        }))

    Target.create "RunTests" (fun _ ->
        let setParams (o: DotNet.TestOptions) = {
            o with
                Configuration = DotNet.BuildConfiguration.Release
                DotNet.TestOptions.MSBuildParams.DisableInternalBinLog = true
                Logger = if isCI then Some "GitHubActions" else None
        }

        "EvgTsvDotMe.sln" |> DotNet.test setParams)

    // --------------------------------------------------------------------------------------
    // Publish the application

    Target.create "Publish" (fun _ ->
        let setParams (o: DotNet.PublishOptions) = {
            o with
                Configuration = DotNet.BuildConfiguration.Release
                OutputPath = publishOutput
                DotNet.PublishOptions.Common.CustomParams =
                    targetArch |> Option.map (fun arch -> $"-a {arch}")
                DotNet.PublishOptions.MSBuildParams.DisableInternalBinLog = true
        }

        "src/EvgTsvDotMe/EvgTsvDotMe.fsproj" |> DotNet.publish setParams)

    // --------------------------------------------------------------------------------------
    // Run tests with code coverage (uses Coverlet collector already present in test projects)

    Target.create "Coverage" (fun _ ->
        let setParams (o: DotNet.TestOptions) = {
            o with
                Configuration = DotNet.BuildConfiguration.Release
                Collect = Some "XPlat Code Coverage"
                ResultsDirectory = Some "coverage-results"
                DotNet.TestOptions.MSBuildParams.DisableInternalBinLog = true
                Logger = if isCI then Some "GitHubActions" else None
        }

        "FSharp.Data.sln" |> DotNet.test setParams

        Trace.log ""
        Trace.log "Coverage results written to ./coverage-results/"
        Trace.log "To generate an HTML report, install dotnet-reportgenerator-globaltool:"
        Trace.log "  dotnet tool install -g dotnet-reportgenerator-globaltool"

        Trace.log
            "  reportgenerator -reports:coverage-results/**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html"

        Trace.log "")

    Target.create "AddHtmx" (fun _ ->
        let dest = "src\EvgTsvDotMe\wwwroot\js\htmx.min.js"

        if (Shell.testFile dest) then
            Trace.log "htmx.min.js already exists in wwwroot/js/, skipping copy"
        else
            let restored = !!"paket-files/**/htmx.min.js" |> Seq.tryHead

            match restored with
            | Some src ->
                Trace.log $"Copying htmx.min.js from {src} to {dest}"
                Shell.copyFile dest src
            | None ->
                Trace.log "Could not find htmx.min.js in paket-files:"

                Trace.log
                    "  Make sure to run `dotnet run --project build/build.fsproj -- -t Restore` first"

                failwith "htmx.min.js not found in paket-files")

    Target.create "BuildTailwind" (fun _ ->
        Trace.log "Starting Tailwind CSS build..."
        let tailwindDir = "./src/Tailwind"

        Npm.install (fun o -> {
            o with
                WorkingDirectory = tailwindDir
        })

        Npm.run "build" (fun o -> {
            o with
                WorkingDirectory = tailwindDir
        })

        Trace.log "Tailwind CSS build completed successfully.")

    // --------------------------------------------------------------------------------------
    // Help

    Target.create "Help" (fun _ ->
        printfn ""
        printfn "  Please specify the target by calling 'build -t <Target>'"
        printfn ""
        printfn "  Additional arguments:"
        printfn "  * ARG: SkipRestore=true to skip restoring dependencies"
        printfn "  * ARG: PublishOutput=path to specify output directory for Publish target"

        printfn
            "  * ARG: Arch=architecture to specify target architecture for Publish (e.g. win-x64, linux-x64, osx-x64)"

        printfn "  * ENV: CI=true to enable CI-specific settings"
        printfn ""
        printfn "  Targets for building:"
        printfn "  * AddHtmx"
        printfn "  * BuildTailwind"
        printfn "  * Frontend (calls previous 2)"
        printfn "  * Restore"
        printfn "  * Build"
        printfn "  * RunTests"

        printfn
            "  * Coverage (run tests with Coverlet code coverage; results in ./coverage-results/)"

        printfn "  * All (calls previous 4)"
        printfn "  * Publish (calls Build and then publishes the app)"
        printfn ""
        printfn "  Other targets:"
        printfn "  * Clean"
        printfn "  * Format"
        printfn "  * CheckFormat"
        printfn "  * VerifyWorkflow"
        printfn "  * GenWorkflow"
        printfn "")

    Target.create "Format" (fun _ ->
        let result = DotNet.exec id "fantomas" "."

        if not result.OK then
            printfn $"Errors while formatting all files: %A{result.Messages}")

    Target.create "CheckFormat" (fun _ ->
        let result = DotNet.exec id "fantomas" ". --check"

        if result.ExitCode = 0 then
            Trace.log "No files need formatting"
        elif result.ExitCode = 99 then
            failwith
                "Some files need formatting, run `dotnet run --project build/build.fsproj -- -t Format` to format them"
        else
            Trace.logf $"Errors while formatting: %A{result.Errors}"
            failwith "Unknown errors while formatting")
    
    Target.create "VerifyWorkflow" (fun _ ->
        let result = DotNet.exec id "fsi" $"{workflowScriptPath} verify"
        
        if not result.OK then
            Trace.logf $"Workflow verification failed with messages: %A{result.Messages}"
            failwith "Workflow verification failed"
        else
            Trace.log "Workflow verification succeeded")
    
    Target.create "GenWorkflow" (fun _ ->
        let result = DotNet.exec id "fsi" $"{workflowScriptPath}"
        
        if not result.OK then
            Trace.logf $"Workflow generation failed with messages: %A{result.Messages}"
            failwith "Workflow generation failed"
        else
            Trace.log "Workflow generation succeeded")

    Target.create "All" ignore
    Target.create "Frontend" ignore

    "AddHtmx" ==> "Frontend"
    "BuildTailwind" ==> "Frontend"

    "Clean" ==> "Build"
    "Frontend" ==> "Build"
    "Restore" ==> "Build"

    "Build" ==> "Coverage"
    "Build" ==> "RunTests" ==> "All"

    "Build" ==> "Publish"

[<EntryPoint>]
let main argv =
    argv
    |> Array.toList
    |> Context.FakeExecutionContext.Create false "build.fsx"
    |> Context.RuntimeContext.Fake
    |> Context.setExecutionContext

    buildscript ()
    Target.runOrDefaultWithArguments "Help"
    0
