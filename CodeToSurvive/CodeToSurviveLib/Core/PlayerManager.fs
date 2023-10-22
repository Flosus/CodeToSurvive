namespace CodeToSurviveLib.Core.Player

open CodeToSurviveLib.Core.Domain

module PlayerManager =

    let mutable players: Player[] = [||]

    let addPlayer newPlayer =
        match players |> Array.contains newPlayer with
        | true -> ()
        | false -> players <- players |> Array.append [| newPlayer |]
