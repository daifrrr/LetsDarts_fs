namespace Client.Components

open Client.Types
open Client.Events
open Feliz
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
                        Html.div [
                            prop.className "col ld-input-sets"
                            prop.children [
                                Html.div [
                                    Html.select [
                                        prop.className "ld-select"
                                        prop.disabled true
                                        prop.onChange (ChangeCountOfLegs >> dispatch)
                                        prop.children [
                                            [1 .. 10] |> List.map Html.option |> Fable.React.Helpers.ofList
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "col ld-input-legs"
                            prop.children[
                                Html.select [
                                    prop.className "ld-select"
                                    prop.value model.Game.Legs
                                    prop.onChange (ChangeCountOfLegs >> dispatch)
                                    prop.children [
                                        [1 .. 10] |> List.map Html.option |> Fable.React.Helpers.ofList
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "outer-player-input-group-create"
                    prop.children [
                        Html.div [
                            prop.className "player-input-group-create"
                            prop.children [
                                model.Game.Players
                                |> List.mapi (fun i p ->
                                    Html.div [
                                        prop.className "player-input"
                                        prop.children [
                                            Html.input [
                                                prop.type' "text"
                                                prop.className "ld-input ld-player-name-input"
                                                prop.placeholder p.Name
                                                prop.custom ("index", i)
                                                prop.onChange (handleInput i)
                                            ]
                                        ]
                                    ])
                                |> Fable.React.Helpers.ofList
                            ]
                        ]
                        Html.div [
                            prop.className "btn-add-player"
                            prop.onClick (fun _ -> AddPlayer |> dispatch)
                            prop.children [
                                Html.span "+"
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "ld-button ld-button-green btn-game-bull-out"
                    prop.children [
                        Html.span "Bull Out"
                    ]
                    prop.onClick (fun _ -> dispatch OrderPlayers)
                ]
            ]
        ]
