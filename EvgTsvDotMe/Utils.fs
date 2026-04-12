module EvgTsvDotMe.Utils

open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading.Tasks
open Oxpecker

let inline writeHtml view ctx =
    htmlView (view ctx) ctx
    
let inline memo<'a, 'b> (cmp: IEqualityComparer<'a>) f =
    let cache = ConcurrentDictionary<'a, Lazy<'b>>(cmp)
    fun arg -> cache.GetOrAdd(arg, fun a -> Lazy<'b>(fun () -> f a)).Value
    
let inline memoAsync (cmp: IEqualityComparer<'a>) (f: 'a -> Async<'b>) =
    let cache = ConcurrentDictionary<'a, Task<'b>>(cmp)
    fun (arg: 'a) -> async {
        let task = cache.GetOrAdd(arg, fun a -> f a |> Async.StartAsTask)
        return! Async.AwaitTask task
    }
    
let inline swap f a b = f b a
