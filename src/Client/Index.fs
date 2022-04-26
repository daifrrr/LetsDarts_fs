namespace Index

open Elmish
open Fable.Remoting.Client
open Shared

type Model = { State: State; Game: Game }

type Msg =
    | GameSettingsSubmit
    | GameStateChanged of State * Game
    | GetThrow of string
    | GotThrow of Game
    | SwitchDoubleOut of bool
    | SwitchDoubleIn of bool
    | AddPlayer of Player
    | PlayernameChange of int * string
    | ModeChanged of string
    | LegsChanged of string
    | Undo
    | UndoDone of Game

module State =
    let gameApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGameApi>

    let init () : Model * Cmd<Msg> =
        let model = { State = Running; Game = Game.Default }

        // let cmd = Cmd.OfAsync.perform todosApi.getTodos () GotTodos
        let cmd = Cmd.OfAsync.perform gameApi.initGame model.Game GameStateChanged

        model, cmd


    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
        match msg with
        | GameSettingsSubmit -> model, Cmd.OfAsync.perform gameApi.initGame model.Game GameStateChanged
        | GameStateChanged (s, g) -> { model with State = s; Game = g }, Cmd.none
        | GetThrow t -> model, Cmd.OfAsync.perform gameApi.sendThrow t GotThrow
        | GotThrow g -> { model with Game = g }, Cmd.none
        | SwitchDoubleOut b -> { model with Game = { model.Game with DoubleIn = b } }, Cmd.none
        | SwitchDoubleIn b -> { model with Game = { model.Game with DoubleIn = b } }, Cmd.none
        | AddPlayer p ->
            { model with
                Game =
                    { model.Game with
                        Players =
                            model.Game.Players
                            @ [ { p with Name = $"Player%d{model.Game.Players.Length + 1}" } ] } },
            Cmd.none
        | PlayernameChange (index, name) ->
            let newPlayerList =
                model.Game.Players
                |> List.mapi (fun i p ->
                    if i = index then
                        { p with Name = name }
                    else
                        p)

            { model with Game = { model.Game with Players = newPlayerList } }, Cmd.none
        | ModeChanged m ->
            Browser.Dom.console.log m

            { model with
                Game =
                    { model.Game with
                        Mode = m |> int
                        Players =
                            model.Game.Players
                            |> List.map (fun p ->
                                { p with
                                    Legs =
                                        [ { CurrentScore = m |> int
                                            Records = [] } ] }) } },
            Cmd.none
        | LegsChanged l ->
            { model with Game = { model.Game with Legs = l |> int } }, Cmd.none
        | Undo -> model, Cmd.OfAsync.perform gameApi.undo () UndoDone
        | UndoDone g -> { model with Game = g }, Cmd.none

module Events =
    ///External event wrapping a message
    let mapEvent = Event<Msg>()

    ///Subscription on external events to bring them into Elmish message queue
    let mapEventSubscription _ =
        let sub dispatch =
            let msgSender msg = msg |> dispatch

            mapEvent.Publish.Add(msgSender)

        Cmd.ofSub sub

    let testHandler i = Browser.Dom.console.log i

    let addPlayer _ =
        mapEvent.Trigger(AddPlayer Player.Default)

    let handleClick (ev: Browser.Types.Event) =
        let evm = ev |> unbox<Browser.Types.MouseEvent>

        let id = evm.target |> unbox<Browser.Types.Element>

        Browser.Dom.console.log (id.getAttribute "id")
        mapEvent.Trigger(GetThrow(id.getAttribute "id"))

    let handleInput index name =
        Browser.Dom.console.log name
        Browser.Dom.console.log index
        mapEvent.Trigger(PlayernameChange(index, name))

open Feliz
open Feliz.Bulma

