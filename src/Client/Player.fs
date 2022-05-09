namespace Client.Components

open Feliz
open Feliz.Bulma
open Shared

module Player =
    let renderPlayers (g: Game) =
        let p = g |> Game.getPlayers

        let currentPlayerIndex =
            g |> Game.getCurrentPlayerIndex

        let filledList =
            p
            |> List.map (fun p ->
                (p |> Player.getCurrentLeg).Records
                |> List.rev
                |> List.chunkBySize 3)
            |> List.head
            |> List.concat


        p
        |> List.mapi (fun i p ->
            Bulma.container [
                match i = currentPlayerIndex with
                | true -> prop.className "player-active"
                | _ -> prop.className "player-inactive"
                prop.children [
                    Bulma.columns [
                        Bulma.column [
                            column.is3
                            prop.children [
                                Bulma.text.span (sprintf "\u2300")
                            ]
                        ]
                        Bulma.column [
                            column.is6
                            text.hasTextCentered
                            prop.children [ Bulma.text.span p.Name ]
                        ]
                        Bulma.column [
                            column.is3
                            prop.children [
                                Bulma.text.span p.Legs.Head.CurrentScore
                            ]
                        ]
                    ]
                    Bulma.columns [
                        filledList
                        |> List.map (fun s ->
                            Bulma.column [
                                column.is4
                                prop.children [
                                    Bulma.tag [
                                        prop.style [
                                            style.width (length.percent 100)
                                            style.marginBottom 5
                                        ]
                                        tag.isLarge
                                        tag.isRounded
                                        color.hasBackgroundGreyLight
                                        prop.text $"%s{s |> Shot.ToString}"
                                    ]
                                ]
                            ])
                        |> Fable.React.Helpers.ofList
                    ]
                ]
            ])
        |> Fable.React.Helpers.ofList
