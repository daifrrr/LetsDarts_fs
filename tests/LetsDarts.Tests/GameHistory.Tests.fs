namespace LetsDarts.Tests

open System
open FsUnit
open Xunit
open Router
open Shared

module DartsGameHistory =
    let staticGuid = Guid.NewGuid()
    let otherStaticGuid = Guid.NewGuid()

    [<Fact>]
    let ``History is empty`` () =
        let history = DartsGameHistory
        let actual = history.GetCurrentGame()
        actual |> should equal None

    [<Fact>]
    let ``history has one game`` () =
        let expected = [ { Game.Default with Id = staticGuid } ]
        let history = DartsGameHistory
        history.AddGame(expected[0])
        let actual = history.GetGames()
        actual |> should equal expected


    [<Fact>]
    let ``history has two games`` () =
        let expected1 = Some({ Game.Default with Id = staticGuid })
        let expected2 = Some({ Game.Default with Id = otherStaticGuid })
        let history = DartsGameHistory
        history.AddGame(expected1.Value)
        history.AddGame(expected2.Value)
        let actual1 = history.GetCurrentGame()
        let actual2 = history.GetBeforeCurrentGame()
        actual1 |> should equal expected2
        actual2 |> should equal expected1
