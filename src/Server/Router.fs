module Router

open LetsDartsCore
open Shared

type DartsGameHistory() =
    let history = ResizeArray<_>()

    member _.GetInfo() : unit =
        for g in history do
            printfn $"%A{g}"

    member _.GetGames() = List.ofSeq history

    member _.GetCurrentGame() : Game option =
        match List.ofSeq history with
        | [] -> None
        | g -> Some(g |> List.head)

    member _.AddGame(game: Game) = history.Insert(0, game)

    member _.GetOneBeforeLastRemoveLastGame() : Game option =
        let OneBeforeLast = List.ofSeq history |> List.tryItem 1

        history.RemoveAt(0)
        OneBeforeLast

    member _.ClearGameHistory() = history.Clear()

let DartsGameHistory = DartsGameHistory()

let gameApi =
    { sortPlayers =
        fun game ->
            async {
                DartsGameHistory.AddGame game
                printfn $"%A{DartsGameHistory.GetInfo()}"
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
        fun str ->
            async {
                let nextStep, newGame =
                    Game.calcNewGame str (DartsGameHistory.GetCurrentGame().Value)

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
                let oldGame =
                    match DartsGameHistory.GetOneBeforeLastRemoveLastGame() with
                    | Some g -> g
                    | None ->
                        match DartsGameHistory.GetCurrentGame() with
                        | Some g -> g
                        | None -> Game.Default

                return Run, oldGame
            } }
