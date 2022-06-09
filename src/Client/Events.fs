namespace Client.Events

open Elmish

[<AutoOpen>]
module Sub =
    ///External event wrapping a message
    let mapEvent = Event<Client.Types.Msg>()

    ///Subscription on external events to bring them into Elmish message queue
    let mapEventSubscription _ =
        let sub dispatch =
            let msgSender msg = msg |> dispatch

            mapEvent.Publish.Add(msgSender)

        Cmd.ofSub sub

    let handleClick (ev: Browser.Types.Event) =
        let evm = ev |> unbox<Browser.Types.MouseEvent>

        let id = evm.target |> unbox<Browser.Types.Element>
        mapEvent.Trigger(Client.Types.Msg.SendShot(id.getAttribute "id"))

    let handleInput index name =
        mapEvent.Trigger(Client.Types.Msg.ChangePlayername(index, name))
