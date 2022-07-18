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
            prop.className "container create-layer"
            prop.children [
                Html.div [
                    prop.className "row game-setup"
                    prop.children [
                        Html.div [
                            prop.className "col-3"
                            prop.children [
                                Html.p "Score"
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
                            prop.className "col-3"
                            prop.children [
                                Html.p "Sets"
                                Html.select [
                                    prop.className "ld-select"
                                    prop.disabled true
                                    prop.onChange (ChangeCountOfLegs >> dispatch)
                                    prop.children [
                                        [ 1..10 ]
                                        |> List.map Html.option
                                        |> Fable.React.Helpers.ofList
                                    ]
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "col-3"
                            prop.children [
                                Html.p "Legs"
                                Html.select [
                                    prop.className "ld-select"
                                    prop.value model.Game.Legs
                                    prop.onChange (ChangeCountOfLegs >> dispatch)
                                    prop.children [
                                        [ 1..10 ]
                                        |> List.map Html.option
                                        |> Fable.React.Helpers.ofList
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "row player-setup"
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
                        Html.div [
                            prop.className "player-add"
                            prop.onClick (fun _ -> AddPlayer |> dispatch)
                            prop.text "+"
                        ]
                    ]
                ]
                Html.div [
                    prop.className "row ld-button green"
                    prop.children [ Html.span "Bull Out" ]
                    prop.onClick (fun _ -> dispatch OrderPlayers)
                ]
            ]
        ]
