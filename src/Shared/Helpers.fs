module Shared.Helpers

open System

module Logger =
    let log (info: string, f: Func<Unit>) : Async<unit> =
        async {
            printfn $"{info}"
            f |> ignore
        }
