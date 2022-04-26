module Router

open Saturn
open Fable.Remoting.Giraffe
open Fable.Remoting.Server

open Shared

type DartsGame() =
    let history = ResizeArray<_>()

    member _.GetGames() = List.ofSeq history

    member _.GetLast() = List.ofSeq history |> List.tail

    member _.AddGame(game: Game) = history.Add game

let DartsGame = DartsGame()

// let getCurrentPlayer (game:Game): Player =
//     game.Players |> List.tryFind (fun p -> p.Legs.Head.Records.Length % 3 <> 0)

// let calcNewGame (throw:string) =
//     let currentPlayer = getCurrentPlayer DartsGame.GetLast
//     currentPlayer

let gameApi =
    { initGame =
        fun game ->
            async {
                DartsGame.AddGame game
                return (Running, game)
            }
      sendThrow = fun str -> async { return Game.Default } }
