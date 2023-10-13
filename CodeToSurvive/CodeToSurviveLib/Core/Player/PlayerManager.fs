namespace CodeToSurviveLib.Core.Player

module PlayerManager =
    
    type Player = string
    
    let mutable players: Player[] = [||]
    
    let addPlayer newPlayer =
        match players |> Array.contains newPlayer with
        | true -> ()
        | false -> players <- players |> Array.append [|newPlayer|]
