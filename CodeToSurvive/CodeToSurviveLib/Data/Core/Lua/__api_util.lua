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