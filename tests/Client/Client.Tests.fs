module Client.Tests

open Fable.Mocha

open Shared

let client = testList "Client" [
 testCase "Client Dummy Test" <| fun _ ->
     Expect.equal true true "True is true"
]

let all =
 testList "All"
     [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
         Shared.Tests.shared
#endif
         client
     ]

[<EntryPoint>]
let main _ = Mocha.runTests all
