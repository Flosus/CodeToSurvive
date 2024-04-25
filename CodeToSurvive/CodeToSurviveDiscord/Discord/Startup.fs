namespace CodeToSurviveDiscord.Discord

open System
open System.Collections.Generic
open CodeToSurviveLib.Storage.StoragePreference
open Microsoft.Extensions.Logging
open CodeToSurviveDiscord.Config

module Startup =

    let doSomething () =
        
        printfn $"Haaaa {Settings.ConfigFileName}"


    ()