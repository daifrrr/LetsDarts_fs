[<AutoOpen>]
module Client.Types

open Shared

type Model = { State: State; Game: Game }

type Msg =
    | SubmitGameSettings
    | ChangeGameState of State * Game
    | GetThrow of string
    | GotThrow of State * Game
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

