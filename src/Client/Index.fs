namespace Client

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
            { State = RunGame; Game = styleGame }

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
        | ChangeMode m ->
            Browser.Dom.console.log m

            { model with Game = { model.Game with Mode = m |> int } }, Cmd.none
        | ChangeCountOfLegs l -> { model with Game = { model.Game with Legs = l |> int } }, Cmd.none
        | Undo -> model, Cmd.OfAsync.perform gameApi.undo () UndoDone
        | UndoDone g -> { model with Game = g }, Cmd.none

open Feliz
open Feliz.Bulma

module Views =


    let playGame (model: Model) (dispatch: Msg -> unit) =
        let currentLeg =
            Player.getLegsPerPlayer model.Game.Players
            |> List.item 0
            |> List.length

        Bulma.container [
            prop.children [
                (*              Bulma.box [ prop.children [ Bulma.columns [ Bulma.column [ text.hasTextCentered prop.text $"Mode: %d{model.Game.Mode}" ] Bulma.column [ text.hasTextCentered prop.text $"Legs: %d{currentLeg} / %d{model.Game.Legs}" ] Bulma.column [ text.hasTextCentered prop.text $"D/I: {model.Game.DoubleIn}" ] Bulma.column [ text.hasTextCentered prop.text $"D/O: {model.Game.DoubleIn}" ] ]*)
                Players.renderPlayers model.Game
                Bulma.button.a [
                    prop.className "btn-undo"
                    prop.text "Undo Last Dart"
                    prop.onClick (fun _ -> dispatch Undo)
                ]
            ]
        ]

    let showGameResult (phase: string) (dispatch: Msg -> unit) =
        Bulma.box [
            prop.children [
                Html.p [ prop.textf "Finished" ]
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
                Bulma.navbarBrand.div [
                ]
                Bulma.heroBody [
                    Bulma.container [
                        Bulma.columns [
                            Bulma.column [
                                column.is6
                                prop.children [
                                    match model.State with
                                    | CreateGame -> createForm model dispatch
                                    // | SortPlayers ->
                                    | RunGame -> playGame model dispatch
                                    | ShowResult -> showGameResult "LegOver" dispatch
                                    | FinishGame -> showGameResult "GameOver" dispatch
                                ]
                            ]
                            Bulma.column [
                                column.is6
                                prop.children [ dartBoard dispatch ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
