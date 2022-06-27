namespace Client.Components

open Client.Types
open Feliz
open Shared

module Sort =
    let Form (model: Model) (dispatch: Msg -> unit) =
        let rec moveDownAt (list: Player list) (index: int) : Player list =
            match list, index with
            | _, -1 -> list
            | h1 :: h2 :: t, 0 -> h2 :: h1 :: t
            | h :: t, index -> h :: moveDownAt t (index - 1)
            | [], _ -> list

        let moveUpAt (list: Player list) (index: int) : Player list = moveDownAt list (index - 1)

        let up =
            model.Game |> Game.getPlayers |> moveUpAt

        let down =
            model.Game |> Game.getPlayers |> moveDownAt

        Html.div [
            prop.className "container-fluid row g-0 sort-layer"
            prop.children [
                Html.div [
                    prop.className "player-list"
                    prop.children [
                        model.Game
                        |> Game.getPlayers
                        |> List.mapi (fun i p ->
                            Html.div [
                                prop.id $"player-{i}"
                                prop.className "player ld-input ld-player-name-input"
                                prop.children [
                                    match i = 0 |> not with
                                    | true -> Html.span [
                                                prop.className "sort-icon left"
                                                prop.text "\u25b2"
                                                prop.ariaHidden true
                                                prop.onClick (fun _ -> MovePlayerPosition(i |> down) |>  dispatch)
                                            ]
                                    | _ -> Html.none
                                    Html.span [
                                        prop.className "player-name"
                                        prop.text p.Name
                                    ]
                                    match i = ((-) (model.Game
                                                   |> Game.getPlayers
                                                   |> List.length) 1) |> not with
                                    | true -> Html.span [
                                                prop.className "sort-icon right"
                                                prop.text "\u25bc"
                                                prop.ariaHidden true
                                                prop.onClick (fun _ -> MovePlayerPosition(i |> down) |>  dispatch)
                                            ]
                                    | _ -> Html.none
                                ]
                            ])
                        |> Fable.React.Helpers.ofList
                    ]
                ]
                Html.div [
                    prop.className "ld-button ld-button-green btn-game-start"
                    prop.text "Start"
                    prop.onClick (fun _ -> dispatch SubmitGameSettings)
                ]
            ]
        ]
