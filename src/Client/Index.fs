namespace Client

open Client
open Client.Components
open Elmish
open Fable.Remoting.Client
open Feliz.Bulma
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
              Game = styleGame }

        // let cmd = Cmd.OfAsync.perform todosApi.getTodos () GotTodos
        //let cmd = Cmd.OfAsync.perform gameApi.initGame model.Game ChangeGameState

        model, Cmd.none


    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
        match msg with
        | SubmitGameSettings -> model, Cmd.OfAsync.perform gameApi.initGame model.Game ChangeGameState
        | ChangeGameState (s, g) -> { model with State = s; Game = g }, Cmd.none
        | GetThrow t -> model, Cmd.OfAsync.perform gameApi.sendThrow t GotThrow
        | GotThrow (s, g) -> { model with State = s; Game = g }, Cmd.none
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
        | Undo -> model, Cmd.OfAsync.perform gameApi.undo () UndoDone
        | UndoDone g -> { model with Game = g }, Cmd.none

open Feliz

module Views =

    let sortPlayers (model: Model) (dispatch: Msg -> unit) =
        Bulma.button.a [
            color.isInfo
            prop.text "Start"
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
                                prop.onClick (fun _ -> dispatch Undo)
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
        Bulma.box [
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
            hero.isFullHeightWithNavbar
            prop.children [
                Bulma.navbarBrand.div []
                Bulma.heroBody [
                    prop.children [
                        match model.State with
                        | CreateGame -> createForm model dispatch
                        //                        | SortPlayers -> sortPlayers model dispatch
                        | RunGame -> playGame model dispatch
                        | ShowResult -> showGameResult "LegOver" dispatch
                        | FinishGame -> showGameResult "GameOver" dispatch
                    ]
                ]
                //                Bulma.footer [ prop.children [] ]
                ]
        ]
