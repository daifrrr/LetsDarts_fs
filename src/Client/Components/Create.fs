namespace Client.Components

open Client.Types
open Client.Events
open Feliz
open Feliz.Bulma
open Shared

[<RequireQualifiedAccess>]
module Create =
    let Form (model: Model) (dispatch: Msg -> unit) =
        Html.div [
            prop.className "container-fluid create-layer"
            prop.children [
                Html.div [
                    prop.className "row"
                    prop.children [
                        Html.div [
                            prop.className "col-4"
                            prop.children [
                                Html.label [
                                    prop.text "Score"
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "col-4"
                            prop.children [
                                Html.label [
                                    prop.text "Sets"
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "col-4"
                            prop.children [
                                Html.label [
                                    prop.text "Legs"
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "row"
                    prop.children [
                        Html.div [
                            prop.className "col-4"
                            prop.children [
                                Html.div [
                                    column.is6
                                    prop.className "ld-input-score"
                                    prop.children [
                                        Html.select [
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
                    ]
                ]
                Html.div [
                    prop.className "col-4"
                    prop.children [
                        Html.div [
                            prop.className "ld-input-sets"
                            prop.children [
                                Html.select [
                                    prop.onChange (ChangeCountOfLegs >> dispatch)
                                    prop.children [
                                        Html.option 1
                                        Html.option 3
                                        Html.option 5
                                        Html.option 7
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "col-4"
                    prop.children [
                        Html.div [
                            prop.className "ld-input-legs"
                            prop.children [
                                Html.select [
                                    prop.value model.Game.Legs
                                    prop.onChange (ChangeCountOfLegs >> dispatch)
                                    prop.children [
                                        Html.option 1
                                        Html.option 3
                                        Html.option 5
                                        Html.option 7
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "frm-players"
                    prop.children [
                        model.Game.Players
                        |> List.mapi (fun i p ->
                            Html.div [
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
                        Html.button [
                            prop.className "btn-player-plus"
                            prop.onClick addPlayer
                            prop.text "+"
                        ]
                        Html.div [
                            prop.children [
                                Html.button [
                                    prop.className "btn-game-start"
                                    prop.text "Start"
                                    prop.onClick (fun _ -> dispatch OrderPlayers)
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
