namespace CodeToSurviveLib.CharacterScript

open System.Collections.Generic
open NLua

module LuaUtil =

    let luaTableToDict (luaTable: LuaTable) typeEnsurer : Dictionary<string, obj> =
        let dict = Dictionary<string, obj>()

        luaTable.Keys
        |> Seq.cast
        |> Seq.map (fun k -> (k, typeEnsurer luaTable[k]))
        |> Seq.iter dict.Add

        dict

    let rec ensureType (input: obj) : obj =
        if input :? LuaTable then
            let luaTable = input :?> LuaTable
            luaTableToDict luaTable ensureType
        else
            input
