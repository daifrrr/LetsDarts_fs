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
            Html.div [
                prop.className "frm-row1"
                prop.children [
                    Html.div [
                        column.is6
                        prop.className "frm-mode"
                        prop.children [
                            Html.label [
                                prop.text "Mode"
                            ]
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
                    Html.div [
                        prop.className "frm-mode"
                        prop.children [
                            Html.label [
                                prop.text "First To Legs"
                            ]
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
                prop.className "frm-row2"
                prop.children [
                    Html.div [
                        prop.className "frm-dbl-in display-none"
                        prop.children [
                            Html.label [
                                Html.input [
                                    prop.type' "checkbox"
                                    prop.onCheckedChange (SwitchDoubleIn >> dispatch)
                                    prop.isChecked model.Game.DoubleIn
                                ]
                                Html.span [
                                    prop.text "Double In"
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "frm-dbl-out display-none"
                        prop.children [
                            Html.label [
                                Html.input [
                                    prop.type' "checkbox"
                                    prop.onCheckedChange (SwitchDoubleOut >> dispatch)
                                    prop.isChecked model.Game.DoubleOut
                                ]
                                Html.span [
                                    prop.text "Double Out"
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
                ]
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
