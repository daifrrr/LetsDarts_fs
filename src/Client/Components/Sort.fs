namespace Client.Components

open Client.Types
open Elmish.DragAndDrop.Types
open Fable.React.Props
open Elmish.DragAndDrop

module Sort =
    let dragAndDropCategoryKey =
        "default-category"

    let dragAndDropConfig =
        { DragAndDropConfig.Empty() with
            DraggedElementStyles =
                Some [
                    Opacity 0.8
                    Position PositionOptions.Fixed
                    Cursor "grabbing"
                ]
            HoverPreviewElementStyles = Some [ Opacity 0.2 ] }

    let mappedMsg msg = DndMsg msg

    let Form (model: Model) (dispatch: Msg -> unit) =
        let dispatchDnDMsg = (mappedMsg >> dispatch)

        let dropAreaContent =
            model.DragAndDrop.ElementIdsForCategorySingleList dragAndDropCategoryKey
            |> List.collect (fun id ->
                let content =
                    Map.tryFind id model.ContentMap
                    |> Option.defaultValue (
                        Fable.React.Standard.div [] [
                            Fable.React.Helpers.str "Unable to find content"
                        ]
                    )

                Draggable.SelfHandle
                    model.DragAndDrop
                    dragAndDropCategoryKey
                    dragAndDropConfig
                    dispatchDnDMsg
                    id
                    Fable.React.Standard.div
                    [ Cursor "grap" ]
                    [ ClassName "WTFFFFFF"; Id id ]
                    [ content ])

        let dropArea =
            DropArea.DropArea
                model.DragAndDrop
                dragAndDropCategoryKey
                dragAndDropConfig
                (MouseEventHandlers.Empty())
                dispatchDnDMsg
                "player-list"
                Fable.React.Standard.div
                [ Class "player-list" ]
                dropAreaContent

        DragDropContext.Context
            model.DragAndDrop
            dispatchDnDMsg
            Fable.React.Standard.div
            [ Class "container-fluid row g-0 sort-layer" ]
            [ dropArea ]
