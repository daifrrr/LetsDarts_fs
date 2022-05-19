namespace Client.Components

open Client.Types
open Client.Events
open Feliz
open Feliz.Bulma
open Shared

[<RequireQualifiedAccess>]
module Create =
    let Form (model: Model) (dispatch: Msg -> unit) =
        Bulma.container [ Bulma.columns [ prop.className "frm-row1"
                                          prop.children [ Bulma.column [ prop.className "frm-mode"
                                                                         prop.children [ Bulma.label "Mode"
                                                                                         Bulma.select [ prop.value (
                                                                                                            model.Game.Mode
                                                                                                            |> string
                                                                                                        )
                                                                                                        prop.onChange (
                                                                                                            ChangeMode
                                                                                                            >> dispatch
                                                                                                        )
                                                                                                        prop.children [ Html.option
                                                                                                                            301
                                                                                                                        Html.option
                                                                                                                            501 ] ] ] ]
                                                          Bulma.column [ prop.className "frm-mode"
                                                                         prop.children [ Bulma.label "First To Legs"
                                                                                         Bulma.select [ prop.value
                                                                                                            model.Game.Legs
                                                                                                        prop.onChange (
                                                                                                            ChangeCountOfLegs
                                                                                                            >> dispatch
                                                                                                        )
                                                                                                        prop.children [ Html.option
                                                                                                                            1
                                                                                                                        Html.option
                                                                                                                            3
                                                                                                                        Html.option
                                                                                                                            5
                                                                                                                        Html.option
                                                                                                                            7 ] ] ] ] ] ]
                          Bulma.columns [ prop.className "frm-row2"
                                          prop.children [ Bulma.column [ prop.className "frm-dbl-in"
                                                                         prop.children [ Bulma.label [ Bulma.input.checkbox [ prop
                                                                                                                                  .onCheckedChange (
                                                                                                                                      SwitchDoubleIn
                                                                                                                                      >> dispatch
                                                                                                                                  )
                                                                                                                              prop.isChecked
                                                                                                                                  model.Game.DoubleIn ]
                                                                                                       Bulma.text.span
                                                                                                           "Double In" ] ] ]
                                                          Bulma.column [ prop.className "frm-dbl-out"
                                                                         prop.children [ Bulma.label [ Bulma.input.checkbox [ prop
                                                                                                                                  .onCheckedChange (
                                                                                                                                      SwitchDoubleOut
                                                                                                                                      >> dispatch
                                                                                                                                  )
                                                                                                                              prop.isChecked
                                                                                                                                  model.Game.DoubleOut ]
                                                                                                       Bulma.text.span
                                                                                                           "Double Out" ] ] ] ] ]
                          Bulma.columns [ prop.className "frm-players"
                                          prop.children [ Bulma.label "Players"
                                                          model.Game.Players
                                                          |> List.mapi (fun i p ->
                                                              Bulma.input.text [ prop.className "frm-player"
                                                                                 prop.placeholder p.Name
                                                                                 prop.custom ("index", i)
                                                                                 prop.onChange (handleInput i) ])
                                                          |> Fable.React.Helpers.ofList
                                                          Bulma.button.span [ prop.className "btn-player-plus"
                                                                              prop.onClick addPlayer
                                                                              prop.text "+" ] ] ]
                          Bulma.columns [ prop.children [ Bulma.button.span [ prop.className "btn-game-start"
                                                                              prop.text "Start"
                                                                              prop.onClick (fun _ ->
                                                                                  dispatch OrderPlayers) ] ] ] ]