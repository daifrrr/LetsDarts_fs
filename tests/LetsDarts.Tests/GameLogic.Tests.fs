namespace LetsDartsCore.Tests

open Xunit
open FsUnit.Xunit
open LetsDartsCore
open Shared

module GameLogic =
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
