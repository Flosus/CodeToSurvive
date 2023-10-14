-----------------------------------------------------------------------------------
---# Action-Api
-----------------------------------------------------------------------------------
---The resulting Action the character should do. All Scripts should return a Action.
---@class Action 
---@field Name string The name of the Action, your character should do
---@field Parameter any Parameters for the Action. WIP, this will change and the definition will be improved
local Action = {}

---@return Action
function Action:new (o)
    o = o or {}   -- create object if user does not provide one
    setmetatable(o, self)
    self.__index = self
    return o
end

---@return Action
function getIdleAction()
    local idleAction = Action:new(nil)
    idleAction.Name = "Idle"
    return idleAction
end

---@return Action
function getContinueAction()
    local idleAction = Action:new(nil)
    idleAction.Name = "Continue"
    return idleAction
end
