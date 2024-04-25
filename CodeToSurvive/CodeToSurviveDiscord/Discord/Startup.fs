namespace CodeToSurviveDiscord.Discord

open CodeToSurviveDiscord.Config

module Startup =

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

    let init (args: string[]) : unit =
        printfn $"Loading config: {Settings.ConfigFileName}"
        updateSetting args "DiscordToken" (fun token -> Settings.DiscordToken <- token)
        updateSetting args "GuildId" (fun token -> Settings.GuildId <- token)
