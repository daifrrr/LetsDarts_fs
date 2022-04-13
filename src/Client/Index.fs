module Index

open Elmish
open Fable.Remoting.Client
open Shared


type Model = { Todos: Todo list; Input: string; }

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo
    | MyEvent of Browser.Types.EventTarget

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

let init () : Model * Cmd<Msg> =
    let model = { Todos = []; Input = "" }

    let cmd = Cmd.OfAsync.perform todosApi.getTodos () GotTodos

    model, cmd

///External event wrapping a message
let mapEvent = Event<Msg>()
///Subscription on external events to bring them into Elmish message queue
let mapEventSubscription initial =
    let sub dispatch =
        let msgSender msg =
            msg
            |> dispatch

        mapEvent.Publish.Add(msgSender)

    Cmd.ofSub sub


let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | GotTodos todos -> { model with Todos = todos }, Cmd.none
    | SetInput value -> { model with Input = value }, Cmd.none
    | AddTodo ->
        let todo = Todo.create model.Input

        let cmd = Cmd.OfAsync.perform todosApi.addTodo todo AddedTodo

        { model with Input = "" }, cmd
    | AddedTodo todo -> { model with Todos = model.Todos @ [ todo ] }, Cmd.none
    | MyEvent me -> //Browser.Dom.console.log(me)
                    model, Cmd.none

open Feliz
open Feliz.Bulma

type transform with
    static member inline matrix(x1: float, y1: float, z1: float, x2: float, y2: float, z2: float) =
            Interop.svgAttribute "transform" (
                "matrix(" +
                (unbox<string> x1) + "," +
                (unbox<string> y1) + "," +
                (unbox<string> z1) + "," +
                (unbox<string> x2) + "," +
                (unbox<string> y2) + "," +
                (unbox<string> z2) + ")"
            )

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

let handleClick (ev: Browser.Types.Event) =
    let evm = ev |> unbox<Browser.Types.MouseEvent>
    let id = evm.target |> unbox<Browser.Types.Element>
    Browser.Dom.console.log(id.getAttribute("id"))
    mapEvent.Trigger (Msg.MyEvent(evm.target))

let sections startAngle endAngle =
    seq { startAngle .. 36. .. endAngle }
    |> Seq.map (fun v ->
            Svg.g [
                svg.id "g19"
                svg.transform.rotate v
                svg.children [
                    Svg.use' [ svg.id "use21"; svg.href "#double"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill "#ff0000"; svg.onClick handleClick]
                    Svg.use' [ svg.id "use23"; svg.href "#outer"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill "#000000"; svg.onClick handleClick]
                    Svg.use' [ svg.id "use25"; svg.href "#triple"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill "#ff0000"; svg.onClick handleClick]
                    Svg.use' [ svg.id "use27"; svg.href "#inner"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill "#000000"; svg.onClick handleClick]
                ]
            ]
    )
    |> List.ofSeq
    |> Fable.React.Helpers.ofList


let containerBox (model: Model) (dispatch: Msg -> unit) =
    Bulma.box [
        Bulma.container [
            Svg.svg [
                svg.id "svg2"
                svg.viewBox (-250, -250, 500, 500)
                unbox("version", "1.0")
                unbox("xmlns:rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#")
                unbox("xmlns", "http://www.w3.org/2000/svg")
                unbox("xmlns:cc", "http://creativecommons.org/ns#")
                unbox("xmlnsXlink", "http://www.w3.org/1999/xlink") // Error when xmlns:xlink
                unbox("xmlns:dc", "http://purl.org/dc/elements/1.1/")
                unbox("xmlns:svg", "http://www.w3.org/2000/svg")
                svg.children [
                    Svg.defs [
                        svg.id "defs6"
                        svg.children [
                            Svg.line [ svg.id "refwire"; svg.y2 167.4; svg.y1 16.2; svg.stroke "#c0c0c0"; svg.x2 26.52; svg.x1 2.566 ]
                            Svg.path [ svg.id "SLICE"; svg.strokeWidth 0; svg.d "m 0 0 l 15.64 98.77 c -10.362 1.64 -20.918 1.64 -31.28 0 l 15.64 -98.77 z" ]
                            Svg.use' [ svg.id "double"; unbox("xlinkHref", "#SLICE"); svg.transform.scale 1.695; svg.height 500; svg.width 500; svg.y 0; svg.x 0 ]
                            Svg.use' [ svg.id "outer"; unbox("xlinkHref", "#SLICE"); svg.transform.scale 1.605; svg.height 500; svg.width 500; svg.y 0; svg.x 0 ]
                            Svg.use' [ svg.id "triple"; unbox("xlinkHref", "#SLICE"); svg.transform.scale 1.065; svg.height 500; svg.width 500; svg.y 0; svg.x 0 ]
                            Svg.use' [ svg.id "inner"; unbox("xlinkHref", "#SLICE"); svg.transform.scale 0.975; svg.height 500; svg.width 500; svg.y 0; svg.x 0 ]
                        ]
                    ]
                    Svg.g [
                        svg.id "g14"
                        transform.matrix (1.104, 0., 0., -1.104, -1.3036, -0.48743)
                        svg.children [
                            Svg.g [
                                svg.id "dartboard"
                                svg.children [
                                    sections 0. 324.
                                    sections 18. 342.
                                ]
                            ]
                        ]
                    ]
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
                    Bulma.column [
                        column.is6
                        column.isOffset3
                        prop.children [
                            Bulma.title [
                                text.hasTextCentered
                                prop.text "LetsDarts"
                            ]
                            containerBox model dispatch
                        ]
                    ]
                ]
            ]
        ]
    ]