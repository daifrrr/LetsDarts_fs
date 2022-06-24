namespace Client.Components

open Client.Types
open Elmish.DragAndDrop.Types
open Fable.React.Props
open Feliz
open Shared
open Elmish.DragAndDrop

module Sort =

    let mappedMsg msg = DndMsg msg

    let dragAndDropCategoryKey =
        "default-category"

    let dragAndDropConfig =
        { DragAndDropConfig.Empty() with
            DraggedElementStyles =
                Some [
                    MarginLeft -130.
                    MarginTop -50.
                    Position PositionOptions.Fixed
                    Cursor "grabbing"
                    Background "#00ffff"
                ]
            HoverPreviewElementStyles = Some [ Opacity 0.2 ] }

    let Form (model: Model) (dispatch: Msg -> unit) =
        let rec moveDownAt (list: Player list) (index: int) : Player list =
            match list, index with
            | _, -1 -> list
            | h1 :: h2 :: t, 0 -> h2 :: h1 :: t
            | h :: t, index -> h :: moveDownAt t (index - 1)
            | [], _ -> list

        let moveUpAt (list: Player list) (index: int) : Player list = moveDownAt list (index - 1)

        let up =
            model.Game |> Game.getPlayers |> moveUpAt

        let down =
            model.Game |> Game.getPlayers |> moveDownAt

        (*let dropAreaContent =
            model.DragAndDrop.ElementIdsForCategorySingleList dragAndDropCategoryKey
            |> List.collect (fun rootElementId ->
                let contentKey = Map.tryFind rootElementId model.ContentMap
                let content = match contentKey with
                              | Some ck -> ck
                              | None -> "Unknown"

        DragDropContext.context
            model.DragAndDrop
            (mappedMsg >> dispatch)
            Fable.React.Standard.div [] [
                dropAreaContent
            ]*)

        Html.div [
            prop.className "container-fluid row g-0 sort-layer"
            prop.children [
                model.Game
                |> Game.getPlayers
                |> List.mapi (fun i p ->
                    DragHandle.Handle
                        model.DragAndDrop
                        dragAndDropCategoryKey
                        $"player-{i}"
                        (mappedMsg >> dispatch)
                        Fable.React.Standard.div
                        [ Id $"player-{i}"
                          Class "player ld-input ld-player-name-input" ]
                        [ Fable.React.Standard.span [ Class "player-name" ] [
                              Fable.React.Helpers.str p.Name
                          ]
                          Fable.React.Standard.span [ Class "sort-icon" ] [
                              Fable.React.Helpers.str "\u2630"
                          ] ])
                |> DropArea.DropArea
                    model.DragAndDrop
                    dragAndDropCategoryKey
                    dragAndDropConfig
                    (MouseEventHandlers.Empty())
                    (mappedMsg >> dispatch)
                    "drop-area"
                    Fable.React.Standard.div
                    [ Class "player-list" ]
                Html.div [
                    prop.className "ld-button ld-button-green btn-game-start"
                    prop.text "Start"
                    prop.onClick (fun _ -> dispatch SubmitGameSettings)
                ]
            ]
        ]
