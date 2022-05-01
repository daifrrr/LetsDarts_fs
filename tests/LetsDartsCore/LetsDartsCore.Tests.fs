module LetsDartsCore.Tests
open Expecto
open Shared

let letsdartscore = testList "LetsDartsCore" [
    testCase "LetsDartsCore Dummy Test" <| fun _ ->
        Expect.equal true true "True is true"
]


let all = testList "All" [
    Shared.Tests.shared
    letsdartscore
]


[<EntryPoint>]
let main _ = runTestsWithCLIArgs [] [||] all

