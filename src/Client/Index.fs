namespace Client

open System
open Client.Components
open Elmish
open Fable.Remoting.Client
open Shared

module State =
    let gameApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGameApi>

    let init () : Model * Cmd<Msg> =
        let stylePlayers =
            { Id = Guid.NewGuid()
              Mode = 501
              Legs = 3
              DoubleIn = false
              DoubleOut = true
              Players =
                [ { Player.Default with Name = "Player 1" }
                  { Player.Default with Name = "Player 2" }
                  //                  { Player.Default with Name = "Player 3" }
//                  { Player.Default with Name = "Player 4" }
                  ] }

        let model =
            { State = Create; Game = stylePlayers }

        //let cmd = Cmd.OfAsync.perform gameApi.initGame model.Game ChangeGameState
        model, Cmd.none


    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
        match msg with
        // SERVER INTERACTION
        // server interaction which send a request  ( Form: Verb + Object ) |>|> outgoing
        // |>|> incoming: server response           ( Form: Passive )
        | OrderPlayers -> { model with State = Order }, Cmd.none
        | PlayersOrdered (s, g) -> { model with State = s; Game = g }, Cmd.none
        | SubmitGameSettings -> model, Cmd.OfAsync.perform gameApi.initGame model.Game GameSettingsSubmitted
        | GameSettingsSubmitted (s, g) -> { model with State = s; Game = g }, Cmd.none
        | SendShot t -> model, Cmd.OfAsync.perform gameApi.sendThrow t ShotReceived
        | ShotReceived (s, g) -> { model with State = s; Game = g }, Cmd.none
        | UndoLastAction -> model, Cmd.OfAsync.perform gameApi.undo () LastActionUndone
        | LastActionUndone (s, g) ->
            { model with
                State = s
                Game =
                    match g with
                    | Some g -> g
                    | None -> model.Game },
            Cmd.none
        // SETUP SETTINGS
        // |>|> no server interaction is happening ``[ for now ]``
        | FinishRound s ->
            match s with
            | "Next Round" -> { model with State = Run }, Cmd.none
            | _ -> { model with State = Create }, Cmd.none
        | SwitchDoubleOut b -> { model with Game = { model.Game with DoubleOut = b } }, Cmd.none
        | SwitchDoubleIn b -> { model with Game = { model.Game with DoubleIn = b } }, Cmd.none
        | AddPlayer ->
            { model with
                Game =
                    { model.Game with
                        Players =
                            model.Game.Players
                            @ [ { Player.Default with Name = $"Player%d{model.Game.Players.Length + 1}" } ] } },
            Cmd.none
        | MovePlayerPosition pl -> { model with Game = { model.Game with Players = pl } }, Cmd.none
        | ChangePlayername (index, name) ->
            let newPlayerList =
                model.Game.Players
                |> List.mapi (fun i p ->
                    if i = index then
                        { p with
                            Name =
                                match name |> String.IsNullOrEmpty with
                                | true -> $"Player{i + 1}"
                                | _ -> name }
                    else
                        p)

            { model with Game = { model.Game with Players = newPlayerList } }, Cmd.none
        | ChangeMode m -> { model with Game = { model.Game with Mode = m |> int } }, Cmd.none
        | ChangeCountOfLegs l -> { model with Game = { model.Game with Legs = l |> int } }, Cmd.none

open Feliz

module Views =
    let view (model: Model) (dispatch: Msg -> unit) =
        Fable.React.Helpers.fragment [] [
            Html.header [ prop.children [] ]
            match model.State with
            | Create -> Create.Form model dispatch
            | Order -> Sort.Form model dispatch
            | Run -> Play.Game model dispatch
            | Show -> Result.Show model dispatch
            | End -> Result.Show model dispatch
            Html.footer [ prop.children [] ]
        ]
