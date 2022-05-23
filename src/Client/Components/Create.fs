namespace Client.Components

open Client.Types
open Client.Events
open Feliz
open Feliz.Bulma
open Shared


// Html.div [prop.className "container-fluid create-layer"prop.children [
// ]]

[<RequireQualifiedAccess>]
module Create =
    let Form (model: Model) (dispatch: Msg -> unit) =
        Html.div [
            prop.className "container-fluid create-layer"
            prop.children [
                Html.div [
                    prop.className "row label-group g-0"
                    prop.children [
                        Html.div [
                            prop.className "col ld-label-score"
                            prop.children [
                                Html.div [
                                    Html.label [
                                        prop.text "Score"
                                    ]
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "col ld-label-sets"
                            prop.children [
                                Html.div [
                                    Html.label [
                                        prop.text "Sets"
                                    ]
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "col ld-label-legs"
                            prop.children [
                                Html.div [
                                    Html.label [
                                        prop.text "Legs"
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "row input-group g-0"
                    prop.children [
                        Html.div [
                            prop.className "col ld-input-score"
                            prop.children [
                                Html.div [
                                    prop.className ""
                                    prop.children [
                                        Html.select [
                                            prop.className "ld-select"
                                            prop.value (model.Game.Mode |> string)
                                            prop.onChange (ChangeMode >> dispatch)
                                            prop.children [
                                                Html.option 301
                                                Html.option 501
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "col ld-input-sets"
                            prop.children [
                                Html.div [
                                    prop.className ""
                                    prop.children [
                                        Html.select [
                                            prop.className "ld-select"
                                            prop.disabled true
                                            prop.onChange (ChangeCountOfLegs >> dispatch)
                                            prop.children [
                                                [1;2;3;4;5;6;7;8;9;10] |> List.map (fun i -> Html.option i) |> Fable.React.Helpers.ofList
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "col ld-input-legs"
                            prop.children[
                                Html.div [
                                    prop.className ""
                                    prop.children [
                                        Html.select [
                                            prop.className "ld-select"
                                            prop.value model.Game.Legs
                                            prop.onChange (ChangeCountOfLegs >> dispatch)
                                            prop.children [
                                                [1;2;3;4;5;6;7;8;9;10] |> List.map (fun i -> Html.option i) |> Fable.React.Helpers.ofList
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "outer-player-input-group"
                    prop.id "outer-player-input-group"
                    prop.children [
                        Html.div [
                            prop.className "player-input-group"
                            prop.id "player-input-group"
                            prop.children [
                                model.Game.Players
                                |> List.mapi (fun i p ->
                                    Html.div [
                                        prop.className "player-input"
                                        prop.children [
                                            Html.div [
                                                prop.className "is-full"
                                                prop.children [
                                                    Html.input [
                                                        prop.type' "text"
                                                        prop.className "ld-input ld-player-name-input"
                                                        prop.placeholder p.Name
                                                        prop.custom ("index", i)
                                                        prop.onChange (handleInput i)
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ])
                                |> Fable.React.Helpers.ofList
                            ]
                        ]
                        Html.div [
                            prop.className "btn-add-player"
                            prop.onClick addPlayer
                            prop.children [
                                Html.i [
                                    prop.className "fa fa-plus"
                                    prop.ariaHidden true
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className ""
                    prop.children [
                        Html.div [
                            prop.className "btn-game-bull-out align-self-end ld-button ld-button-green"
                            prop.id "btn-game-bull-out"
                            prop.text "Bull Out"
                            prop.onClick (fun _ -> dispatch OrderPlayers)
                        ]
                    ]
                ]
            ]
        ]
