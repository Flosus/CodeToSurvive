namespace CodeToSurviveDiscord.Discord

open System.Threading.Tasks
open CodeToSurviveDiscord.Config
open Discord
open Discord.WebSocket

module Startup =

    let private logMessage (msg: LogMessage): Task = task { printfn $"{msg.ToString()}" }
    let private getOnReady (client: DiscordSocketClient) (onReadyExtern: DiscordSocketClient -> Task)  =
        let onReady () = task {
            do! DiscordUtil.setStatus client "Setting up!"
            printfn "Calling onReadyExtern"
            let _ = Task.Run  (fun () -> onReadyExtern client )
            return! Task.CompletedTask
        }
        onReady
        
    let rec private getArgument (args: string[]) (name: string) : string option =
        if args.Length < 2 then None
        elif args[0] = name then Some(args[1])
        else getArgument args[1..] name
        
    let private updateSetting args name pred =
        match getArgument args name with
        | None -> ()
        | Some token ->
            printfn $"Overriding {name} from arguments"
            pred token
    
    let private handleGuildScheduledEvents (_: DiscordSocketClient) (_: SocketGuildEvent): Task =
        task {
            printfn "handleGuildScheduledEvents called"
        }
    
    let private handleInviteCreated (_: DiscordSocketClient) (_: SocketInvite): Task =
        task {
            printfn "handleInviteCreated called"
        }
    
    let private handleMessageReceived (_: DiscordSocketClient) (_: SocketMessage): Task =
        task {
            printfn "handleMessageReceived called"
        }
    
    let private handlePresenceUpdated (_: DiscordSocketClient) (usr: SocketUser) (pres1:SocketPresence) (pres2:SocketPresence): Task =
        task {
            printfn $"handlePresenceUpdated called; usr={usr}; pres1={pres1}; pres2={pres2}"
        }

    let init (args: string[]) : unit =
        printfn $"Loading config: {Settings.ConfigFileName}"
        updateSetting args "DiscordToken" (fun token -> Settings.DiscordToken <- token)
        updateSetting args "GuildId" (fun id -> Settings.GuildId <- id)

    let start onReady =
        let config = DiscordSocketConfig ()
        config.GatewayIntents <- GatewayIntents.All
        let client = new DiscordSocketClient(config)
    
        let withClient fn =
            fn client

        task {
            client.add_Log logMessage
            handleGuildScheduledEvents |> withClient |> client.add_GuildScheduledEventCreated
            handleInviteCreated |> withClient |> client.add_InviteCreated 
            handleMessageReceived |> withClient |> client.add_MessageReceived
            handlePresenceUpdated |> withClient |> client.add_PresenceUpdated
            client.add_Ready (fun () -> getOnReady client onReady ())
            printfn "Logging in"
            do! client.LoginAsync(TokenType.Bot, Settings.DiscordToken)
            printfn "Starting"
            do! client.StartAsync()
            do! Task.Delay -1
            ()
        }