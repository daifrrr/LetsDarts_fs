namespace Shared

open System

type AppState =
    | Create
    | Order
    | Run
    | Show
    | End

type GameState =
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

[<AutoOpen>]
type Shot =
    { Factor: Factor
      Value: int
      Result: int }
    static member Shot(factor, value) =
        match factor with
        | Triple ->
            { Factor = Triple
              Value = value
              Result = 3 * value }
        | Double ->
            { Factor = Double
              Value = value
              Result = 2 * value }
        | _ ->
            { Factor = Single
              Value = value
              Result = 1 * value }

    static member ZERO = Shot.Shot(Single, 0)

    static member ToString(r: Shot) : string =
        match r.Factor with
        | Triple -> $"T{r.Value}"
        | Double -> $"D{r.Value}"
        | Single -> $"{r.Value}"

type Leg =
    { CurrentScore: int
      Records: Shot list }

    static member Default =
        { CurrentScore = 0; Records = [] }

    static member calcCurrentScore(s: Shot list) : int =
        match s with
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

    static member getLegsWon (p: Player) (mode: int) : int =
        p.Legs
        |> List.where (fun l -> l.CurrentScore = mode)
        |> List.length

    static member getAverage(p: Player) : float =
        match (p |> Player.getCurrentLeg).Records
              |> List.map (fun s -> s.Result |> float)
            with
        | [] -> 0.0
        | ss -> (ss |> List.average) * 3.0

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

    static member getCurrentLegNumber(g: Game) : int =
        let allLegsSum =
            (0, g.Players)
            ||> List.fold (fun acc p -> acc + p.Legs.Length)

        ((/) allLegsSum g.Players.Length)

    static member isFinished(g: Game) =
        g
        |> Game.getPlayers
        |> Player.getLegsPerPlayer
        |> List.map (fun r -> r |> List.sumBy (fun r -> r.CurrentScore))
        |> List.contains (g.Mode * g.Legs)

    static member getCurrentPlayerIndex(g: Game) : int =
        let throwCounter =
            g
            |> Game.getPlayers
            |> Player.getLegsPerPlayer
            |> List.map (fun l -> l.Head.Records.Length)
            |> List.reduce (fun acc l -> acc + l)

        let currentLeg = Game.getCurrentLegNumber g
        let roundCounter = ((/) throwCounter 3)
        ((%) ((-) ((+) roundCounter currentLeg) 1) g.Players.Length)

    static member getCurrentPlayer(g: Game) : Player =
        (g |> Game.getPlayers)[g |> Game.getCurrentPlayerIndex]

module Route =
    let builder typeName methodName = $"/api/%s{typeName}/%s{methodName}"

type IGameApi =
    { initGame: Game -> Async<AppState * Game>
      sendThrow: string -> Async<AppState * Game>
      undo: unit -> Async<AppState * Game option> }

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
