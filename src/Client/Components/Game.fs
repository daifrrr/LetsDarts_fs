namespace Client.Components

open Client.Types
open Feliz
open Shared

module internal Players =
    let protectedHTMLSpace =
        ({ Fable.React.Props.__html = "&nbsp;" }
         |> Fable.React.Props.DangerouslySetInnerHTML)

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


        Html.div [
            prop.className "prev-record"
            prop.children [
                Html.div [
                    prop.className "row"
                    prop.children [
                        Html.div [
                            prop.className "col-12"
                            prop.children [
                                Html.div [
                                    prop.className "prev-score"
                                    prop.text $"%s{s}"
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.className "row"
                    prop.children [
                        Html.div [
                            prop.className "col-12"
                            prop.children [
                                shotList
                                |> List.map (fun shot ->
                                    Html.span [
                                        prop.className "prev-darts"
                                        prop.text $"%s{shot}"
                                    ])
                                |> Fable.React.Helpers.ofList
                            ]
                        ]
                    ]
                ]
            ]
        ]


    let legsWon (p: Player) (mode: int, legs: int) =
        let wonLegs =
            (p, mode) ||> Player.getLegsWon

        Html.div [
            prop.className "legs"
            prop.children [
                [ 1..legs ]
                |> List.map (fun i ->
                    Html.div [
                        match i <= wonLegs with
                        | true -> prop.className "filled"
                        | _ -> prop.className "empty"
                    ])
                |> Fable.React.Helpers.ofList
            ]
        ]

    let renderPlayers g =
        let ps = g |> Game.getPlayers

        let currentPlayerIndex =
            g |> Game.getCurrentPlayerIndex

        Html.div [
            prop.className "row player-wrapper flex-nowrap col-12 g-0"
            prop.children [
                ps
                |> List.mapi (fun i p ->
                    Html.div [
                        match i = currentPlayerIndex with
                        | true -> prop.className "col-3 player active"
                        | _ -> prop.className "col-3 player inactive"
                        prop.children [
                            Html.div [
                                prop.className "row"
                                prop.children [
                                    Html.div [
                                        prop.className "col-12"
                                        prop.children [
                                            Html.div [
                                                prop.className "name"
                                                prop.text $"{p.Name}"
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                            match i = currentPlayerIndex with
                            | false -> p |> previousRecord |> lastRoundInfo
                            | _ -> ("–", [ "–"; "–"; "–" ]) |> lastRoundInfo
                            Html.div [
                                prop.className "row"
                                prop.children [
                                    Html.div [
                                        prop.className "col-12"
                                        prop.children [
                                            Html.div [
                                                prop.className "score"
                                                prop.text
                                                    $"{((-) g.Mode (p |> Player.getCurrentLeg).CurrentScore)}"
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                            Html.div [
                                prop.className "row"
                                prop.children [
                                    Html.div [
                                        prop.className "col-12"
                                        prop.children [
                                            Html.div [
                                                prop.className "average"
                                                prop.text $"%.2f{p |> Player.getAverage}"
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                            Html.div [
                                prop.className "row wrapper-sets-legs"
                                prop.children [
                                    legsWon p (g.Mode, g.Legs)
                                ]
                            ]
                        ]
                    ])
                |> Fable.React.Helpers.ofList
            ]
        ]


[<RequireQualifiedAccess>]
module Play =
    let Game (model: Model) (dispatch: Msg -> unit) =

        Html.div [
            prop.className "container-fluid row g-0 game-layer"
            prop.children [
                Html.div [
                    prop.className "col-6 player-stats-container"
                    prop.children [
                        Players.renderPlayers model.Game
                        Html.div [
                            prop.className "row player-record"
                            prop.children [
                                model.Game
                                |> Game.getCurrentPlayer
                                |> Players.filledList
                                |> List.map (fun s ->
                                    Html.div [
                                        prop.className "col-3 record-item"
                                        match s with
                                        | "-" -> prop.dangerouslySetInnerHTML "&nbsp;"
                                        | _ -> prop.text s
                                    ])
                                |> Fable.React.Helpers.ofList
                            ]
                        ]
                        Html.div [
                            prop.className "row col-12 g-0 button-undo"
                            prop.children [
                                Html.span "Undo Last Dart"
                            ]
                            prop.onClick (fun _ -> dispatch UndoLastAction)
                        ]
                    ]
                ]
                Html.div [
                    prop.className "col-6 dartboard-container"
                    prop.children [ Dartboard dispatch ]
                ]
            ]
        ]
