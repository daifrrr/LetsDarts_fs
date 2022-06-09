namespace Client.Components

open Client.Types
open Feliz
open Shared

[<AutoOpen>]
module Dartboard =
    let internal defs _ =
        Svg.defs [
            svg.id "defs"
            svg.children [
                Svg.path [
                    svg.id "sSLICE"
                    svg.stroke BACKGROUND
                    svg.strokeWidth 1
                    svg.d "M 0 0 L 39.108616 246.922085 A 250 250 0 0 0 -39.108616 246.922085 L 0 0 Z"
                ]
                Svg.path [
                    svg.id "dSLICE"
                    svg.stroke BACKGROUND
                    svg.strokeWidth 1
                    svg.d
                        "M 31.286893 197.537668 L 39.108616 246.922085 A 250 250 0 0 1 -39.108616 246.922085 L -31.286893 197.537668 A 200 200 0 0 0 31.286893 197.537668 Z"
                ]
                Svg.path [
                    svg.id "tSLICE"
                    svg.stroke BACKGROUND
                    svg.strokeWidth 1
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

    let internal sections (fieldValue: int, angle: float) (contrast, color) (dispatch: Msg -> unit) =
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
                    svg.fill contrast
                    svg.onClick (fun _ -> SendShot($"s{fieldValue}") |> dispatch)
                ]
                Svg.use' [
                    svg.id (string $"d{fieldValue}")
                    svg.href "#double"
                    svg.height 500
                    svg.width 500
                    svg.y 0
                    svg.x 0
                    svg.fill color
                    svg.onClick (fun _ -> SendShot($"d{fieldValue}") |> dispatch)
                ]
                Svg.use' [
                    svg.id (string $"t{fieldValue}")
                    svg.href "#triple"
                    svg.height 500
                    svg.width 500
                    svg.y 0
                    svg.x 0
                    svg.fill color
                    svg.onClick (fun _ -> SendShot($"t{fieldValue}") |> dispatch)
                ]
            ]
        ]

    let Dartboard (dispatch: Msg -> unit) =
        Html.div [
            prop.className "dartboardContainer"
            prop.children [
                Svg.svg [
                    svg.id "svg"
                    svg.viewBox (-300, -300, 600, 600)
                    unbox ("version", "1.0")
                    unbox ("xmlns:rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#")
                    unbox ("xmlns", "http://www.w3.org/2000/svg")
                    unbox ("xmlns:cc", "http://creativecommons.org/ns#")
                    unbox ("xmlnsXlink", "http://www.w3.org/1999/xlink") // Error when xmlns:xlink
                    unbox ("xmlns:dc", "http://purl.org/dc/elements/1.1/")
                    unbox ("xmlns:svg", "http://www.w3.org/2000/svg")
                    svg.children [
                        defs ()
                        Svg.g [
                            svg.id "gTransform"
                            svg.transform.matrix (1, 0, 0, -1, -1, 0)
                            svg.children [
                                Svg.g [
                                    // TODO: Seq<a'> Seq<b'> must have same length, check!
                                    svg.id "dartboard"
                                    svg.children [
                                        // dartboard background
                                        Svg.rect [
                                            svg.id "s0"
                                            svg.x -300
                                            svg.y -300
                                            svg.width 600
                                            svg.height 600
                                            svg.stroke BACKGROUND
                                            svg.strokeWidth 1
                                            svg.fill BACKGROUND
                                            svg.onClick (fun _ -> SendShot("s0") |> dispatch)
                                        ]
                                        // dartboard background
                                        (Constants.DARTNUMBERS, seq { 0.0..18.0..342.0 })
                                        ||> Seq.map2 (fun n a -> (n, a))
                                        |> Seq.indexed
                                        |> Seq.map (fun (i, (n, a)) ->
                                            match i % 2 = 0 with
                                            | true -> sections (n, a) (BLACK, RED) dispatch
                                            | _ -> sections (n, a) (WHITE, GREEN) dispatch)
                                        |> List.ofSeq
                                        |> Fable.React.Helpers.ofList
                                        // dartboard bull
                                        Svg.circle [
                                            svg.id "s25"
                                            svg.cx 0
                                            svg.cx 0
                                            svg.r 60
                                            svg.stroke BACKGROUND
                                            svg.strokeWidth 1
                                            svg.fill GREEN
                                            svg.onClick (fun _ -> SendShot("s25") |> dispatch)
                                        ]
                                        // dartboard bull's eye
                                        Svg.circle [
                                            svg.id "d25"
                                            svg.cx 0
                                            svg.cx 0
                                            svg.r 25
                                            svg.stroke BACKGROUND
                                            svg.strokeWidth 1
                                            svg.fill RED
                                            svg.onClick (fun _ -> SendShot("d25") |> dispatch)
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        // dartboard number ring
                        (Constants.DARTNUMBERS, seq { 0.0..18.0..342.0 })
                        ||> Seq.map2 (fun n a -> (n, a))
                        |> Seq.indexed
                        |> Seq.map (fun (i, (n, a)) ->
                            Svg.g [
                                svg.id "gText"
                                svg.children [
                                    Svg.text [
                                        svg.id $"t{n}"
                                        svg.x 0
                                        svg.y 0
                                        svg.text n
                                        svg.textAnchor.middle
                                        svg.fontSize 32
                                        svg.custom ("fontFamily", "Jost, sans-serif")
                                        match a with
                                        | a when a > 90. && 270. > a ->
                                            svg.custom (
                                                "transform",
                                                $"rotate({-360. + a}, 0, 0) translate(0, -277) scale(-1, -1)"
                                            )
                                        | _ -> svg.custom ("transform", $"rotate({a}, 0, 0) translate(0, -255)")
                                        svg.textAnchor.middle
                                        svg.fill WHITE
                                        svg.onClick (fun _ -> SendShot("s0") |> dispatch)
                                    ]
                                ]
                            ])
                        |> List.ofSeq
                        |> Fable.React.Helpers.ofList
                    ]
                ]
            ]
        ]
