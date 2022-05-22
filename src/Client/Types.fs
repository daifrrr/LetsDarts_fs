[<AutoOpen>]
module Client.Types

open Shared

type Model = { State: AppState; Game: Game }

type Msg =
    | OrderPlayers
    | PlayersOrdered of AppState * Game
    | SubmitGameSettings
    | GameSettingsSubmitted of AppState * Game
    | SendShot of string
    | ShotReceived of AppState * Game
    | UndoLastAction
    | LastActionUndone of AppState * Game
    | FinishRound of string
    | SwitchDoubleOut of bool
    | SwitchDoubleIn of bool
    | AddPlayer of Player
    | MovePlayerPosition of Player list
    | ChangePlayername of int * string
    | ChangeMode of string
    | ChangeCountOfLegs of string
