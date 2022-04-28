module Shared.Helpers

module Logger =
    let log (info: string, f) : Async<unit> =
        async {
            printfn $"{info}"
            f
        }
