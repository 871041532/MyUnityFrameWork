
local StateBase = class("StateBase")

function StateBase:ctor(machine, enumType, param)
    self.enumType = enumType
    self.machine = machine
    self.param = param
    self:_OnInit()
end

function StateBase:Enter()
    self:_OnEnter()
end

function StateBase:Exit()
    self:_OnExit()
end


function StateBase:TriggerEvent(eventName, data)
    self:_OnTriggerEvent(eventName, data)
end

function StateBase:GotoState(enumType)
    self.machine:GotoState(enumType)
end

---------------------------------------- 模板方法 --------------------------------------------------
function StateBase:_OnInit()

end

function StateBase:_OnEnter()

end

function StateBase:_OnExit()

end

function StateBase:_OnTriggerEvent(eventName, data)

end
----------------------------------------------------------------------------------------------------

-- 状态机
local StateMechine = class("StateMechine")

function StateMechine:ctor(param)
    self.param = param
    self.states = {}
    self.curState = nil
    self.translationFuncDict = {}
end

-- 外部接口：添加一个状态，stateClass可空
function StateMechine:AddState(enum, stateClass)
    if not stateClass then
        self.states[enum] = StateBase.New(self, enum, self.param)
    else
        self.states[enum] = stateClass.New(self, enum, self.param)
    end
end

-- 获取当前状态对象
function StateMechine:GetCurState()
    return self.curState
end

-- 获取当前状态枚举值
function StateMechine:GetCurStateEnum()
    return self.curState and self.curState.enumType
end

-- 转移到下个状态
function StateMechine:GotoState(enumType)
    -- 老状态先退出
    if self.curState then
        self.curState:Exit()
    end
    -- 触发状态转移函数
    if self.curState then
        local curType = self.curState.enumType
        local func = self.translationFuncDict[curType] and self.translationFuncDict[curType][enumType]
        if func then
            func()
        end
    end
    -- 进入下个状态
    local nextState = self.states[enumType]
    self.curState = nextState
    nextState:Enter()
end

-- 对当前状态触发事件
function StateMechine:TriggerEvent(eventName, data)
    if self.curState then
        self.curState:TriggerEvent(eventName, data)
    end
end

-- 注册状态转移时的触发函数
function StateMechine:RegisterTranslationFunc(srcType, targetType, func)
    local srcFuncMap = self.translationFuncDict[srcType]
    if not srcFuncMap then
        srcFuncMap = {}
        self.translationFuncDict[srcType] = srcFuncMap
    end
    srcFuncMap[targetType] = func
end

local StateMechineLib = {}

-- StateClass 供外部继承
StateMechineLib.StateClass = StateBase
-- 外部接口：新建一个状态机对象
StateMechineLib.CreateMechine = function(param)
    return StateMechine.New(param)
end

return StateMechineLib