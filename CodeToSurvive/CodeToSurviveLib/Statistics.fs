namespace CodeToSurviveLib

open System
open System.Collections.Concurrent
open Microsoft.Extensions.Logging

module Statistics =
    let private sleepTimeQueue = ConcurrentQueue<float>()
    let private tickTimeQueue = ConcurrentQueue<int64>()
    // Keep on hour of statistics
    let private queueSize = 720


    let addSleepTime sleepTime =
        match sleepTimeQueue.Count > queueSize with
        | false -> ()
        | true ->
            let mutable out = -1.0
            sleepTimeQueue.TryDequeue(&out) |> ignore

        sleepTimeQueue.Enqueue sleepTime

    let addTickTime tickTime =
        match tickTimeQueue.Count > queueSize with
        | false -> ()
        | true ->
            let mutable out = -1L
            tickTimeQueue.TryDequeue(&out) |> ignore

        tickTimeQueue.Enqueue tickTime

    let getAverageSleepTime () =
        sleepTimeQueue
        |> Seq.sum
        |> (fun sleepSum -> double sleepSum / double sleepTimeQueue.Count)

    // ----------
    // Ticks
    // ----------
    let getAverageTickTime () =
        tickTimeQueue
        |> Seq.sum
        |> (fun sleepSum -> double sleepSum / double tickTimeQueue.Count)
        |> int64

    let getMaxTickTime () = tickTimeQueue |> Seq.max
    let getTicks () = tickTimeQueue |> Seq.toList
    // Ms
    let private tickInMs = fun tT -> tT / TimeSpan.TicksPerMillisecond
    let getAverageTickTimeInMs () = getAverageTickTime () |> tickInMs
    let getMaxTickTimeInMs () = getMaxTickTime () |> tickInMs
    // Micro
    let private tickInMicro = fun tT -> tT / TimeSpan.TicksPerMicrosecond
    let getAverageTickTimeInMicro () = getAverageTickTime () |> tickInMicro
    let getMaxTickTimeInMicro () = getMaxTickTime () |> tickInMicro

    // ----------
    // Report
    // ----------

    let printReport (log: ILogger) =
        let tickInMs = getAverageTickTimeInMs ()
        let tickCount = tickTimeQueue.Count

        let tickText =
            "Average of "
            + (if tickInMs > 10 then
                   $"{tickInMs}ms"
               else
                   $"{getAverageTickTimeInMicro ()}μs")
            + $" over {tickCount} game ticks"

        log.LogDebug tickText
        let out = tickTimeQueue |> Seq.last |> tickInMicro
        log.LogDebug $"Last tick took {out}μs"
