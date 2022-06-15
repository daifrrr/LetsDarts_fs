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

    let handleInput index name =
        mapEvent.Trigger(Client.Types.Msg.ChangePlayername(index, name))
