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
            prop.children [
                Bulma.columns [
                    Bulma.column [
                        Bulma.label "Mode"
                        Bulma.select [
                            prop.value (string model.Game.Mode)
                            text.hasTextCentered
                            prop.children [
                                Html.option 301
                                Html.option 501
                            ]
                            prop.onChange (ChangeMode >> dispatch)
                        ]
                    ]
                    Bulma.column [
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
                Bulma.columns [
                    Bulma.column [
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
                    Bulma.column [
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
                Bulma.columns [
                    Bulma.column [ Bulma.label "Players" ]
                    Bulma.column [
                        Bulma.button.a [
                            color.isInfo
                            prop.text "+"
                            prop.onClick addPlayer
                        ]
                    ]
                ]
                model.Game.Players
                |> List.mapi (fun i p ->
                    Bulma.input.text [
                        text.hasTextCentered
                        prop.placeholder p.Name
                        prop.custom ("index", i)
                        prop.onChange (handleInput i)
                    ])
                |> Fable.React.Helpers.ofList
                Bulma.button.a [
                    color.isInfo
                    prop.text "Start"
                    prop.onClick (fun _ -> dispatch SubmitGameSettings)
                ]
            ]
        ]
