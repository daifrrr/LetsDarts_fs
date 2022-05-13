[<AutoOpen>]
module Client.Types

open Shared

type Model = { State: GameState; Game: Game }

type Msg =
    | SubmitGameSettings
    | ChangeGameState of GameState * Game
    | GetThrow of string
    | GotThrow of GameState * Game
    | CloseShowResults
    | EndGame
    | SwitchDoubleOut of bool
    | SwitchDoubleIn of bool
    | AddPlayer of Player
    | ChangePlayername of int * string
    | ChangeMode of string
    | ChangeCountOfLegs of string
    | Undo
    | UndoDone of Game

