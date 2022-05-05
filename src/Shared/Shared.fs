namespace Shared

open System

type State =
    | CreateGame
    | RunGame
    | ShowResult
    | FinishGame

type RuleSet =
    | GameOn
    | GameOver
    | LegOver
    | DoubleInFail
    | DoubleInSuccess
    | Overthrown

type Factor =
    | Single
    | Double
    | Triple

type Shot(factor: Factor, value: int) =
    member this.Factor = factor
    member this.Value = value

    member this.Result =
        match factor with
        | Single -> 1 * this.Value
        | Double -> 2 * this.Value
        | Triple -> 3 * this.Value

    static member ZERO = Shot(Single, 0)

    static member ToString(r: Shot) : string =
        match r.Factor with
        | Single -> $"{r.Value}"
        | Double -> $"D{r.Value}"
        | Triple -> $"T{r.Value}"

    override x.GetHashCode() = hash (factor, value)

    override x.Equals(s) =
        match s with
        | :? Shot as p -> (factor, value) = (p.Factor, p.Value)
        | _ -> false


type Leg =
    { CurrentScore: int
      Records: Shot list }

    static member Default = { CurrentScore = 0; Records = [] }

    static member calcCurrentScore(l: Leg) : int =
        match l.Records with
        | [] -> 0
        | s ->
            (s, 0)
            ||> List.foldBack (fun s acc -> s.Result + acc)


type Player =
    { Name: string
      Legs: Leg list }
    static member Default =
        { Name = "Player"
          Legs = [ Leg.Default ] }

    static member getCurrentLeg(p: Player) : Leg = p.Legs |> List.head
    static member getLegForPlayer(p: Player) = p.Legs
    static member getLegsPerPlayer(pl: Player list) = pl |> List.map (fun pl -> pl.Legs)

    static member getLegs(pl: Player list) =
        pl |> List.map (fun pl -> pl.Legs) |> List.concat

type Game =
    { Id: Guid
      Mode: int
      Legs: int
      DoubleIn: bool
      DoubleOut: bool
      Players: Player list }

    static member Default =
        { Id = Guid.NewGuid()
          Mode = 501
          Legs = 3
          DoubleIn = false
          DoubleOut = true
          Players =
            [ { Player.Default with Name = "Player1" }
              { Player.Default with Name = "Player2" } ] }

    static member getPlayers(g: Game) = g.Players

    static member addNewLeg(g: Game) =
        let players = g |> Game.getPlayers

        { g with
            Players =
                players
                |> List.map (fun p -> { p with Legs = [ Leg.Default ] @ p.Legs }) }

    static member getCurrentLeg(g: Game): int =
        let allLegsSum = (0, g.Players)
                         ||> List.fold (fun acc p -> acc + p.Legs.Length)
        ((/) allLegsSum g.Players.Length)

module Route =
    let builder typeName methodName = $"/api/%s{typeName}/%s{methodName}"

type IGameApi =
    { initGame: Game -> Async<State * Game>
      sendThrow: string -> Async<State * Game>
      undo: unit -> Async<Game> }

[<AutoOpen>]
module Constants =
    let DARTNUMBERS =
        seq {
            20
            1
            18
            4
            13
            6
            10
            15
            2
            17
            3
            19
            7
            16
            8
            11
            14
            9
            12
            5
        }

    [<AutoOpen>]
    module Colors =
        let BLACK = "#282a38"
        let WHITE = "#fde1d0"
        let RED = "#d95652"
        let GREEN = "#528b6e"
        let BACKGROUND = "#38394d"
