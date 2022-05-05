﻿namespace LetsDartsCore

open System
open System.Text.RegularExpressions
open Shared

module Game =
    let prependThrow l t = { l with Records = [ t ] @ l.Records }

    let endRecordResetLastThrows l =
        let numberOfToDeleteFromRecord =
            match ((%) l.Records.Length 3) with
            | 0 -> 3
            | n -> n

        let oldRecord = l.Records |> List.skip numberOfToDeleteFromRecord

        let resetScore =
            l.Records
            |> List.take numberOfToDeleteFromRecord
            |> List.fold (fun s t -> s + t.Result) 0

        { l with
            CurrentScore = ((+) l.CurrentScore resetScore)
            Records = List.replicate 3 (Shot(Single, 0)) @ oldRecord }

    let validateDoubleIn l (r:Shot) =
        match l, r with
        | l, r when r.Factor = Double && l.Records |> List.isEmpty ->
            (DoubleInSuccess, { l with CurrentScore = ((-) l.CurrentScore r.Result) })
        | l, r when
            r.Factor = Double
            && l.Records |> List.forall (fun e -> e = (Shot(Single, 0)))
            ->
            (DoubleInSuccess, { l with CurrentScore = ((-) l.CurrentScore r.Result) })
        | _, r -> (DoubleInFail, l)

    let validateDoubleOut l (r:Shot) =
        match l, r with
        | l, r when r.Factor = Double && ((-) l.CurrentScore r.Result) = 0 -> (GameOver, { l with CurrentScore = 0 })
        | l, r when ((-) l.CurrentScore r.Result) < 2 -> (DoubleOutFail, l)
        | _ -> (GameOn, { l with CurrentScore = ((-) l.CurrentScore r.Result) })

    let applyGameLogic l (t:Shot) (dblI, dblO) =
        match (dblI, validateDoubleIn l t) with
        | true, (DoubleInSuccess, l) -> prependThrow l t
        | true, (DoubleInFail, l) -> prependThrow l (Shot(Single, 0))
        | _ ->
            match (dblO, validateDoubleOut l t) with
            | true, (DoubleOutFail, l) -> endRecordResetLastThrows l
            | true, (GameOver, l) -> prependThrow l t
            | _, (GameOn, l) -> prependThrow l t
            // TODO: no doubleIN; noDoubleOut
            | _ -> failwith "ruleset validation failed"

    let getCurrentPlayerIndex (players: Player list) : int =
        let throwCounter =
            Player.getLegsPerPlayer players
            |> List.map (fun l -> l.Head.Records.Length)
            |> List.reduce (fun acc l -> acc + l)

        ((%) ((/) throwCounter 3) players.Length)


    let calcNewGame (throw: string) (game: Game) =
        let currentPlayer = game.Players[(Game.getPlayers game |> getCurrentPlayerIndex)]

        let currentPlayerLeg = Player.getCurrentLeg currentPlayer

        let currentThrow = (throw |> parseThrow).Value

        let state =
            applyGameLogic currentPlayerLeg currentThrow (game.DoubleIn, game.DoubleOut)

        let currentPoints =
            state.Records
            |> List.fold (fun m t -> m - t.Result) game.Mode

        let players = Game.getPlayers game

        let newPlayers =
            players
            |> List.map (fun p ->
                match p = currentPlayer with
                | true -> { p with Legs = [ state ] @ p.Legs.Tail }
                | _ -> p)


        let nextStep =
            match currentPoints with
            | 0 ->
                match Player.getLegsPerPlayer newPlayers
                      |> List.concat with
                | l when l.Length < ((-)((*) game.Legs 2) 1)  ->
                    printfn $"{l}"
                    LegOver
                | _ -> GameOver
            | _ -> GameOn

        match nextStep with
        | LegOver ->
            (nextStep,
             { game with
                 Players =
                     newPlayers
                     |> List.map (fun p ->
                         { p with
                             Legs =
                                 [ { Leg.Default with CurrentScore = game.Mode } ]
                                 @ p.Legs }) })
        | GameOn -> (nextStep, { game with Players = newPlayers })
        | _ -> (nextStep, { game with Players = newPlayers })