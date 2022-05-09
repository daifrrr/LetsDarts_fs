namespace LetsDartsCore.Tests

open FsUnit.Xunit
open Shared
open Xunit




module TypesTests =
    let staticGuid = System.Guid.NewGuid()

    [<Fact>]
    let ``Test Shot Single * Value Equals 1 * Value`` () =
        let shot = Shot(Single, 12)
        shot.Result |> should equal 12

    [<Fact>]
    let ``Test Shot Double * Value Equals 2 * Value`` () =
        let shot = Shot(Double, 12)
        shot.Result |> should equal 24

    [<Fact>]
    let ``Test Shot Triple * Value Equals 3 * Value`` () =
        let shot = Shot(Triple, 12)
        shot.Result |> should equal 36

    [<Fact>]
    let ``Test Shot ZERO`` () =
        let expectedFactor = Single
        let expectedValue = 0
        let expectedResult = 0
        let expectedString = "0"
        let actual = Shot.ZERO
        actual.Factor |> should equal expectedFactor
        actual.Value |> should equal expectedValue
        actual.Result |> should equal expectedResult

        actual
        |> Shot.ToString
        |> should equal expectedString

    [<Fact>]
    let ``Test Shot Equality`` () =
        Shot(Single, 20).Equals(Some("Anything"))
        |> should be False

        Shot(Single, 20).Equals(Shot(Double, 20))
        |> should be False

        Shot(Single, 13).Equals(Shot(Single, 13))
        |> should be True

        Shot(Triple, 20)
        |> should not' (equal (Shot(Triple, 19)))

        Shot(Triple, 20)
        |> should equal (Shot(Triple, 20))

    [<Fact>]
    let ``Test ToString = First Char of Factor + Value`` () =
        Shot(Single, 3) |> ToString
        |> should equal "3"

        Shot(Double, 2) |> ToString
        |> should equal "D2"

        Shot(Triple, 1) |> ToString
        |> should equal "T1"


    [<Fact>]
    let ``Test Leg Default`` () =
        let expected =
            { CurrentScore = 0; Records = [] }

        let actual = Leg.Default
        actual |> should equal expected

    [<Fact>]
    let ``Test Leg Calculates CurrentScore with Empty Leg Records`` () =
        let expected = 0

        let actual =
            Leg.Default.Records |> Leg.calcCurrentScore

        actual |> should equal expected

    [<Fact>]
    let ``Test Leg Calculates CurrentScore`` () =
        let expected = 55

        let actual =
            [ Shot(Single, 1); Shot(Triple, 18) ]
            |> Leg.calcCurrentScore

        actual |> should equal expected

    [<Fact>]
    let ``Test Player Default`` () =
        let expected =
            { Name = "Player"
              Legs = [ Leg.Default ] }

        let actual = Player.Default
        actual |> should equal expected

    [<Fact>]
    let ``Test Player Get Current Leg (the Head of Leg list)`` () =
        let expected =
            { Leg.Default with Records = [ Shot(Triple, 20); Shot(Triple, 19) ] }

        let actual =
            { Player.Default with
                Legs =
                    [ { Leg.Default with Records = [ Shot(Triple, 20); Shot(Triple, 19) ] }
                      Leg.Default
                      Leg.Default
                      Leg.Default ] }
            |> Player.getCurrentLeg

        actual |> should equal expected

    [<Fact>]
    let ``Test Player Get All Legs From a Single Player `` () =
        let expected =
            [ Leg.Default
              { Leg.Default with Records = [ Shot(Triple, 20); Shot(Triple, 19) ] } ]

        let actual =
            { Player.Default with
                Legs =
                    [ Leg.Default
                      { Leg.Default with Records = [ Shot(Triple, 20); Shot(Triple, 19) ] } ] }
            |> Player.getLegForPlayer

        actual |> should equal expected

    [<Fact>]
    let ``Test Player list with Default Get All Legs in List of Leg lists`` () =
        let expected =
            [ [ Leg.Default ]; [ Leg.Default ] ]

        let actual =
            [ Player.Default; Player.Default ]
            |> Player.getLegsPerPlayer

        actual |> should equal expected

    [<Fact>]
    let ``Test Player list with Default Get All Legs in List of Legs`` () =
        let expected = [ Leg.Default; Leg.Default ]

        let actual =
            [ Player.Default; Player.Default ]
            |> Player.getLegs

        actual |> should equal expected

    [<Fact>]
    let ``Test Game Default`` () =
        let expected =
            { Id = staticGuid
              Mode = 501
              Legs = 3
              DoubleIn = false
              DoubleOut = true
              Players = [] }

        let actual =
            { Game.Default with
                Id = staticGuid
                Players = List.Empty }

        actual |> should equal expected

    [<Fact>]
    let ``Test Game Get List of the Players in the Game`` () =
        let expected =
            [ { Player.Default with Name = "Player1" }
              { Player.Default with Name = "Player2" } ]

        let actual = Game.Default |> Game.getPlayers
        actual |> should equal expected

    [<Fact>]
    let ``Test Game add new Leg to Game.Players`` () =
        let expected = 2
        let actual = Game.Default |> Game.addNewLeg

        let actualCurrentLeg =
            actual |> Game.getCurrentLegNumber

        for p in actual.Players do
            p.Legs.Length |> should equal expected

        actualCurrentLeg |> should equal expected
        let actual2 = actual |> Game.addNewLeg

        let actualCurrentLeg2 =
            actual2 |> Game.getCurrentLegNumber

        for p in actual2.Players do
            p.Legs.Length |> should equal ((+) expected 1)

        actualCurrentLeg2 |> should equal ((+) expected 1)

    [<Fact>]
    let ``Test Game add new Leg to Game.Players with loop`` () =
        let rec addLegToGame counter (g: Game) =
            (g |> Game.getCurrentLegNumber)
            |> should equal counter

            match counter with
            | 19 -> 0
            | n ->
                for p in g.Players do
                    p.Legs.Length |> should equal n

                addLegToGame ((+) counter 1) (g |> Game.addNewLeg)

        addLegToGame 1 Game.Default

    [<Fact>]
    let ``Test Game is Finished Success`` () =
        let testPlayers =
            [ { Player.Default with
                  Name = "Player1"
                  Legs =
                      [ { Leg.Default with CurrentScore = 501 }
                        { Leg.Default with CurrentScore = 501 }
                        { Leg.Default with CurrentScore = 501 } ] }
              { Player.Default with
                  Name = "Player1"
                  Legs =
                      [ { Leg.Default with CurrentScore = 123 }
                        { Leg.Default with CurrentScore = 456 }
                        { Leg.Default with CurrentScore = 789 } ] } ]

        let actual = { Game.Default with Players = testPlayers} |> Game.isFinished
        actual |> should be True

    [<Fact>]
    let ``Test Game is Finished Fail`` () =
        let testPlayers =
            [ { Player.Default with
                  Name = "Player1"
                  Legs =
                      [ { Leg.Default with CurrentScore = 499 }
                        { Leg.Default with CurrentScore = 501 }
                        { Leg.Default with CurrentScore = 501 } ] }
              { Player.Default with
                  Name = "Player1"
                  Legs =
                      [ { Leg.Default with CurrentScore = 123 }
                        { Leg.Default with CurrentScore = 456 }
                        { Leg.Default with CurrentScore = 789 } ] } ]

        let actual = { Game.Default with Players = testPlayers} |> Game.isFinished
        actual |> should be False


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
            | l when l.Length = 1 -> 0
            | _ ->
                appendLeg
                    ({ g with Players = g |> Game.getPlayers }
                     |> Game.addNewLeg)
                    el.Tail

        0 |> should equal (appendLeg testGame expectedList)