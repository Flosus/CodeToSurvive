-----------------------------------------------------------------------------------
--- Communication-Api
-----------------------------------------------------------------------------------

---Lets the character say something. Other players can hear this.
---@param words string
function say(words)
    _api_communication:say(words)
end

---Lets the character yell something. Other players can hear this.
---@param words string
function yell(words)
    _api_communication:yell(words)
end

---Tell one other character something
---@param character string The name of an other character on the current map
---@param words string
function whisper(character, words)
    _api_communication:whisper(character, words)
end

---Returns the spoken and whispered text from the previous tick
---TODO rename this function
---@return string[]
function getLastWords()
    return _api_communication:getLastWords()
end
