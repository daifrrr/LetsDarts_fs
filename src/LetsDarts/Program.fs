namespace LetsDartsCore

open Shared

module Game =
    let replaceLast_1_to_3_ShotsWithShotZERO l =
        match l.Records with
        | [] -> { l with Records = List.replicate 3 Shot.ZERO }
        | _ ->
            let numberOfToDeleteFromRecord =
                match ((%) l.Records.Length 3) with
                | 0 -> 3
                | n -> n

            let oldRecord =
                l.Records |> List.skip numberOfToDeleteFromRecord

            let resetScore =
                l.Records
                |> List.take numberOfToDeleteFromRecord
                |> List.fold (fun s t -> s + t.Result) 0

            { l with
                CurrentScore = ((-) l.CurrentScore resetScore)
                Records = List.replicate 3 Shot.ZERO @ oldRecord }

    let validateDoubleIn (s: Shot) =
        match s with
        | s when s.Factor = Double -> DoubleInSuccess
        | _ -> DoubleInFail

    let validateDoubleOut (s: Shot) =
        match s with
        | s when s.Factor = Double -> GameOver
        | _ -> Overthrown

    let prependThrow l t = { l with Records = [ t ] @ l.Records }

    let applyGameLogic l (s: Shot) g =
        let newLeg = prependThrow l s

        match (g.Mode, newLeg.Records |> Leg.calcCurrentScore)
              ||> (-)
            with
        | 0 when g.DoubleOut ->
            match validateDoubleOut s with
            | GameOver -> { newLeg with CurrentScore = newLeg.Records |> Leg.calcCurrentScore }
            | _ -> replaceLast_1_to_3_ShotsWithShotZERO l
        | 0 when g.DoubleOut |> not -> { newLeg with CurrentScore = newLeg.Records |> Leg.calcCurrentScore }
        | sc when 0 > sc && g.DoubleOut |> not -> replaceLast_1_to_3_ShotsWithShotZERO l
        | sc when 2 > sc && g.DoubleOut -> replaceLast_1_to_3_ShotsWithShotZERO l
        | sc when (g.Mode - s.Result) = sc && g.DoubleIn ->
            match validateDoubleIn s with
            | DoubleInSuccess -> { newLeg with CurrentScore = newLeg.Records |> Leg.calcCurrentScore }
            | _ -> prependThrow l Shot.ZERO
        | _ -> { newLeg with CurrentScore = newLeg.Records |> Leg.calcCurrentScore }

    let calcNewGame (throw: string) (game: Game) =
        let currentPlayer =
            Game.getCurrentPlayer game

        let currentPlayerLeg =
            currentPlayer |> Player.getCurrentLeg

        let currentThrow =
            match (throw |> parseThrow) with
            | Some s -> s
            | None -> failwithf $"Could not {throw} parse throw"

        let modifiedLeg =
            applyGameLogic currentPlayerLeg currentThrow game

        let newPlayers =
            game
            |> Game.getPlayers
            |> List.map (fun p ->
                match p = currentPlayer with
                | true -> { p with Legs = [ modifiedLeg ] @ p.Legs.Tail }
                | _ -> p)


        let nextStep =
            match ((-) game.Mode (modifiedLeg.Records |> Leg.calcCurrentScore)) with
            | 0 ->
                if { game with Players = newPlayers }
                   |> Game.isFinished then
                    GameOver
                else
                    LegOver
            | _ -> GameOn

        match nextStep with
        | LegOver ->
            (nextStep,
             { game with Players = newPlayers }
             |> Game.addNewLeg)
        | GameOn -> (nextStep, { game with Players = newPlayers })
        | _ -> (nextStep, { game with Players = newPlayers })
