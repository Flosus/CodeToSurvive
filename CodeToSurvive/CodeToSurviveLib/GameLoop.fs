namespace CodeToSurviveLib

open System
open System.Threading
open CodeToSurviveLib.Core.Tick
open CodeToSurviveLib.Core.GameState
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

        let try1 = (rounded - DateTime.UtcNow)

        match try1.Milliseconds < 1 with
        | false -> try1
        // Add 5 seconds in case we "go back in time"
        | true -> (rounded.AddSeconds(int interval) - DateTime.UtcNow)

    let rec _gameLoop context consumeCurrentContext shouldStop (skipTimer: bool) =
        let log = context.CreateLogger "GameLoop"

        if skipTimer then
            log.LogTrace "Skipping wait time;"
        else
            let sleepTime = getWaitTimeInMilliseconds ()
            log.LogTrace $"Waiting for next tick for {int sleepTime.Milliseconds}ms;"
            Statistics.addSleepTime sleepTime.Milliseconds
            Thread.Sleep(sleepTime)

        let startTick = DateTime.Now.Ticks
        let newContext = tick context
        Statistics.addTickTime (DateTime.Now.Ticks - startTick)
        consumeCurrentContext newContext

        match shouldStop () with
        | true -> Finished
        | false -> _gameLoop context consumeCurrentContext shouldStop skipTimer

    let gameLoop context provideCurrentState shouldStop skipTimer =
        let log = context.CreateLogger "GameLoop"

        try
            _gameLoop context provideCurrentState shouldStop skipTimer
        with e ->
            log.LogError $"Error while running gameLoop; {e}"
            Error e
