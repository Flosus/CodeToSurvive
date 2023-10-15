-----------------------------------------------------------------------------------
---# Memory-Api
-----------------------------------------------------------------------------------
---You can read and write to the memory of the character.
---@class Memory
Memory = {}

---@return (string*string)[]
function Memory:getKnowledge()
    return _api_memory:getKnowledge()
end

function Memory:getMemory()
    return _api_memory:getMemory()
end

function Memory:getMemoryKeys()
    return _api_memory:getMemoryKeys()
end

---@param key string
function Memory:getMemoryValue(key)
    return _api_memory:getMemoryValue(key)
end

---@param key string
function Memory:setMemory(key, value)
    return _api_memory:setMemory(key, value)
end
