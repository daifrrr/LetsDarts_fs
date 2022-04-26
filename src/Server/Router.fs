module Router

open LetsDartsCore
open Shared

type DartsGameHistory() =
    let history = ResizeArray<_>()
    member _.GetGames() = List.ofSeq history
    member _.GetLast() : Game option =
        match List.ofSeq history with
        | [] -> None
        | g -> Some(g |> List.last)

    member _.GetLastLast() : Game option =
        match List.ofSeq history with
        | [] -> None
        | g -> g |> List.tail |> List.rev |> List.tryItem(1)

    member _.RemoveLast(game: Game): unit =
        let start = history.IndexOf(game)
        history.RemoveRange(start, history.Count)
    member _.AddGame(game: Game) = history.Add game
    member _.ClearHistory() = history.Clear()

let DartsGameHistory = DartsGameHistory()

// let getCurrentPlayer (game:Game): Player =
//     game.Players |> List.tryFind (fun p -> p.Legs.Head.Records.Length % 3 <> 0)

// let calcNewGame (throw:string) =
//     let currentPlayer = getCurrentPlayer DartsGame.GetLast
//     currentPlayer

let gameApi =
    { initGame =
        fun game ->
            async {
                DartsGameHistory.ClearHistory()
                DartsGameHistory.AddGame game
                return (Running, game)
            }
      sendThrow = fun str -> async {
          let newGame = Game.calcNewGame str (DartsGameHistory.GetLast().Value)
          DartsGameHistory.AddGame(newGame)
          return newGame
      }
      undo = fun _ -> async {
        let oldGame = DartsGameHistory.GetLastLast().Value
        return oldGame
      }
    }
