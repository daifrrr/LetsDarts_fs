namespace Client.Components

open Client.Types
open Client.Events
open Feliz
open Feliz.Bulma
open Shared

[<AutoOpen>]
module Form =
    let createForm (model: Model) (dispatch: Msg -> unit) =
        Bulma.container [
            prop.className "frm"
            prop.children [
                Bulma.columns [
                    prop.className "frm-row1"
                    prop.children [
                        Bulma.column [
                            prop.className "frm-mode"
                            prop.children [
                                Bulma.label "Mode"
                                Bulma.select [
                                    prop.value (model.Game.Mode |> string)
                                    text.hasTextCentered
                                    prop.children [
                                        Html.option 301
                                        Html.option 501
                                    ]
                                    prop.onChange (ChangeMode >> dispatch)
                                ]
                            ]
                        ]
                        Bulma.column [
                            prop.className "frm-mode"
                            prop.children [
                                Bulma.label "First To Legs"
                                Bulma.select [
                                    prop.value model.Game.Legs
                                    text.hasTextCentered
                                    prop.children [
                                        Html.option 1
                                        Html.option 3
                                        Html.option 5
                                        Html.option 7
                                    ]
                                    prop.onChange (ChangeCountOfLegs >> dispatch)
                                ]
                            ]
                        ]
                    ]
                ]
                Bulma.columns [
                    prop.className "frm-row2"
                    prop.children [
                        Bulma.column [
                            prop.className "frm-dbl-in"
                            prop.children [
                                Bulma.label [
                                    Bulma.input.checkbox [
                                        prop.onCheckedChange (SwitchDoubleIn >> dispatch)
                                        prop.isChecked model.Game.DoubleIn
                                    ]
                                    Bulma.text.span [
                                        prop.style [ style.marginLeft 8 ]
                                        prop.text "Double In"
                                    ]
                                ]
                            ]
                        ]
                        Bulma.column [
                            prop.className "frm-dbl-in"
                            prop.children [
                                Bulma.label [
                                    Bulma.input.checkbox [
                                        prop.onCheckedChange (SwitchDoubleOut >> dispatch)
                                        prop.isChecked model.Game.DoubleOut
                                    ]
                                    Bulma.text.span [
                                        prop.style [ style.marginLeft 8 ]
                                        prop.text "Double Out"
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Bulma.label [
                    prop.className "frm-players"
                    prop.children [
                        Bulma.text.span [
                            prop.style [ style.marginLeft 8 ]
                            prop.text "Players"
                        ]
                        model.Game.Players
                        |> List.mapi (fun i p ->
                            Bulma.input.text [
                                prop.className "frm-player"
                                prop.placeholder p.Name
                                prop.custom ("index", i)
                                prop.onChange (handleInput i)
                            ])
                        |> Fable.React.Helpers.ofList
                    ]
                ]
                Bulma.button.span [
                    prop.className "btn-player-plus"
                    prop.onClick addPlayer
                    prop.text "+"
                ]
                Bulma.button.span [
                    prop.className "btn-game-start"
                    prop.text "Start"
                    prop.onClick (fun _ -> dispatch OrderPlayers)
                ]
            ]
        ]
