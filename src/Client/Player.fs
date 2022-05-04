namespace Client.Components

open Feliz
open Feliz.Bulma
open Shared

module Player =
    let renderPlayers (p: Player list) =
            let currentPlayer = 0
            let filledList =
                p |>
                List.map (fun p ->
                    match (Player.getCurrentLeg p).Records with
                    | [] -> [ " "; " "; " " ]
                    | r ->
                        match r.Length % 3 with
                        | 0 ->
                            r
                            |> List.take 3
                            |> List.map (fun s -> $"%s{s |> Shot.ToString}")
                        | c ->
                            (List.replicate ((-) 3 c) " "
                             @ (r
                                |> List.take c
                                |> List.map (fun s -> $"%s{s |> Shot.ToString}")))
                            |> List.rev
                )
            p
            |> List.mapi (fun i p ->
                Bulma.container [
                    match i = currentPlayer with
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
