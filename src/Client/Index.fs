module Index

open Elmish
open Fable.Remoting.Client
open Shared


let numbs = seq { 20; 1; 18; 4; 13; 6; 10; 15; 2; 17; 3; 19; 7; 16; 8; 11; 14; 9; 12; 5 }

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

let sections startAngle endAngle color =
    seq { startAngle .. 36. .. endAngle }
    |> Seq.map (fun v ->
            Svg.g [
                svg.id "g19"
                svg.transform.rotate v
                svg.children [
                    Svg.use' [ svg.id "use23"; svg.href "#single"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill (fst color); svg.onClick handleClick]
                    Svg.use' [ svg.id "use21"; svg.href "#double"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill (snd color); svg.onClick handleClick]
                    Svg.use' [ svg.id "use25"; svg.href "#triple"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill (snd color); svg.onClick handleClick]
                ]
            ]
    )
    |> List.ofSeq
    |> Fable.React.Helpers.ofList

let redSections (dartnumber: int, angle: float) (color: string * string) =
    Browser.Dom.console.log(angle)
    Svg.g [
        svg.id (string $"f{dartnumber}")
        svg.transform.rotate angle
        svg.children [
            Svg.use' [ svg.id (string $"s{dartnumber}"); svg.href "#single"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill (fst color); svg.onClick handleClick]
            Svg.use' [ svg.id (string $"d{dartnumber}"); svg.href "#double"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill (snd color); svg.onClick handleClick]
            Svg.use' [ svg.id (string $"t{dartnumber}"); svg.href "#triple"; svg.height 500; svg.width 500; svg.y 0; svg.x 0; svg.fill (snd color); svg.onClick handleClick]
        ]
    ]


let containerBox (model: Model) (dispatch: Msg -> unit) =
    Bulma.box [
        prop.width 100
        prop.height 100
        prop.children [
            Bulma.container [
                prop.className "dartboardContainer"
                prop.children [
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
                                    Svg.path [ svg.id "SLICE"; svg.stroke "#bbbbbb" ;svg.strokeWidth 2; svg.d "M 0 0 L 39.108616 246.922085 A 250 250 0 0 0 -39.108616 246.922085 L 0 0 Z" ]
                                    Svg.path [ svg.id "DLICE"; svg.stroke "#bbbbbb" ;svg.strokeWidth 2; svg.d "M 31.286893 197.537668 L 39.108616 246.922085 A 250 250 0 0 1 -39.108616 246.922085 L -31.286893 197.537668 A 200 200 0 0 0 31.286893 197.537668 Z" ]
                                    Svg.path [ svg.id "TLICE"; svg.stroke "#bbbbbb" ;svg.strokeWidth 2; svg.d "M 15.643447 98.768834 L 23.465169 148.153251 A 150 150 0 0 1 -23.465169 148.153251 L -15.643447 98.768834 A 100 100 0 0 0 15.643447 98.768834 Z" ]
                                    Svg.use' [ svg.id "single"; unbox("xlinkHref", "#SLICE"); svg.height 500; svg.width 500; svg.y 0; svg.x 0 ]
                                    Svg.use' [ svg.id "double"; unbox("xlinkHref", "#DLICE"); svg.height 500; svg.width 500; svg.y 0; svg.x 0 ]
                                    Svg.use' [ svg.id "triple"; unbox("xlinkHref", "#TLICE"); svg.height 500; svg.width 500; svg.y 0; svg.x 0 ]
                                ]
                            ]
                            Svg.g [
                                svg.id "g14"
                                svg.transform.matrix (1, 0, 0, -1, -1, 0)
                                svg.children [
                                    Svg.g [
                                        // TODO: Seq<a'> Seq<b'> must have same length, check!
                                        svg.id "dartboard"
                                        svg.children [
                                                (numbs, seq { 0. .. 18. .. 342. })
                                                ||> Seq.map2 (fun n a -> (n, a))
                                                |> Seq.indexed
                                                |> Seq.map(fun (i, (n, a)) -> match i % 2 = 0 with
                                                                                | true -> redSections (n, a) ("#0", "#ff0000")
                                                                                | _ -> redSections (n, a) ("#ffffff", "#00ff00"))
                                                |> List.ofSeq
                                                |> Fable.React.Helpers.ofList
                                                Svg.circle [ svg.cx 0; svg.cx 0; svg.r 50; svg.stroke "#bbbbbb"; svg.strokeWidth 2; svg.fill "#00ff00"]
                                                Svg.circle [ svg.cx 0; svg.cx 0; svg.r 25; svg.stroke "#bbbbbb"; svg.strokeWidth 2; svg.fill "#ff0000"]
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
                        column.is10
                        column.isOffset1
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