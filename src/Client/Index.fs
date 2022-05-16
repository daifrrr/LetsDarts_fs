namespace Client

open Elmish
open Fable.Remoting.Client
open Shared

module State =
    let gameApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGameApi>

    let init () : Model * Cmd<Msg> =
        let styleGame =
            { Game.Default with
                Mode = 501
                Legs = 3
                DoubleIn = false
                DoubleOut = true
                Players =
                    [ { Name = "Kai"
                        Legs = [ { CurrentScore = 0; Records = [] } ] }
                      { Name = "David"
                        Legs = [ { CurrentScore = 0; Records = [] } ] }
                      { Name = "Frieda"
                        Legs = [ { CurrentScore = 0; Records = [] } ] }
                      { Name = "Philipp"
                        Legs = [ { CurrentScore = 0; Records = [] } ] } ] }


        let model =
            { State = CreateGame
              Game = Game.Default }

        // let cmd = Cmd.OfAsync.perform todosApi.getTodos () GotTodos
        //let cmd = Cmd.OfAsync.perform gameApi.initGame model.Game ChangeGameState

        model, Cmd.none


    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
        match msg with
        // SERVER INTERACTION
        // server interaction which send a request  ( Form: Verb + Object ) |>|> outgoing
        // |>|> incoming: server response           ( Form: Passive Construct )
        | OrderPlayers -> model, Cmd.OfAsync.perform gameApi.sortPlayers model.Game PlayersOrdered
        | PlayersOrdered (s, g) -> { model with State = s; Game = g }, Cmd.none
        | SubmitGameSettings -> model, Cmd.OfAsync.perform gameApi.initGame model.Game GameSettingsSubmitted
        | GameSettingsSubmitted (s, g) -> { model with State = s; Game = g }, Cmd.none
        | SendShot t -> model, Cmd.OfAsync.perform gameApi.sendThrow t ShotReceived
        | ShotReceived (s, g) -> { model with State = s; Game = g }, Cmd.none
        | UndoLastAction -> model, Cmd.OfAsync.perform gameApi.undo () LastActionUndone
        | LastActionUndone (s, g) -> { model with State = s; Game = g }, Cmd.none
        // SETUP SETTINGS
        // |>|> no server interaction is happening ``[ for now ]``
        | CloseShowResults -> { model with State = RunGame }, Cmd.none
        | EndGame ->
            { model with
                State = CreateGame
                Game = Game.Default },
            Cmd.none
        | SwitchDoubleOut b -> { model with Game = { model.Game with DoubleOut = b } }, Cmd.none
        | SwitchDoubleIn b -> { model with Game = { model.Game with DoubleIn = b } }, Cmd.none
        | AddPlayer p ->
            { model with
                Game =
                    { model.Game with
                        Players =
                            model.Game.Players
                            @ [ { p with Name = $"Player%d{model.Game.Players.Length + 1}" } ] } },
            Cmd.none
        | ChangePlayername (index, name) ->
            let newPlayerList =
                model.Game.Players
                |> List.mapi (fun i p ->
                    if i = index then
                        { p with Name = name }
                    else
                        p)

            { model with Game = { model.Game with Players = newPlayerList } }, Cmd.none
        | ChangeMode m -> { model with Game = { model.Game with Mode = m |> int } }, Cmd.none
        | ChangeCountOfLegs l -> { model with Game = { model.Game with Legs = l |> int } }, Cmd.none

open Feliz
open Feliz.Bulma
open Client.Components

module Views =
    let sortPlayers (model: Model) (dispatch: Msg -> unit) =
        Bulma.container [
            Bulma.columns [
                prop.className "srt"
                prop.children [
                    model.Game
                    |> Game.getPlayers
                    |> List.mapi (fun i p ->
                        Bulma.column [
                            prop.text p.Name
                        ]
                    )
                    |> Fable.React.Helpers.ofList
                    Bulma.button.span [
                        prop.className "btn-game-start"
                        prop.text "Start"
                        prop.onClick (fun _ -> dispatch SubmitGameSettings)
                    ]
                ]
            ]
        ]

    let playGame (model: Model) (dispatch: Msg -> unit) =
        Bulma.container [
            Bulma.columns [
                prop.children [
                    Bulma.column [
                        column.is6
                        prop.children [
                            Players.renderPlayers model.Game
                            Bulma.button.a [
                                prop.className "btn-undo"
                                prop.text "Undo Last Dart"
                                prop.onClick (fun _ -> dispatch UndoLastAction)
                            ]
                        ]
                    ]
                    Bulma.column [
                        column.is6
                        prop.children [ dartBoard dispatch ]
                    ]
                ]
            ]
        ]

    let showGameResult (phase: string) (dispatch: Msg -> unit) =
        Bulma.container [
            prop.children [
                Html.p [ prop.text $"%s{phase}" ]
                Bulma.button.a [
                    color.isInfo
                    match phase with
                    | "LegOver" ->
                        prop.text "Close"
                        prop.onClick (fun _ -> dispatch CloseShowResults)
                    | _ ->
                        prop.text "New Game"
                        prop.onClick (fun _ -> dispatch EndGame)
                ]
            ]
        ]

    let view (model: Model) (dispatch: Msg -> unit) =
        Bulma.hero [
                hero.isFullHeight
                prop.children [
                    Bulma.heroHead []
                    Bulma.heroBody [
                        match model.State with
                        | CreateGame -> createForm model dispatch
                        | ChangePlayerOrder -> sortPlayers model dispatch
                        | RunGame -> playGame model dispatch
                        | ShowResult -> showGameResult "LegOver" dispatch
                        | FinishGame -> showGameResult "GameOver" dispatch
                    ]
                    Bulma.heroFoot []
                ]
        ]