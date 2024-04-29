namespace CodeToSurviveDiscord.Discord

open System.Threading.Tasks
open CodeToSurviveDiscord.Config
open Discord.WebSocket

module DiscordUtil =

    let sendMessage (channel: SocketTextChannel) msg = channel.SendMessageAsync msg

    let setStatus (client: DiscordSocketClient) msg = client.SetCustomStatusAsync msg

    let getGuild (client: DiscordSocketClient) =
        let expectedGuildId = uint64 Settings.GuildId
        let guilds = client.Guilds

        let guild =
            guilds
            |> Seq.find (fun guild -> guild.Id = expectedGuildId)

        guild

    let isUserPlayer (usr: SocketGuildUser) : bool =
        usr.Roles
        |> Seq.filter (fun role -> role.Name = Settings.PlayerRole)
        |> Seq.isEmpty
        |> not

    let getPlayers (guild: SocketGuild) =
        task {
            printfn "Downloading users"
            do! guild.DownloadUsersAsync ()
            printfn "Users downloaded"
            let users = guild.Users

            let player = users |> Seq.filter (fun usr -> usr |> isUserPlayer)
            return player
        }
