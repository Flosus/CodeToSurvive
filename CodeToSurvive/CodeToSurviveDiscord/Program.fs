open System
open System.Threading
open System.Threading.Tasks
open CodeToSurviveDiscord.Discord
open CodeToSurviveDiscord
open FSharp.Configuration



[<EntryPoint>]
let main args =
    printfn "Starting Discord Bot"
    Startup.init args
    
    let task = Example.tryIt ()
    task.Wait()
    0





