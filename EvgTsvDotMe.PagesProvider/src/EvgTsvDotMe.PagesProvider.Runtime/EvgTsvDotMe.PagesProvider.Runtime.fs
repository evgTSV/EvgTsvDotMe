namespace EvgTsvDotMe.PagesProvider

open System

[<AutoOpen>]
module Helpers =
    
    let runtimeDir = Environment.CurrentDirectory

// Put the TypeProviderAssemblyAttribute in the runtime DLL, pointing to the design-time DLL
[<assembly: CompilerServices.TypeProviderAssembly("EvgTsvDotMe.PagesProvider.DesignTime.dll")>]
do ()