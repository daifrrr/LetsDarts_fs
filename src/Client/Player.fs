namespace Client.Components

open Feliz
open Feliz.Bulma
open Shared

module Player =
    let renderPlayers (g: Game) =
            let p = g |> Game.getPlayers
            let currentPlayerIndex = g |> Game.getCurrentPlayerIndex
            let filledList =
                p |>
                List.map (fun p ->
                    (p |> Player.getCurrentLeg).Records |> List.rev |> List.chunkBySize 3
                )
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
                    ]
                ]) |> Fable.React.Helpers.ofList
