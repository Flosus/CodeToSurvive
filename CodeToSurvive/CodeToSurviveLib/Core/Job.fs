namespace CodeToSurvive.Lib.Core

open System
open CodeToSurvive.Lib.Core.Player

module Job =
    
    type JobName = string

    type Job =
        {
          ID: Guid
          Name: JobName
          Duration: int
          CurrentProgress: int
          IsCancelable: bool }

    let IdleJob =
        {
          ID = Guid.Empty
          Name = "Idle"
          Duration = 5
          CurrentProgress = 0
          IsCancelable = false }

    let IsJobFinished (job: Job) =
        TimeSpan.FromSeconds(job.Duration) - TimeSpan.FromSeconds(job.CurrentProgress)
        <= TimeSpan.Zero

    type PlayerTask = { Player: Player; Job: Job }
    let isPlayerTaskOpen = fun pJob -> IsJobFinished pJob.Job |> not

