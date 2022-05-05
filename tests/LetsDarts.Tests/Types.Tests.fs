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
    let ``Test Hash of Shot should be equal hash(shot.Factor, shot.Value)`` () =
        let expected = hash (Double, 12)

        let ShotHash =
            Shot(Double, 12).GetHashCode()

        ShotHash |> should equal expected

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
        Shot(Single, 3)
        |> Shot.ToString
        |> should equal "3"

        Shot(Double, 2)
        |> Shot.ToString
        |> should equal "D2"

        Shot(Triple, 1)
        |> Shot.ToString
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
            Leg.Default |> Leg.calcCurrentScore

        actual |> should equal expected

    [<Fact>]
    let ``Test Leg Calculates CurrentScore`` () =
        let expected = 55

        let actual =
            { Leg.Default with Records = [ Shot(Single, 1); Shot(Triple, 18) ] }
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
        let actualCurrentLeg = actual |> Game.getCurrentLeg
        for p in actual.Players do
            p.Legs.Length |> should equal expected
        actualCurrentLeg |> should equal expected
        let actual2 = actual |> Game.addNewLeg
        let actualCurrentLeg2 = actual2 |> Game.getCurrentLeg
        for p in actual2.Players do
            p.Legs.Length |> should equal ((+) expected 1)
        actualCurrentLeg2 |> should equal ((+) expected 1)

    [<Fact>]
    let ``Test Game add new Leg to Game.Players with loop`` () =
        let rec addLegToGame counter (g: Game) =
            (g |> Game.getCurrentLeg) |> should equal counter
            match counter with
            | 19 -> 0
            | n -> for p in g.Players do
                       p.Legs.Length |> should equal n
                   addLegToGame ((+) counter 1) (g |> Game.addNewLeg)
        addLegToGame 1 Game.Default