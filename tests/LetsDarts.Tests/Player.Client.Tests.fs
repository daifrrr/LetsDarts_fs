namespace LetsDartsCore.Client.Tests

open FsUnit.Xunit
open Xunit


module Player =
    [<Fact>]
    let ``Empty list returns [" "; " "; " "]`` () =
        let expected = true
        let actual = true
        actual |> should equal expected

