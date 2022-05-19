[<AutoOpen>]
module LetsDartsCore.Parser

open Shared

let parseThrow (throw: string) : Shot option =
    let r =
        System.Text.RegularExpressions.Regex(
            @"^(?<character>[dst]{1})(?<number>\d{1,2})$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        )

    let m = r.Match(throw.ToLower())

    match (m.Success, System.Char.TryParse m.Groups["character"].Value, System.Int32.TryParse m.Groups["number"].Value)
        with
    | true, (true, c), (true, n) ->
        match c with
        | 's' -> Some(Shot(Single, n))
        | 'd' -> Some(Shot(Double, n))
        | _ -> Some(Shot(Triple, n))
    | _, (_, _), (_, _) -> None