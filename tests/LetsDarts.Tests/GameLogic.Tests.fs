namespace LetsDartsCore.Tests

open Xunit
open FsUnit.Xunit
open LetsDartsCore
open Shared

module GameLogic =
    [<Fact>]
    let ``Current Player of Game.Default Should Be Player with Index 0`` () =
        let expected = 0
        let actual = Game.getCurrentPlayerIndex Game.Default.Players

        actual |> should equal expected


    [<Fact>]
    let ``Current Player Should Be Player with Index 1 When Player With Index 0 Had First Record``() =
        let expected = 1
        let players = [ { Name = "XXX"; Legs = [ { Leg.Default with Records = [Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)]} ] }
                        Player.Default ]
        let actual = players |> Game.getCurrentPlayerIndex
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Current Player Should Be Player With Index X Which Has Not Threw a Dart Yet V1``() =
        let expected = 4
        let players = [
                        { Name = "XXX"; Legs = [ { Leg.Default with Records = [Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)]} ] }
                        { Name = "XXX"; Legs = [ { Leg.Default with Records = [Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)]} ] }
                        { Name = "XXX"; Legs = [ { Leg.Default with Records = [Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)]} ] }
                        { Name = "XXX"; Legs = [ { Leg.Default with Records = [Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)]} ] }
                        Player.Default
                      ]
        let actual = players |> Game.getCurrentPlayerIndex
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Current Player Should Be Player With Index X Which Has Not Threw a Dart Yet V2``() =
        let expected = 0
        let players = [
                        { Name = "AAA"; Legs = [ { Leg.Default with Records = [Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)
                                                                               Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)]} ] }
                        { Name = "YYY"; Legs = [ { Leg.Default with Records = [Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)
                                                                               Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)]} ] }
                        { Name = "ZZZ"; Legs = [ { Leg.Default with Records = [Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)
                                                                               Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)]} ] }
                        { Name = "XXX"; Legs = [ { Leg.Default with Records = [Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)
                                                                               Shot(Single, 1); Shot(Single, 2); Shot(Single, 3)]} ] }
                      ]
        let actual = players |> Game.getCurrentPlayerIndex
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Parsing Throw Tuple from String``() =
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

    [<Fact(Skip = "skip for now")>]
    let ``Test DoubleIn List is Empty`` () =
        let expected = DoubleInSuccess, { Leg.Default with CurrentScore = 499 }
        let actual = Game.validateDoubleIn Leg.Default (Shot(Double, 1))
        actual |> should equal expected

    [<Fact>]
    let ``Test DoubleIn List is filled with zero throws`` () =
        let expected = DoubleInSuccess, {
            Leg.Default with CurrentScore = 461; Records = [Shot(Single, 0); Shot(Single, 0); Shot(Single, 0)
                                                            Shot(Single, 0); Shot(Single, 0); Shot(Single, 0)]
        }
        let _, a = expected
        let actual = ({ a with CurrentScore = 501 }, Shot(Double, 20)) ||>Game.validateDoubleIn
        actual |> should equal expected

    [<Fact>]
    let ``Test DoubleIn Player Throws Not a Double`` () =
        let expected = DoubleInFail, Leg.Default
        let actual = Game.validateDoubleIn Leg.Default (Shot(Single, 2))
        actual |> should equal expected

    [<Fact>]
    let ``Test DoubleOut Success`` () =
        let expected = GameOver, { Leg.Default with CurrentScore = 0 }
        let actual = ({Leg.Default with CurrentScore = 20}, Shot(Double, 10))
                     ||> Game.validateDoubleOut
        actual |> should equal expected

    [<Fact>]
    let ``Test DoubleOut Fail`` () =
        let expected = DoubleOutFail, { Leg.Default with CurrentScore = 40 }
        let actual = (expected |> snd, Shot(Double, 25))
                     ||> Game.validateDoubleOut
        actual |> should equal expected

    [<Fact>]
    let ``Test DoubleOut GameOn`` () =
        let expected = GameOn, { Leg.Default with CurrentScore = 20 }
        let actual = ({ Leg.Default with CurrentScore = 40 }, Shot(Single, 20))
                     ||> Game.validateDoubleOut
        actual |> should equal expected