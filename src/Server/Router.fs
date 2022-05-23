module Router

open LetsDartsCore
open Shared

type DartsGameHistory() =
    let history = ResizeArray<_>()

    member _.GetGames() = List.ofSeq history

    member _.GetCurrentGame() : Game option =
        match history |> Seq.tryHead  with
        | Some g -> Some g
        | None -> None

    member _.GetBeforeCurrentGame() : Game option =
        history.RemoveAt(0)
        match history |> Seq.isEmpty |> not with
        | true -> history |> Seq.tryHead
        | _ -> None

    member _.AddGame(game: Game) = history.Insert(0, game)
    member _.ClearGameHistory() = history.Clear()

let DartsGameHistory = DartsGameHistory()

let gameApi =
    { sortPlayers =
        fun game ->
            async {
                DartsGameHistory.AddGame game
                return Order, game
            }
      initGame =
        fun game ->
            async {
                DartsGameHistory.ClearGameHistory()
                DartsGameHistory.AddGame game
                return Run, game
            }
      sendThrow =
        fun shot ->
            async {
                let nextStep, newGame =
                    Game.calcNewGame shot (DartsGameHistory.GetCurrentGame().Value)

                DartsGameHistory.AddGame(newGame)

                match nextStep with
                | GameOn -> return Run, newGame
                | LegOver -> return Show, newGame
                | GameOver -> return End, newGame
                | _ -> return Run, newGame
            }
      undo =
        fun _ ->
            async {
                match DartsGameHistory.GetBeforeCurrentGame() with
                | Some g -> return Run, Some g
                | None -> return Create, None
            }
        }
