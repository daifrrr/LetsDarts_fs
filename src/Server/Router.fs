module Router

open System
open LetsDartsCore
open Shared

type DartsGameHistory() =
    let history = ResizeArray<_>()

    member _.GetInfo(): unit =
        for g in history do
            printfn "%A" g
    member _.GetGames() = List.ofSeq history
    member _.GetLast() : Game option =
        match List.ofSeq history with
        | [] -> None
        | g -> Some(g |> List.last)

    member _.GetLastLast() : Game option =
        match List.ofSeq history with
        | [] -> None
        | g -> g |> List.rev |> List.tryItem(2)

    member _.GetItemIndex(game: Game): int =
        try
            history.IndexOf(game)
        with
            | ex -> printfn $"%s{ex.ToString()}"; 0

    member _.Remove(start: int): unit =
        try
            history.RemoveRange(start, history.Count - 1)
        with
           | ex -> printfn $"%s{ex.ToString()}"

    member _.AddGame(game: Game) = history.Add game
    member _.ClearHistory() = history.Clear()

let DartsGameHistory = DartsGameHistory()

// let getCurrentPlayer (game:Game): Player =
//     game.Players |> List.tryFind (fun p -> p.Legs.Head.Records.Length % 3 <> 0)

// let calcNewGame (throw:string) =
//     let currentPlayer = getCurrentPlayer DartsGame.GetLast
//     currentPlayer
let log (from:string): Async<unit> = async {
//            Console.Clear()
//            Console.ForegroundColor <- ConsoleColor.Red
            printfn $"{from}"
            DartsGameHistory.GetInfo()
            //Console.ResetColor()
    }

let gameApi =
    { initGame =
        fun game ->
            async {
                DartsGameHistory.ClearHistory()
                DartsGameHistory.AddGame game
                log "INIT" |> Async.Start
                return (Running, game)
            }
      sendThrow = fun str -> async {
          let newGame = Game.calcNewGame str (DartsGameHistory.GetLast().Value)
          DartsGameHistory.AddGame(newGame)
          log "SEND THROW" |> Async.Start
          return newGame
      }
      undo = fun _ -> async {
        let oldGame = match DartsGameHistory.GetLastLast() with
                      | Some i -> let idx = DartsGameHistory.GetItemIndex(i)
                                  printfn $"IDX******: {idx}"
                                  DartsGameHistory.Remove(idx)
                                  DartsGameHistory.GetLast().Value
                      | None -> DartsGameHistory.Remove(1)
                                DartsGameHistory.GetLast().Value
        log "UNDO" |> Async.Start
        return oldGame
      }
    }
