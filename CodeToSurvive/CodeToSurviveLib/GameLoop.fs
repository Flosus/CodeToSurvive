namespace CodeToSurvive.Lib

open System
open System.Threading
open CodeToSurvive.Lib.Core.Tick
open CodeToSurvive.Lib.Core.GameState
open Microsoft.Extensions.Logging

module GameLoop =

    let private interval = int64 5
    let private intervalInTicks = interval * TimeSpan.TicksPerSecond
    let private halfIntervalInTicks = intervalInTicks / 2L

    type RunResult =
        | Finished
        | Error of Exception

    let private getWaitTimeInMilliseconds () =
        let currentTime = DateTime.UtcNow

        let rounded =
            DateTime(((currentTime.Ticks + halfIntervalInTicks) / intervalInTicks) * intervalInTicks)

        let try1 = (rounded - DateTime.UtcNow).TotalMilliseconds

        match try1 < 1 with
        | false -> try1
        // Add 5 seconds in case we "go back in time"
        | true -> (rounded.AddSeconds(int interval) - DateTime.UtcNow).TotalMilliseconds

    let rec _gameLoop state context provideCurrentState shouldStop =
        let log = context.CreateLogger "GameLoop"
        let sleepTime = getWaitTimeInMilliseconds ()
        log.LogTrace $"Waiting for next tick for {int sleepTime}ms;"
        Statistics.addSleepTime sleepTime
        Thread.Sleep(int sleepTime)
        let startTick = DateTime.Now.Ticks
        let newState = tick state context
        Statistics.addTickTime (DateTime.Now.Ticks - startTick)
        provideCurrentState newState

        match shouldStop () with
        | true -> Finished
        | false -> _gameLoop newState context provideCurrentState shouldStop

    let rec gameLoop state context provideCurrentState shouldStop =
        let log = context.CreateLogger "GameLoop"

        try
            _gameLoop state context provideCurrentState shouldStop
        with e ->
            log.LogError $"Error while running gameLoop; {e}"
            Error e
