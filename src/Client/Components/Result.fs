namespace Client.Components

open Client.Types
open Feliz
open Feliz.Bulma
open Shared

module Result =
    let Show (model: Model) (dispatch: Msg -> unit) =
        Bulma.container [
            prop.children [
                Bulma.button.a [
                    match model.Game |> Game.isFinished with
                    | true ->
                        prop.text "New Game"
                        prop.onClick (fun _ -> dispatch EndGame)
                    | _ ->
                        prop.text "Close"
                        prop.onClick (fun _ -> dispatch CloseShowResults)
                ]
            ]
        ]
