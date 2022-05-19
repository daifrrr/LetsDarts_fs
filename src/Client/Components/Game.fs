namespace Client.Components

open Client.Types
open Feliz
open Feliz.Bulma
open Shared

module internal Players =
    let protectedHTMLSpace =
        ({ Fable.React.Props.__html = "&nbsp;" }
         |> Fable.React.Props.DangerouslySetInnerHTML)

    let currentRecord =
        ref (List.replicate 3 "")

    let filledList p =
        match (Player.getCurrentLeg p).Records with
        | [] -> [ "-"; "-"; "-" ]
        | r ->
            match r.Length % 3 with
            | 0 ->
                r
                |> List.take 3
                |> List.map (fun s -> s |> ToString)
            | c ->
                (List.replicate ((-) 3 c) "-"
                 @ (r
                    |> List.take c
                    |> List.map (fun s -> s |> ToString)))
                |> List.rev



    let previousRecord (p: Player) =
        let r =
            (p |> Player.getCurrentLeg).Records
            |> List.chunkBySize 3
            |> List.head

        (r |> Leg.calcCurrentScore |> string, r |> List.map (fun s -> s |> ToString))

    let lastRoundInfo (s, r) =
        let shotList =
            match r with
            | [] -> [ "0"; "0"; "0" ]
            | _ -> r


        Bulma.container [
            text.hasTextCentered
            prop.className "ply-previous"
            prop.children [
                Html.h3 [
                    prop.className "ply-prevscore"
                    prop.text $"%s{s}"
                ]
                Bulma.columns [
                    columns.isCentered
                    prop.className "ply-prevshots"
                    prop.children [
                        shotList
                        |> List.map (fun shot ->
                            Bulma.column [
                                column.is3
                                prop.className "ply-prevshot"
                                prop.children [ Html.span $"%s{shot}" ]
                            ])
                        |> Fable.React.Helpers.ofList
                    ]
                ]
            ]
        ]


    let legsWon (p: Player) (mode: int, legs: int) =
        let wonLegs =
            (p, mode) ||> Player.getLegsWon

        Bulma.columns [
            prop.className "ply-legs"
            prop.children [
                [ 1..legs ]
                |> List.map (fun i ->
                    Bulma.column [
                        match i <= wonLegs with
                        | true -> prop.className "filled"
                        | _ -> prop.className "empty"
                    ])
                |> Fable.React.Helpers.ofList
            ]
        ]

    let renderPlayers g =
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
                                match i = currentPlayerIndex with
                                | true -> prop.className "ply ply-active"
                                | _ -> prop.className "ply ply-not-active"
                                prop.children [
                                    Html.h2 [
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
                                    Html.h3 [
                                        prop.className "ply-avg"
                                        prop.text $"%.2f{p |> Player.getAverage}"
                                    ]
                                    legsWon p (g.Mode, g.Legs)
                                ]
                            ])
                        |> Fable.React.Helpers.ofList
                    ]
                ]
                Bulma.columns [
                    prop.className "record-items"
                    prop.children [
                        g
                        |> Game.getCurrentPlayer
                        |> filledList
                        |> List.map (fun s ->
                            Bulma.column [
                                prop.className "record-item"
                                match s with
                                | "-" -> prop.dangerouslySetInnerHTML "&nbsp;"
                                | _ -> prop.text s
                            ])
                        |> Fable.React.Helpers.ofList
                    ]
                ]
            ]
        ]

[<RequireQualifiedAccess>]
module Play =
    let Game (model: Model) (dispatch: Msg -> unit) =
        Bulma.container [
            Bulma.columns [
                prop.children [
                    Bulma.column [
                        column.is6
                        prop.children [
                            Players.renderPlayers model.Game
                            Bulma.button.a [
                                prop.className "btn-undo"
                                prop.text "Undo Last Dart"
                                prop.onClick (fun _ -> dispatch UndoLastAction)
                            ]
                        ]
                    ]
                    Bulma.column [
                        column.is6
                        prop.children [ Dartboard dispatch ]
                    ]
                ]
            ]
        ]
