namespace Client.Components

open Client.Types
open Feliz
open Shared

module Result =
    let Show (model: Model) (dispatch: Msg -> unit) =
        Html.div [
            prop.children [
                Html.button [
                    match model.Game |> Game.isFinished with
                    | true ->
                        prop.text "New Game"
                        prop.onClick (fun _ -> FinishRound "Game End" |> dispatch)
                    | _ ->
                        prop.text "Close"
                        prop.onClick (fun _ -> FinishRound "Next Round" |> dispatch)
                ]
            ]
        ]
