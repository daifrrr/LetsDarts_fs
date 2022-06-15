module App

open Elmish
open Elmish.React

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram Client.State.init Client.State.update Client.Views.view
|> Program.withSubscription Client.Events.Sub.mapEventSubscription
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