module Views =
    let navBrand =
        Bulma.navbarBrand.div [
            Bulma.navbarItem.a [
                prop.href "https://safe-stack.github.io/"
                navbarItem.isActive
                prop.children [
                    Html.img [
                        prop.src "/favicon.png"
                        prop.alt "Logo"
                    ]
                ]
            ]
        ]

    let sections (fieldValue: int, angle: float) (color: string * string) =
        Svg.g [
            svg.id (string $"f{fieldValue}")
            svg.transform.rotate -angle
            svg.children [
                Svg.use' [
                    svg.id (string $"s{fieldValue}")
                    svg.href "#single"
                    svg.height 500
                    svg.width 500
                    svg.y 0
                    svg.x 0
                    svg.fill (fst color)
                    svg.onClick Events.handleClick
                ]
                Svg.use' [
                    svg.id (string $"d{fieldValue}")
                    svg.href "#double"
                    svg.height 500
                    svg.width 500
                    svg.y 0
                    svg.x 0
                    svg.fill (snd color)
                    svg.onClick Events.handleClick
                ]
                Svg.use' [
                    svg.id (string $"t{fieldValue}")
                    svg.href "#triple"
                    svg.height 500
                    svg.width 500
                    svg.y 0
                    svg.x 0
                    svg.fill (snd color)
                    svg.onClick Events.handleClick
                ]
            ]
        ]

    let dartBoard (model: Model) (dispatch: Msg -> unit) =
        Bulma.box [
            prop.children [
                Bulma.container [
                    prop.className "dartboardContainer"
                    prop.children [
                        Svg.svg [
                            svg.id "svg2"
                            svg.viewBox (-250, -250, 500, 500)
                            unbox ("version", "1.0")
                            unbox ("xmlns:rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#")
                            unbox ("xmlns", "http://www.w3.org/2000/svg")
                            unbox ("xmlns:cc", "http://creativecommons.org/ns#")
                            unbox ("xmlnsXlink", "http://www.w3.org/1999/xlink") // Error when xmlns:xlink
                            unbox ("xmlns:dc", "http://purl.org/dc/elements/1.1/")
                            unbox ("xmlns:svg", "http://www.w3.org/2000/svg")
                            svg.children [
                                Svg.defs [
                                    svg.id "defs6"
                                    svg.children [
                                        Svg.path [
                                            svg.id "sSLICE"
                                            svg.stroke "#bbbbbb"
                                            svg.strokeWidth 2
                                            svg.d
                                                "M 0 0 L 39.108616 246.922085 A 250 250 0 0 0 -39.108616 246.922085 L 0 0 Z"
                                        ]
                                        Svg.path [
                                            svg.id "dSLICE"
                                            svg.stroke "#bbbbbb"
                                            svg.strokeWidth 2
                                            svg.d
                                                "M 31.286893 197.537668 L 39.108616 246.922085 A 250 250 0 0 1 -39.108616 246.922085 L -31.286893 197.537668 A 200 200 0 0 0 31.286893 197.537668 Z"
                                        ]
                                        Svg.path [
                                            svg.id "tSLICE"
                                            svg.stroke "#bbbbbb"
                                            svg.strokeWidth 2
                                            svg.d
                                                "M 15.643447 98.768834 L 23.465169 148.153251 A 150 150 0 0 1 -23.465169 148.153251 L -15.643447 98.768834 A 100 100 0 0 0 15.643447 98.768834 Z"
                                        ]
                                        Svg.use' [
                                            svg.id "single"
                                            unbox ("xlinkHref", "#sSLICE")
                                            svg.height 500
                                            svg.width 500
                                            svg.y 0
                                            svg.x 0
                                        ]
                                        Svg.use' [
                                            svg.id "double"
                                            unbox ("xlinkHref", "#dSLICE")
                                            svg.height 500
                                            svg.width 500
                                            svg.y 0
                                            svg.x 0
                                        ]
                                        Svg.use' [
                                            svg.id "triple"
                                            unbox ("xlinkHref", "#tSLICE")
                                            svg.height 500
                                            svg.width 500
                                            svg.y 0
                                            svg.x 0
                                        ]
                                    ]
                                ]
                                Svg.g [
                                    svg.id "gTransform"
                                    svg.transform.matrix (1, 0, 0, -1, -1, 0)
                                    svg.children [
                                        Svg.g [
                                            // TODO: Seq<a'> Seq<b'> must have same length, check!
                                            svg.id "dartboard"
                                            svg.children [
                                                (Constants.DartNumbers, seq { 0.0..18.0..342.0 })
                                                ||> Seq.map2 (fun n a -> (n, a))
                                                |> Seq.indexed
                                                |> Seq.map (fun (i, (n, a)) ->
                                                    match i % 2 = 0 with
                                                    | true -> sections (n, a) ("#0", "#ff0000")
                                                    | _ -> sections (n, a) ("#ffffff", "#00ff00"))
                                                |> List.ofSeq
                                                |> Fable.React.Helpers.ofList
                                                Svg.circle [
                                                    svg.id (string "s25")
                                                    svg.cx 0
                                                    svg.cx 0
                                                    svg.r 50
                                                    svg.stroke "#bbbbbb"
                                                    svg.strokeWidth 2
                                                    svg.fill "#00ff00"
                                                    svg.onClick Events.handleClick
                                                ]
                                                Svg.circle [
                                                    svg.id (string "d25")
                                                    svg.cx 0
                                                    svg.cx 0
                                                    svg.r 25
                                                    svg.stroke "#bbbbbb"
                                                    svg.strokeWidth 2
                                                    svg.fill "#ff0000"
                                                    svg.onClick Events.handleClick
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

    let createForm (model: Model) (dispatch: Msg -> unit) =
        Bulma.container [
            prop.children [
                Bulma.box [
                    prop.children [
                        Bulma.title [
                            text.hasTextCentered
                            color.hasTextBlack
                            prop.text "LetsDarts"
                        ]
                        Bulma.columns [
                            Bulma.column [
                                Bulma.label "Mode"
                                Bulma.select [
                                    prop.value (string model.Game.Mode)
                                    text.hasTextCentered
                                    prop.children [
                                        Html.option 301
                                        Html.option 501
                                    ]
                                    prop.onChange (ModeChanged >> dispatch)
                                ]
                            ]
                            Bulma.column [
                                Bulma.label "First To Legs"
                                Bulma.select [
                                    prop.value model.Game.Legs
                                    text.hasTextCentered
                                    prop.children [
                                        Html.option 1
                                        Html.option 3
                                        Html.option 5
                                        Html.option 7
                                    ]
                                    prop.onChange (LegsChanged >> dispatch)
                                ]
                            ]
                        ]
                        Bulma.columns [
                            Bulma.column [
                                Bulma.label [
                                    Bulma.input.checkbox [
                                        prop.onCheckedChange (SwitchDoubleIn >> dispatch)
                                        prop.isChecked model.Game.DoubleIn
                                    ]
                                    Bulma.text.span [
                                        prop.style [ style.marginLeft 8 ]
                                        prop.text "Double In"
                                    ]
                                ]
                            ]
                            Bulma.column [
                                Bulma.label [
                                    Bulma.input.checkbox [
                                        prop.onCheckedChange (SwitchDoubleOut >> dispatch)
                                        prop.isChecked model.Game.DoubleOut
                                    ]
                                    Bulma.text.span [
                                        prop.style [ style.marginLeft 8 ]
                                        prop.text "Double Out"
                                    ]
                                ]
                            ]
                        ]
                        Bulma.columns [
                            Bulma.column [ Bulma.label "Players" ]
                            Bulma.column [
                                Bulma.button.a [
                                    color.isInfo
                                    prop.text "+"
                                    prop.onClick Events.addPlayer
                                ]
                            ]
                        ]
                        model.Game.Players
                        |> List.mapi (fun i p ->
                            Bulma.input.text [
                                text.hasTextCentered
                                prop.placeholder p.Name
                                prop.custom ("index", i)
                                prop.onChange (Events.handleInput i)
                            ])
                        |> Fable.React.Helpers.ofList
                        Bulma.button.a [
                            color.isInfo
                            prop.text "Start"
                            prop.onClick (fun _ -> dispatch GameSettingsSubmit)
                        ]
                    ]
                ]
            ]
        ]

    let player (p: Player) (dispatch: Msg -> unit) =
        let filledList =
            match (Player.getCurrentLeg p).Records with
            | [] -> [ " "; " "; " " ]
            | r ->
                match r.Length % 3 with
                | 0 ->
                    r
                    |> List.take 3
                    |> List.map (fun (c, n) -> $"%c{c}%d{n}")
                | c ->
                    (List.replicate ((-) 3 c) " "
                    @ (r
                       |> List.take c
                       |> List.map (fun (c, n) -> $"%c{c}%d{n}")))
                    |> List.rev

        Bulma.container [
            prop.children [
                Bulma.columns [
                    Bulma.column [
                        column.is3
                        prop.children [
                            Bulma.text.span (sprintf "\u2300")
                        ]
                    ]
                    Bulma.column [
                        column.is6
                        text.hasTextCentered
                        prop.children [ Bulma.text.span p.Name ]
                    ]
                    Bulma.column [
                        column.is3
                        prop.children [
                            Bulma.text.span p.Legs.Head.CurrentScore
                        ]
                    ]
                ]
                Bulma.columns [
                    filledList
                    |> List.map (fun s ->
                        Bulma.column [
                            column.is4
                            prop.children [
                                Bulma.tag [
                                    prop.style [
                                        style.width (length.percent 100)
                                        style.marginBottom 5
                                    ]
                                    tag.isLarge
                                    tag.isRounded
                                    color.hasBackgroundGreyLight
                                    prop.text $"%s{s}"
                                ]
                            ]
                        ])
                    |> Fable.React.Helpers.ofList
                ]
            ]
        ]

    let playGame (model: Model) (dispatch: Msg -> unit) =
        Bulma.container [
            prop.children [
                Bulma.box [
                    prop.children [
                        Bulma.title [
                            text.hasTextCentered
                            color.hasTextBlack
                            prop.text "LetsDarts"
                        ]
                        Bulma.columns [
                            Bulma.column [
                                text.hasTextCentered
                                prop.text $"Mode: %d{model.Game.Mode}"
                            ]
                            Bulma.column [
                                text.hasTextCentered
                                prop.text $"Legs: %d{model.Game.Legs}"
                            ]
                            Bulma.column [
                                text.hasTextCentered
                                prop.text $"D/I: {model.Game.DoubleIn}"
                            ]
                            Bulma.column [
                                text.hasTextCentered
                                prop.text $"D/O: {model.Game.DoubleIn}"
                            ]
                        ]
                        model.Game.Players
                        |> List.map (fun p -> player p dispatch)
                        |> Fable.React.Helpers.ofList
#if DEBUG
                        Bulma.column [
                            Bulma.button.a [
                                color.isInfo
                                prop.text "Checkout"
                                prop.onClick (fun _ -> dispatch Undo)
                            ]
                        ]
#endif
                        ]
                ]
            ]
        ]

    let view (model: Model) (dispatch: Msg -> unit) =
        Bulma.hero [
            hero.isFullHeight
            color.isPrimary
            prop.style [
                style.backgroundSize "cover"
                style.backgroundImageUrl "https://unsplash.it/1200/900?random"
                style.backgroundPosition "no-repeat center center fixed"
            ]
            prop.children [
                Bulma.heroHead [
                    Bulma.navbar [
                        Bulma.container [ navBrand ]
                    ]
                ]
                Bulma.heroBody [
                    Bulma.container [
                        Bulma.columns [
                            Bulma.column [
                                column.is4
                                prop.children [
                                    match model.State with
                                    | Create -> createForm model dispatch
                                    | Running -> playGame model dispatch
                                    | _ ->
                                        Bulma.box [
                                            prop.children [
                                                Html.p [ prop.textf "Finished" ]
                                            ]
                                        ]
                                ]
                            ]
                            Bulma.column [
                                column.is8
                                prop.children [
                                    dartBoard model dispatch
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]