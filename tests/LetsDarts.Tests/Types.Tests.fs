namespace LetsDartsCore.Tests

open Xunit
open FsUnit.Xunit
open Shared

module TypesTests =
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
        let shot = Shot(Double, 12)
        shot.Result |> should equal 24

    [<Fact>]
    let ``Test Hash of Shot should be equal hash(shot.Factor, shot.Value)`` () =
        let expected = hash(Double, 12)
        let ShotHash = Shot(Double, 12).GetHashCode()
        ShotHash |> should equal expected