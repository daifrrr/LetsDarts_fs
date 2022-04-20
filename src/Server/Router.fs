module Router

open Saturn
open Fable.Remoting.Giraffe
open Fable.Remoting.Server

open Shared


module Storage =
    let aBoolString = (true, "Here i am")

let gameApi =
    {
      initGame = fun game -> async {
            printfn "%A" game
            return (Running, game)
          }
      sendThrow = fun str -> async {return Game.Default }
    }