local dictName = "System.Collections.Generic.Dictionary"

function toLuaType(val)
    if val == nil then
        return nil
    end
    local res = {}
    local valAsString = tostring(val)
    if (valAsString:find(dictName)) then
        for key in luanet.each(val.Keys) do
            local entry = val[key]
            local luaEntry = toLuaType(entry)
            res[key] = luaEntry
        end
        return res
    end
    return val
end

local _pcall_bak = pcall
local abort = false
pcall = function(fun)
    local status, error = _pcall_bak(fun)
    print(tostring(status) .. "_" .. tostring(error))
    if error == "abort" then
        abort = true
        error("abort")
    end
end