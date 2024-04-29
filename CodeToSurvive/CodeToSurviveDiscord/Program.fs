open System
open System.Threading.Tasks
open CodeToSurviveDiscord.Config
open CodeToSurviveDiscord.Discord
open CodeToSurviveDiscord
open Discord.WebSocket



let private ensurePlayers (players: SocketGuildUser seq) (guild: SocketGuild) =
    task {
        printfn "Ensuring player channels"
        let categories = guild.CategoryChannels

        for player in players do
            let playerName = player.DisplayName

            let categoryName =
                String.Format(Settings.CategoryTemplate, playerName)
            let channelExists = categories |> Seq.map (_.Name) |> Seq.contains categoryName
            if channelExists then
                let! _ = guild.CreateCategoryChannelAsync categoryName
                printfn $"Channel `{categoryName}` created"
            else
                printfn $"Channel `{categoryName}` already exists"
    }


let private step (client: DiscordSocketClient) (guild: SocketGuild) (players: SocketGuildUser seq): Task =
    task {
        let myId = 203547302009241600UL
        let! user = client.GetUserAsync myId
        return! Task.CompletedTask
    }


[<TailCall>]
let rec private _loop (client: DiscordSocketClient) (guild: SocketGuild) (players: SocketGuildUser seq) =
    async {
        do! step client guild players |> Async.AwaitTask
        do! Task.Delay -1 |> Async.AwaitTask
        return! _loop client guild players
    }

let private clientLoop (client: DiscordSocketClient) : Task =
    task {
        let guild = DiscordUtil.getGuild client
        printfn $"Found discord server; name={guild.Name}"
        let! players = DiscordUtil.getPlayers guild
        printfn $"Found {players |> Seq.length} players"
        do! ensurePlayers players guild
        printfn $"{guild.Users.Count} total Users on guild"
        return! _loop client guild players
    }


[<EntryPoint>]
let main (args: string []) =
    printfn "Starting Discord Bot"
    Startup.init args

    let task = Startup.start clientLoop
    task.Wait()
    0
