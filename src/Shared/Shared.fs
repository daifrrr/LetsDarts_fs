namespace Shared

open System

type Todo = { Id: Guid; Description: string }

type Leg =
    { CurrentScore: int
      Records: (char * int) list }

    static member Default = { CurrentScore = 0; Records = [] }

type Player =
    { Name: string
      Legs: Leg list }
    static member Default =
        { Name = "Player"
          Legs = [ Leg.Default ] }

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

module Todo =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create (description: string) =
        { Id = Guid.NewGuid()
          Description = description }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ITodosApi =
    { getTodos: unit -> Async<Todo list>
      addTodo: Todo -> Async<Todo> }

module DartGame =
    let DefaultLeg = Leg.Default

    let DefaultPlayer = Player.Default

    let DefaultGame = Game.Default

module Constans =
    let DartNumbers =
        seq {20;1;18;4;13;6;10;15;2;17;3;19;7;16;8;11;14;9;12;5}