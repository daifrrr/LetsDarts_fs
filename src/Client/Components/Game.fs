namespace Client.Components

open Client.Types
open Feliz
open Shared

module internal Players =
    let protectedHTMLSpace =
        ({ Fable.React.Props.__html = "&nbsp;" }
         |> Fable.React.Props.DangerouslySetInnerHTML)

    let previousRecord (p: Player) =
        let r =
            (p |> Player.getCurrentLeg).Records
            |> List.chunkBySize 3
            |> List.head

        (r |> Leg.calcCurrentScore |> string, r |> List.map (fun s -> s |> ToString))

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

    let lastRoundInfo (s, r) =
        let shotList =
            match r with
            | [] -> [ "0"; "0"; "0" ]
            | _ -> r


        Fable.React.Helpers.fragment [] [
            Html.div [
                prop.className "prev-record"
                prop.children [
                    Html.div [
                        prop.className "prev-score"
                        prop.text $"%s{s}"
                    ]
                ]
            ]
            Html.div [
                prop.className "row"
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


    let legsWon (p: Player) (mode: int, legs: int) =
        let wonLegs =
            (p, mode) ||> Player.getLegsWon


        [ 1..legs ]
        |> List.map (fun i ->
            Html.div [
                match i <= wonLegs with
                | true -> prop.className "filled"
                | _ -> prop.className "empty"
            ])
        |> Fable.React.Helpers.ofList

    let renderPlayers g =
        let ps = g |> Game.getPlayers

        let currentPlayerIndex =
            g |> Game.getCurrentPlayerIndex

        let calcFlexBasis _ =
            match ((/) 100 (ps |> List.length)) |> float with
            | x when x > 33. -> x
            | _ -> 33.

        ps
        |> List.mapi (fun i p ->
            Html.div [
                prop.style [
                    style.flexBasis (calcFlexBasis() |> length.percent)
                ]
                match i = currentPlayerIndex with
                | true -> prop.className "player active"
                | _ -> prop.className "player inactive"
                prop.children [
                    Html.div [
                        prop.className "name"
                        prop.text $"{p.Name}"
                    ]
                    Html.div [
                        prop.className "last-record"
                        prop.children [
                            match i = currentPlayerIndex with
                            | false -> p |> previousRecord |> lastRoundInfo
                            | _ -> ("–", [ "–"; "–"; "–" ]) |> lastRoundInfo
                        ]
                    ]
                    Html.div [
                        prop.className "score"
                        prop.text $"{((-) g.Mode (p |> Player.getCurrentLeg).CurrentScore)}"
                    ]
                    Html.div [
                        prop.className "average"
                        prop.text $"%.2f{p |> Player.getAverage}"
                    ]
                    Html.div [
                        prop.className "sets-legs"
                        prop.children [
                            legsWon p (g.Mode, g.Legs)
                        ]
                    ]
                ]
            ])
        |> Fable.React.Helpers.ofList


[<RequireQualifiedAccess>]
module Play =
    let Game (model: Model) (dispatch: Msg -> unit) =

        Html.div [
            prop.className "content-container game-layer"
            prop.children [
                Html.div [
                    prop.className "players-wrapper"
                    prop.children [
                        // Players
                        Html.div [
                            prop.className "players"
                            prop.children [
                                Players.renderPlayers model.Game
                            ]
                        ]
                        // Record
                        Html.div [
                            prop.className "record-wrapper"
                            prop.children [
                                model.Game
                                |> Game.getCurrentPlayer
                                |> Players.filledList
                                |> List.map (fun s ->
                                    Html.div [
                                        prop.className "record-item"
                                        match s with
                                        | "-" -> prop.dangerouslySetInnerHTML "&nbsp;"
                                        | _ -> prop.text s
                                    ])
                                |> Fable.React.Helpers.ofList
                            ]
                        ]
                        //Button
                        Html.div [
                            prop.className "ld-button red"
                            prop.children [
                                Html.span "Undo Last Dart"
                            ]
                            prop.onClick (fun _ -> dispatch UndoLastAction)
                        ]
                    ]
                ]
                Html.div [
                    prop.className "dartboard-container"
                    prop.children [
                        Html.div [
                            prop.className "dartboard"
                            prop.children [ Dartboard dispatch ]
                        ]
                    ]
                ]
            ]
        ]
