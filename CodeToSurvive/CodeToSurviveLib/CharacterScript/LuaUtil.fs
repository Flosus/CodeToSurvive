namespace CodeToSurviveLib.CharacterScript

open System
open System.Collections.Generic
open NLua

module LuaUtil =

    let luaTableToDict (luaTable: LuaTable) typeEnsurer : Dictionary<string, Object> =
        let dict = Dictionary<string, Object>()

        luaTable.Keys
        |> Seq.cast
        |> Seq.map (fun k -> (k, typeEnsurer luaTable[k]))
        |> Seq.iter dict.Add

        dict

    let rec ensureType (input: Object) : Object =
        if input :? LuaTable then
            let luaTable = input :?> LuaTable
            luaTableToDict luaTable ensureType
        else
            input
