-----------------------------------------------------------------------------------
---# Job-Api
-----------------------------------------------------------------------------------
---The resulting job the character should do. All Scripts should return a job.
---@class Job 
---@field Name string The name of the job, your character should do
---@field Parameter any Parameters for the job. WIP, this will change and the definition will be improved
local Job = {}

---@return Job
function Job:new (o)
    o = o or {}   -- create object if user does not provide one
    setmetatable(o, self)
    self.__index = self
    return o
end

---@return Job
function getIdleJob()
    local idleJob = Job:new(nil)
    idleJob.Name = "idle"
    return idleJob
end
