open Fake.Core
open Fake.IO
open Farmer
open Farmer.Builders

open Helpers

initializeContext ()

let sharedPath = Path.getFullName "src/Shared"
let serverPath = Path.getFullName "src/Server"
let clientPath = Path.getFullName "src/Client"
let letsDartsCorePath = Path.getFullName "src/LetsDartsCore"
let deployPath = Path.getFullName "deploy"

Target.create "Clean" (fun _ ->
    Shell.cleanDir deployPath
    run dotnet "fable clean --yes" clientPath // Delete *.fs.js files created by Fable
)

Target.create "InstallClient" (fun _ -> run yarn "install" ".")

Target.create "Bundle" (fun _ ->
    [ "server", dotnet $"publish -c Release -o \"{deployPath}\"" serverPath
      "client", dotnet "fable -o output -s --run yarn build" clientPath ]
    |> runParallel)

Target.create "Azure" (fun _ ->
    let web =
        webApp {
            name "LetsDarts"
            zip_deploy "deploy"
        }

    let deployment =
        arm {
            location Location.WestEurope
            add_resource web
        }

    deployment
    |> Deploy.execute "LetsDarts" Deploy.NoParameters
    |> ignore)

Target.create "Run" (fun _ ->
    run dotnet "build" sharedPath

    [ "server", dotnet "watch run" serverPath
      "client", dotnet "fable watch -o output -s --run yarn start" clientPath ]
    |> runParallel)

Target.create "RunTests" (fun _ ->
//    run dotnet "build" sharedTestsPath

    [ (*"server", dotnet "watch run" serverTestsPath
      "client", dotnet "fable watch -o output -s --run yarn test:live" clientTestsPath
      "letsdartscore", dotnet "watch run" letsDartsCoreTestsPath*) ]
    |> runParallel)

Target.create "Format" (fun _ -> run dotnet "fantomas . -r" "src")

open Fake.Core.TargetOperators

let dependencies =
    [ "Clean"
      ==> "InstallClient"
      ==> "Bundle"
      ==> "Azure"

      "Clean" ==> "InstallClient" ==> "Run"

      "InstallClient" ==> "RunTests" ]

[<EntryPoint>]
let main args = runOrDefault args