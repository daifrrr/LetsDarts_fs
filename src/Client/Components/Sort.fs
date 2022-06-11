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
            prop.className "container-fluid sort-layer"
            prop.children [
                Html.div [
                    prop.className "vertical-space-sort"
                ]
                Html.div [
                    prop.className "outer-player-input-group-sort"
                    prop.children [
                        Html.div [
                            prop.className "player-input-group-sort"
                            prop.children [
                                model.Game
                                |> Game.getPlayers
                                |> List.mapi (fun i p ->
                                    Html.div [
                                        Html.div [
                                            prop.className "srt-player"
                                            prop.custom ("index", i)
                                            prop.children [
                                                Html.div [
                                                    prop.className "players"
                                                    prop.children [
                                                        Html.p [
                                                            prop.className "ld-player-label"
                                                            prop.text p.Name
                                                        ]
                                                        Html.p [
                                                            prop.className "fa fa-bars"
                                                            prop.ariaHidden true
                                                            prop.onClick (fun _ -> MovePlayerPosition(i |> down) |>  dispatch)
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ])
                                |> Fable.React.Helpers.ofList
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "ld-button ld-button-green btn-game-start"
                    prop.text "Start"
                    prop.onClick (fun _ -> dispatch SubmitGameSettings)
                ]
            ]
        ]
