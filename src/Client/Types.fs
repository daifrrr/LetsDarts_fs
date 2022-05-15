[<AutoOpen>]
module Client.Types

open Shared

type Model = { State: AppState; Game: Game }

type Msg =
    | SubmitGameSettings
    | OrderPlayers
    | ChangeGameState of AppState * Game
    | GetThrow of string
    | GotThrow of AppState * Game
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

