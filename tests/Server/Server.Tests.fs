module Server.Tests

open Expecto

open Shared
open Server

let server = testList "Server" [
     testCase "Server Dummy Test" <| fun _ ->
         let expected = true
         let actual = true
         Expect.equal actual expected "Should be true"
 ]

let all = testList "All" [ Shared.Tests.shared; server ]

[<EntryPoint>]
let main _ = runTestsWithCLIArgs [] [||] all