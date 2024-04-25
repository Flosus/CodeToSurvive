namespace CodeToSurviveDiscord.Discord

open System.Threading.Tasks
open CodeToSurviveDiscord.Config
open Discord
open Discord.WebSocket

module Example =

    let logMessage (msg: LogMessage) : Task = task { printfn $"{msg.ToString()}" }

    let getOnReady (client: DiscordSocketClient)  =
        let onReady () = task {
            do! client.SetCustomStatusAsync "Setting up the game!"
            let expectedGuildId = uint64 Settings.GuildId
            let guilds = client.Guilds
            let guild = guilds |> Seq.find (fun guild ->
                guild.Id = expectedGuildId)
            let! restUserMsg = guild.DefaultChannel.SendMessageAsync ("Woho")
            return! Task.CompletedTask
        }
        onReady

    let tryIt () =
        let client = new DiscordSocketClient()

        task {
            printfn "Logging in"
            do! client.LoginAsync(TokenType.Bot, Settings.DiscordToken)
            printfn "Starting"
            do! client.StartAsync()
            client.add_Log logMessage
            let onReady = getOnReady client
            client.add_Ready (fun () -> onReady ())
            do! Task.Delay -1
            ()
        }

    ()
