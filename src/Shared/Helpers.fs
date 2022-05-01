module Shared.Helpers

open System

module Logger =
    let log (info: string, f) : Async<unit> =
        async {
            printfn $"{info}"
            f |> ignore
        }

