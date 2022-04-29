namespace LetsDartsCore

open System
open System.Text.RegularExpressions
open FSharp.Control.Websockets
open Shared
open Shared.Helpers

module Game =
    let getCurrentPlayerIndex (players: Player list) : int =
        let throwCounter =
            Player.getLegs players
            |> List.map (fun l -> l.Head.Records.Length)
            |> List.reduce (fun acc l -> acc + l)

        ((%) ((/) throwCounter 3) players.Length)

    let parseThrow (throw: string) : (char * int) option =
        let r =
            Regex(@"^(?<character>[dst]{1})(?<number>\d{1,2})$", RegexOptions.IgnoreCase)

        let m = r.Match(throw.ToLower())

        match (m.Success, Char.TryParse m.Groups["character"].Value, Int32.TryParse m.Groups["number"].Value) with
        | true, (true, c), (true, n) -> Some(c, n)
        | _, (_, _), (_, _) -> None

    let thrownPoints t =
        match t with
        | c, n when c = 's' -> (c, 1 * n)
        | c, n when c = 'd' -> (c, 2 * n)
        | c, n when c = 't' -> (c, 3 * n)
        | _ -> ('s', 0)

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
            |> List.fold (fun s t -> s + snd (thrownPoints t)) 0

        { l with
            CurrentScore = ((+) l.CurrentScore resetScore)
            Records = List.replicate 3 ('s', 0) @ oldRecord }

    let validateDoubleIn l (c, n) =
        match l, (c, n) with
        | l, (c, n) when c = 'd' && l.Records |> List.isEmpty ->
            (DoubleInSuccess, { l with CurrentScore = ((-) l.CurrentScore n) })
        | l, (c, n) when
            c = 'd'
            && l.Records |> List.forall (fun e -> e = ('s', 0))
            ->
            (DoubleInSuccess, { l with CurrentScore = ((-) l.CurrentScore n) })
        | _, (_, _) -> (DoubleInFail, l)

    let validateDoubleOut l (c, n) =
        match l, (c, n) with
        | l, (c, n) when c = 'd' && ((-) l.CurrentScore n) = 0 -> (GameOver, { l with CurrentScore = 0 })
        | l, (_, n) when ((-) l.CurrentScore n) < 2 -> (DoubleOutFail, l)
        | _ -> (GameOn, { l with CurrentScore = ((-) l.CurrentScore n) })

    let applyGameLogic l t (dblI, dblO) =
        match (dblI, validateDoubleIn l (thrownPoints t)) with
        | true, (DoubleInSuccess, l) -> prependThrow l t
        | true, (DoubleInFail, l) -> prependThrow l ('s', 0)
        | _ ->
            match (dblO, validateDoubleOut l (thrownPoints t)) with
            | true, (DoubleOutFail, l) -> endRecordResetLastThrows l
            | true, (GameOver, l) -> prependThrow l t
            | _, (GameOn, l) -> prependThrow l t
            | _ -> failwith "ruleset validation failed"


    let calcNewGame (throw: string) (game: Game) =
        let currentPlayer = game.Players[(Game.getPlayers game |> getCurrentPlayerIndex)]

        let currentPlayerLeg = Player.getCurrentLeg currentPlayer

        let currentThrow = (throw |> parseThrow).Value

        let state =
            applyGameLogic currentPlayerLeg currentThrow (game.DoubleIn, game.DoubleOut)

        let currentPoints =
            state.Records
            |> List.fold (fun m t -> m - snd (thrownPoints t)) game.Mode

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
                match Player.getLegs newPlayers
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