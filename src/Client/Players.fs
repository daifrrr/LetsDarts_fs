namespace Client.Components

open Feliz
open Feliz.Bulma
open Shared

module Players =

    let previousRecord (p: Player) =
        let r =
            (p |> Player.getCurrentLeg).Records
            |> List.chunkBySize 3
            |> List.head

        (r |> Leg.calcCurrentScore |> string, r |> List.map (fun s -> s |> ToString))

    let lastRoundInfo (s, r) =
        let shotList =
            match r with
            | [] -> [ "-"; "-"; "-" ]
            | _ -> r

        Bulma.container [
            prop.children [
                Bulma.content [
                    Html.h3 $"%s{s}"
                    Bulma.columns [
                        columns.isCentered
                        prop.children [
                            shotList
                            |> List.map (fun shot ->
                                Bulma.column [
                                    column.is3
                                    prop.children [ Html.span $"%s{shot}" ]
                                ])
                            |> Fable.React.Helpers.ofList
                        ]
                    ]
                ]
            ]
        ]

    let legsWon g =
        Bulma.container [
            Bulma.columns [
                prop.className "legs"
                prop.children [
                    [ 1 .. g.Legs ]
                    |> List.map (fun _ -> Bulma.column [ prop.className "emptyLeg" ])
                    |> Fable.React.Helpers.ofList
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
                    prop.className "plys"
                    prop.children [
                        p
                        |> List.mapi (fun i p ->
                            Bulma.column [
                                text.hasTextCentered
                                prop.children [
                                    Bulma.content [
                                        match i = currentPlayerIndex with
                                        | true -> prop.className "ply ply-active"
                                        | _ -> prop.className "ply ply-not-active"
                                        prop.children [
                                            Html.h3 [
                                                prop.className "ply-name"
                                                prop.text $"{p.Name}"
                                            ]
                                            match i = currentPlayerIndex with
                                            | false -> p |> previousRecord |> lastRoundInfo
                                            | _ -> ("-", [ "-"; "-"; "-" ]) |> lastRoundInfo
                                            Html.h1 [
                                                prop.className "ply-score"
                                                prop.text $"{((-) g.Mode (p |> Player.getCurrentLeg).CurrentScore)}"
                                            ]
                                            Html.h2 [
                                                prop.className "ply-avg"
                                                prop.text $"26,1"
                                            ]
                                            legsWon g
                                        ]
                                    ]
                                ]
                            ])
                        |> Fable.React.Helpers.ofList
                    ]
                ]
            ]
        ]
