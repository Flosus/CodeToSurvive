-----------------------------------------------------------------------------------
--- Communication-Api
-----------------------------------------------------------------------------------

---Lets the character say something. Other players can hear this.
---@param words string
function say(words)

end

---Lets the character yell something. Other players can hear this.
---@param words string
function yell(words)

end

---Tell one other character something
---@param character string The name of an other character on the current map
---@param words string
function whisper(character, words)

end

---Returns the spoken and whispered text from the previous tick
---TODO rename this function
---@return string[]
function getLastWord()
    return {}
end
