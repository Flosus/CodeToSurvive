namespace CodeToSurvive.Lib

open System
open System.Threading
open CodeToSurvive.Lib.Core.Tick

module GameLoop =

    let private interval = int64 5
    let private oneMillion = 1000000L
    let private intervalInMicro = interval * oneMillion

    let private getWaitTimeInMilliseconds () =
        let currentTime = DateTime.UtcNow
        let microseconds = (currentTime.Ticks / 10L) % oneMillion
        let remainder = microseconds % intervalInMicro
        let sleepDuration = if remainder = 0L then 0L else (intervalInMicro - remainder)

        let nextDateTime = currentTime.AddTicks(sleepDuration * 10L)
        (nextDateTime - DateTime.UtcNow).TotalMilliseconds

    let rec gameLoop state context provideCurrentState shouldStop =
        let sleepTime = getWaitTimeInMilliseconds ()
        Thread.Sleep(int sleepTime)
        let newState = tick state context
        provideCurrentState newState

        match shouldStop () with
        | false -> ()
        | true -> gameLoop newState context provideCurrentState shouldStop
