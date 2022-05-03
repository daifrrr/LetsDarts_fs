module Tests


open Xunit
open FsUnit.Xunit
open LetsDartsCore
open Shared

[<Fact>]
let ``Current Player of Game.Default Should Be Player with Index 0`` () =
    let expected = 0
    let actual = Game.getCurrentPlayerIndex Game.Default.Players

    actual |> should equal expected


[<Fact>]
let ``Current Player Should Be Player with Index 1 When Player With Index 0 Had First Record``() =
    let expected = 1
    let players = [ { Name = "XXX"; Legs = [ { Leg.Default with Records = ['s', 1; 's', 2; 's', 3]} ] }
                    Player.Default ]
    let actual = players |> Game.getCurrentPlayerIndex
    Assert.Equal(expected, actual)

[<Fact>]
let ``Current Player Should Be Player With Index X Which Has Not Threw a Dart Yet V1``() =
    let expected = 4
    let players = [
                    { Name = "XXX"; Legs = [ { Leg.Default with Records = ['s', 1; 's', 2; 's', 3]} ] }
                    { Name = "XXX"; Legs = [ { Leg.Default with Records = ['s', 1; 's', 2; 's', 3]} ] }
                    { Name = "XXX"; Legs = [ { Leg.Default with Records = ['s', 1; 's', 2; 's', 3]} ] }
                    { Name = "XXX"; Legs = [ { Leg.Default with Records = ['s', 1; 's', 2; 's', 3]} ] }
                    Player.Default
                  ]
    let actual = players |> Game.getCurrentPlayerIndex
    Assert.Equal(expected, actual)

[<Fact>]
let ``Current Player Should Be Player With Index X Which Has Not Threw a Dart Yet V2``() =
    let expected = 0
    let players = [
                    { Name = "AAA"; Legs = [ { Leg.Default with Records = ['t', 1; 't', 2; 't', 3; 't', 4; 't', 5; 't', 6]} ] }
                    { Name = "YYY"; Legs = [ { Leg.Default with Records = ['s', 1; 's', 2; 's', 3; 's', 4; 's', 5; 's', 6]} ] }
                    { Name = "ZZZ"; Legs = [ { Leg.Default with Records = ['d', 1; 'd', 2; 'd', 3; 'd', 4; 'd', 5; 'd', 6]} ] }
                    { Name = "XXX"; Legs = [ { Leg.Default with Records = ['s', 1; 's', 2; 's', 3; 'd', 4; 'd', 5; 'd', 6]} ] }
                  ]
    let actual = players |> Game.getCurrentPlayerIndex
    Assert.Equal(expected, actual)

[<Fact>]
let ``Parsing Throw Tuple from String``() =
    let expected1 = Some('s', 12)
    let expected2 = Some('d', 12)
    let expected3 = Some('t', 12)
    let actual1 = Game.parseThrow "s12"
    let actual2 = Game.parseThrow "d12"
    let actual3 = Game.parseThrow "t12"
    Assert.Equal(expected1, actual1)
    Assert.Equal(expected2, actual2)
    Assert.Equal(expected3, actual3)
    let expected4 = None
    let actual4 = Game.parseThrow "s123"
    Assert.Equal(expected4, actual4)

[<Fact>]
let ``Test DoubleIn List is Empty`` () =
    let expected = DoubleInSuccess, { Leg.Default with CurrentScore = 499 }
    let actual = Game.validateDoubleIn Leg.Default ('d', 2)
    Assert.Equal(expected, actual)

[<Fact>]
let ``Test DoubleIn List is filled with zero throws`` () =
    let expected = DoubleInSuccess, {
        Leg.Default with CurrentScore = 461; Records = ['s', 0; 's', 0; 's', 0; 's', 0; 's', 0]
    }
    let _, a = expected
    let actual = ({ a with CurrentScore = 501 }, ('d', 40)) ||>Game.validateDoubleIn
    actual |> should equal expected

[<Fact>]
let ``Test DoubleIn Player Throws Not a Double`` () =
    let expected = DoubleInFail, Leg.Default
    let actual = Game.validateDoubleIn Leg.Default ('s', 2)
    actual |> should equal expected

[<Fact>]
let ``Test DoubleOut Success`` () =
    let expected = GameOver, { Leg.Default with CurrentScore = 0 }
    let actual = ({Leg.Default with CurrentScore = 20}, ('d', 20))
                 ||> Game.validateDoubleOut
    actual |> should equal expected

[<Fact>]
let ``Test DoubleOut Fail`` () =
    let expected = DoubleOutFail, { Leg.Default with CurrentScore = 40 }
    let actual = (expected |> snd, ('s', 40))
                 ||> Game.validateDoubleOut
    actual |> should equal expected

let ``Test DoubleOut`` () =
    let expected = GameOn, { Leg.Default with CurrentScore = 40 }
    let actual = ({ Leg.Default with CurrentScore = 20 }, ('s', 20))
                 ||> Game.validateDoubleOut
    actual |> should equal expected
