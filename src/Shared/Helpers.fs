module Shared.Helpers

open System

module Logger =
    let log (info: string, f) : Async<unit> =
        async {
            printfn $"{info}"
            f |> ignore
        }

let f = [
            [
                { CurrentScore = 121; Records = [('s', 0); ('s', 0); ('s', 0); ('t', 20); ('t', 20); ('t', 20)] }
                { CurrentScore = 0; Records = [('d', 17); ('t', 9); ('t', 20); ('t', 20); ('t', 20); ('t', 20)] }
            ]
            [
                { CurrentScore = 0; Records = [('d', 17); ('t', 20); ('t', 19); ('d', 25); ('d', 25); ('d', 25)] }
                { CurrentScore = 121; Records = [('t', 20); ('t', 20); ('t', 20)] }
            ]
        ]

