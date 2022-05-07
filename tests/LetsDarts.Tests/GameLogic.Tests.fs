namespace LetsDartsCore.Tests

open System
open Xunit
open FsUnit.Xunit
open LetsDartsCore
open Shared

module GameLogic =
    [<Fact>]
    let ``Current Player has Index 0 at the Start of a new Game`` () =
        let expected = "Player1"

        let actual =
            Game.getCurrentPlayer Game.Default

        actual.Name |> should equal expected

    [<Fact>]
    let ``Current Player has Index 1 after Player with Index0 threw 3 Shots`` () =
        let expected = "Player2"

        let testPlayers =
            [ { Name = "Player1"
                Legs = [ { Leg.Default with Records = [ Shot.ZERO; Shot.ZERO; Shot.ZERO ] } ] }
              { Name = "Player2"
                Legs = [ Leg.Default ] } ]

        let actual =
            { Game.Default with Players = testPlayers }
            |> Game.getCurrentPlayer

        actual.Name |> should equal expected

    [<Fact>]
    let ``CurrentPlayer has Index 1 in the 2nd Leg at the Start of a new Leg`` () =
        let expected = "Player2"

        let actual =
            Game.Default
            |> Game.addNewLeg
            |> Game.getCurrentPlayer

        actual.Name |> should equal expected

    [<Fact>]
    let ``CurrentPlayer has Index 0 in the 3rd Leg at the Start of a new Leg`` () =
        let expected = "Player1"

        let actual =
            Game.Default
            |> Game.addNewLeg
            |> Game.addNewLeg
            |> Game.getCurrentPlayer

        actual.Name |> should equal expected

    [<Fact>]
    let ``Player 1 starts in an Odd number Leg - Player 2 start in an Even number Leg`` () =
        let rec appendLeg counter g =
            let expected =
                $"Player{UsefulMaths.PingPong 1 2 counter}"

            let actual = g |> Game.getCurrentPlayer
            actual.Name |> should equal expected

            match counter with
            | 100. -> 0
            | n -> appendLeg ((+) n 1.) (g |> Game.addNewLeg)

        appendLeg 1. Game.Default

    [<Fact>]
    let ``Test Current Player with 8 Players in a Game`` () =
        let testPlayers =
            [ { Player.Default with Name = "Player1" }
              { Player.Default with Name = "Player2" }
              { Player.Default with Name = "Player3" }
              { Player.Default with Name = "Player4" }
              { Player.Default with Name = "Player5" }
              { Player.Default with Name = "Player6" }
              { Player.Default with Name = "Player7" }
              { Player.Default with Name = "Player8" } ]

        let testGame =
            { Game.Default with Players = testPlayers }

        let rec simulateGame counter g =
            let expected =
                $"Player{((%) counter g.Players.Length) + 1}"

            let testPlayer = g |> Game.getCurrentPlayer

            let randomRecord =
                List.replicate 3 Shot.ZERO
                @ (testPlayer |> Player.getCurrentLeg).Records

            let newPlayer =
                { testPlayer with
                    Legs =
                        [ { Leg.Default with
                              CurrentScore = randomRecord |> Leg.calcCurrentScore
                              Records = randomRecord } ] }

            let newGame =
                { g with
                    Players =
                        g.Players
                        |> List.map (fun p ->
                            if p.Name = newPlayer.Name then
                                newPlayer
                            else
                                p) }

            testPlayer.Name |> should equal expected

            ((%) testPlayer.Legs.Head.Records.Length 3)
            |> should equal 0

            match counter with
            | 100 -> newGame
            | n -> simulateGame ((+) n 1) newGame

        let expected =
            (simulateGame 0 testGame |> Game.getPlayers, 0)
            ||> List.foldBack (fun p acc -> p.Legs.Head.CurrentScore + acc)

        0 |> should equal expected


    [<Fact>]
    let ``Current Player iterates over all Players in the Game always when new Leg starts``
        ()
        =
        let testPlayers =
            [ { Player.Default with Name = "Player1" }
              { Player.Default with Name = "Player2" }
              { Player.Default with Name = "Player3" }
              { Player.Default with Name = "Player4" }
              { Player.Default with Name = "Player5" }
              { Player.Default with Name = "Player6" }
              { Player.Default with Name = "Player7" }
              { Player.Default with Name = "Player8" } ]

        let testGame =
            { Game.Default with Players = testPlayers }

        let expectedList =
            [ "Player1"
              "Player2"
              "Player3"
              "Player4"
              "Player5"
              "Player6"
              "Player7"
              "Player8" ]

        let rec appendLeg g (el: string list) =
            let expected = el.Head

            let actual =
                (g |> Game.getCurrentPlayer).Name

            actual |> should equal expected

            match el with
            | [] -> 0
            | _ ->
                appendLeg
                    ({ g with Players = g |> Game.getPlayers }
                     |> Game.addNewLeg)
                    expectedList.Tail

        appendLeg testGame |> ignore


    [<Fact>]
    let ``Parsing Throw Tuple from String`` () =
        let expected1 = Some(Shot(Single, 12))
        let expected2 = Some(Shot(Double, 12))
        let expected3 = Some(Shot(Triple, 12))
        let actual1 = Parser.parseThrow "s12"
        let actual2 = Parser.parseThrow "d12"
        let actual3 = Parser.parseThrow "t12"
        actual1 |> should equal expected1
        actual1.Value.Result |> should equal 12
        actual2 |> should equal expected2
        actual2.Value.Result |> should equal 24
        actual3 |> should equal expected3
        actual3.Value.Result |> should equal 36
        let expected4 = None
        let actual4 = Parser.parseThrow "s123"
        actual4 |> should equal expected4
        let expected5 = None
        let actual5 = Parser.parseThrow "v12"
        actual5 |> should equal expected5

    [<Fact>]
    let ``Test applyGameLogic Success Double Out true`` () =
        let testLeg =
            { Leg.Default with
                CurrentScore = 110
                Records = [ Shot(Triple, 20) ] }

        let testShot = Shot(Double, 25)

        let testGame =
            { Game.Default with
                DoubleIn = false
                DoubleOut = true
                Mode = 110 }

        let expected =
            { testLeg with
                CurrentScore = 110
                Records = [ testShot ] @ testLeg.Records }

        let actual =
            Game.applyGameLogic testLeg testShot testGame

        actual |> should equal expected

    [<Fact>]
    let ``Test applyGameLogic Fail Double Out true Shot Single thrown`` () =
        let testLeg =
            { Leg.Default with
                CurrentScore = 80
                Records = [ Shot(Triple, 20); Shot(Single, 20) ] }

        let testShot = Shot(Single, 20)

        let testGame =
            { Game.Default with
                DoubleIn = false
                DoubleOut = true
                Mode = 100 }

        let expected =
            { testLeg with
                CurrentScore = 0
                Records = List.replicate 3 Shot.ZERO }

        let actual =
            Game.applyGameLogic testLeg testShot testGame

        actual |> should equal expected

    [<Fact>]
    let ``Test applyGameLogic Fail Double Out true Overthrown`` () =
        let testLeg =
            { Leg.Default with
                CurrentScore = 60
                Records = [ Shot(Triple, 20) ] }

        let testShot = Shot(Double, 25)

        let testGame =
            { Game.Default with
                DoubleIn = false
                DoubleOut = true
                Mode = 100 }

        let expected =
            { testLeg with
                CurrentScore = 0
                Records = List.replicate 3 Shot.ZERO }

        let actual =
            Game.applyGameLogic testLeg testShot testGame

        actual |> should equal expected

    [<Fact>]
    let ``Test applyGameLogic Success Double Out false`` () =
        let testLeg =
            { Leg.Default with
                CurrentScore = 54
                Records = [ Shot(Triple, 18) ] }

        let testShot = Shot(Single, 19)

        let testGame =
            { Game.Default with
                DoubleIn = false
                DoubleOut = false
                Mode = 73 }

        let expected =
            { testLeg with
                CurrentScore = 73
                Records = [ testShot ] @ testLeg.Records }

        let actual =
            Game.applyGameLogic testLeg testShot testGame

        actual |> should equal expected

    [<Fact>]
    let ``Test applyGameLogic Success Double In true Double Out true`` () =
        let testLeg =
            { Leg.Default with
                CurrentScore = 0
                Records = [] }

        let testShot = Shot(Double, 13)

        let testGame =
            { Game.Default with
                DoubleIn = true
                DoubleOut = true
                Mode = 99 }

        let expected =
            { testLeg with
                CurrentScore = 26
                Records = [ testShot ] @ testLeg.Records }

        let actual =
            Game.applyGameLogic testLeg testShot testGame

        actual |> should equal expected

    [<Fact>]
    let ``Test applyGameLogic Success Double In true Double Out false`` () =
        let testLeg =
            { Leg.Default with
                CurrentScore = 0
                Records =
                    [ Shot.ZERO
                      Shot.ZERO
                      Shot.ZERO
                      Shot.ZERO
                      Shot.ZERO ] }

        let testShot = Shot(Double, 13)

        let testGame =
            { Game.Default with
                DoubleIn = true
                DoubleOut = false
                Mode = 199 }

        let expected =
            { testLeg with
                CurrentScore = 26
                Records = [ testShot ] @ testLeg.Records }

        let actual =
            Game.applyGameLogic testLeg testShot testGame

        actual |> should equal expected

    [<Fact>]
    let ``Test applyGameLogic Fail Double In true Double Out false`` () =
        let testLeg =
            { Leg.Default with
                CurrentScore = 0
                Records = [ Shot.ZERO; Shot.ZERO; Shot.ZERO ] }

        let testShot = Shot(Single, 13)

        let testGame =
            { Game.Default with
                DoubleIn = true
                DoubleOut = false
                Mode = 333 }

        let expected =
            { testLeg with
                CurrentScore = 0
                Records = [ Shot.ZERO ] @ testLeg.Records }

        let actual =
            Game.applyGameLogic testLeg testShot testGame

        actual |> should equal expected

    [<Fact>]
    let ``Test applyGameLogic Fail Double In true Double Out true`` () =
        let testLeg =
            { Leg.Default with
                CurrentScore = 0
                Records = [] }

        let testShot = Shot(Single, 19)

        let testGame =
            { Game.Default with
                DoubleIn = true
                DoubleOut = true
                Mode = 73 }

        let expected =
            { testLeg with
                CurrentScore = 0
                Records = [ Shot.ZERO ] @ testLeg.Records }

        let actual =
            Game.applyGameLogic testLeg testShot testGame

        actual |> should equal expected

    [<Fact>]
    let ``Test applyGameLogic Success Double In false Double Out false`` () =
        let testLeg =
            { Leg.Default with
                CurrentScore = 240
                Records =
                    [ Shot(Triple, 20)
                      Shot(Triple, 20)
                      Shot(Triple, 20)
                      Shot(Triple, 20) ] }

        let testShot = Shot(Double, 13)

        let testGame =
            { Game.Default with
                DoubleIn = false
                DoubleOut = false
                Mode = 701 }

        let expected =
            { testLeg with
                CurrentScore = 266
                Records = [ testShot ] @ testLeg.Records }

        let actual =
            Game.applyGameLogic testLeg testShot testGame

        actual |> should equal expected

    [<Fact>]
    let ``Test Replace 1 to 3 Shots from Leg Record with ShotZERO because Overthrown - Empty Records`` () =
        let expected =
            { Leg.Default with
                CurrentScore = 100
                Records = List.replicate 3 Shot.ZERO }

        let actual =
            Game.replaceLast_1_to_3_ShotsWithShotZERO { Leg.Default with CurrentScore = 100 }

        actual |> should equal expected

    [<Fact>]
    let ``Test Replace 1 to 3 Shots from Leg Record with ShotZERO because Overthrown - One Shot in Records`` () =
        let expected =
            { Leg.Default with
                CurrentScore = 0
                Records = List.replicate 3 Shot.ZERO }

        let actual =
            Game.replaceLast_1_to_3_ShotsWithShotZERO
                { Leg.Default with
                    CurrentScore = 19
                    Records = [ Shot(Single, 19) ] }

        actual |> should equal expected

    [<Fact>]
    let ``Test Replace 1 to 3 Shots from Leg Record with ShotZERO because Overthrown - Two Shots in Records`` () =
        let expected =
            { Leg.Default with
                CurrentScore = 0
                Records = List.replicate 3 Shot.ZERO }

        let actual =
            Game.replaceLast_1_to_3_ShotsWithShotZERO
                { Leg.Default with
                    CurrentScore = 120
                    Records = [ Shot(Triple, 20); Shot(Triple, 20) ] }

        actual |> should equal expected

    [<Fact>]
    let ``Test Replace 1 to 3 Shots from Leg Record with ShotZERO because Overthrown - One Full Record in Records`` () =
        let expected =
            { Leg.Default with
                CurrentScore = 0
                Records = List.replicate 3 Shot.ZERO }

        let actual =
            Game.replaceLast_1_to_3_ShotsWithShotZERO
                { Leg.Default with
                    CurrentScore = 180
                    Records =
                        [ Shot(Triple, 20)
                          Shot(Triple, 20)
                          Shot(Triple, 20) ] }

        actual |> should equal expected

    [<Fact>]
    let ``Test Replace 1 to 3 Shots from Leg Record with ShotZERO because Overthrown - X Shots in Records`` () =
        let expected =
            { Leg.Default with
                CurrentScore = 180
                Records =
                    List.replicate 3 Shot.ZERO
                    @ [ Shot(Triple, 20)
                        Shot(Triple, 20)
                        Shot(Triple, 20) ] }

        let actual =
            Game.replaceLast_1_to_3_ShotsWithShotZERO
                { Leg.Default with
                    CurrentScore = 300
                    Records =
                        [ Shot(Triple, 20)
                          Shot(Triple, 20)
                          Shot(Triple, 20)
                          Shot(Triple, 20)
                          Shot(Triple, 20) ] }

        actual |> should equal expected
