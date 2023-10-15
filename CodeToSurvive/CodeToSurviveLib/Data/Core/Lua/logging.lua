-----------------------------------------------------------------------------------
--- Logging-Api
-----------------------------------------------------------------------------------

---Logs a message.
---@param logMessage string
function think(logMessage)
    _api_log:think(logMessage)
end

---Logs a debug message. Filter Thonk messages later in your output
---@param debugMessage string
function thonk(debugMessage)
    _api_log:thonk(debugMessage)
end

print = think
