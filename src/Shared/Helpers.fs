module Shared.Helpers

module Logger =
    open System

    let log (info: string, f) : Async<unit> =
        async {
            Console.ForegroundColor <- ConsoleColor.Red
            printfn $"{info}"
            f
            Console.ResetColor()
        }
