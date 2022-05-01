module Shared.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open Shared

 let shared = testList "Shared" [
     testCase "Shared Dummy Test" <| fun _ ->
         let expected = true
         let actual = true
         Expect.equal actual expected "Should be true"
 ]