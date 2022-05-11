namespace Client.Components

open Feliz
open Feliz.Bulma
open Shared

module Players =

    let previousScore (p:Player) =
        let previousRecord = (p |> Player.getCurrentLeg).Records
                             |> List.chunkBySize 3
                             |> List.head
        let previousScore = previousRecord |> Leg.calcCurrentScore
        Bulma.container [
            prop.className "dev"
            prop.children [
                Bulma.content [
                    Html.h2 $"%d{previousScore}"
                ]
                Bulma.columns [
                    previousRecord |> List.map (fun s ->
                        Bulma.column [
                            column.isOneThird
                            prop.children [
                                Bulma.content [
                                    Html.h6 $"%s{s |> ToString}"
                                ]
                            ]
                        ]
                    ) |> Fable.React.Helpers.ofList
                ]
            ]
        ]
    let renderPlayers (g: Game) =
        let p = g |> Game.getPlayers

        let currentPlayerIndex =
            g |> Game.getCurrentPlayerIndex

        Bulma.container [
            prop.children [
                Bulma.columns [
                    prop.className "players"
                    prop.children [
                        p
                        |> List.mapi (fun i p ->
                            Bulma.column [
                                prop.className "player"
                                prop.children [
                                    Bulma.content [
                                        match i = currentPlayerIndex with
                                        | true -> prop.className "active"
                                        | _ -> prop.className "not-active"
                                        text.hasTextCentered
                                        prop.children [
                                            Html.h3 [ prop.text $"{p.Name}" ]
                                            previousScore p
                                            Html.h1 [ prop.text $"{(p |> Player.getCurrentLeg).CurrentScore}" ]
                                            Html.h2 [ prop.text $"26,1" ]

                                            ]
                                    ]
                                ]
                            ])
                        |> Fable.React.Helpers.ofList
                    ]
                ]
            ]
        ]
