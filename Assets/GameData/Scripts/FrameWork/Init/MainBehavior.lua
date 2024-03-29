---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by qqqqq.
--- DateTime: 2020/5/17 12:02
---
local MainBehavior = class("MainBehavior")

function MainBehavior:ctor(comList)
    self.comList = comList or {}
    self.updateFunc = WrapPartial1(self.Update, self)
    self.destroyFunc = WrapPartial1(self.Destroy, self)
    self:Init()
end

function MainBehavior:Init()
    self:_OnInit()
    for _, v in ipairs(self.comList) do
        if v.Init then v:Init() end
    end
    CallManager:RegisterEvent("OnUpdate", self.updateFunc)
    CallManager:RegisterEvent("OnDestroy", self.destroyFunc)
end

function MainBehavior:Update()
    for _, v in ipairs(self.comList) do
        if v.Update then v:Update() end
    end
    self:_OnUpdate()
end

function MainBehavior:Destroy()
    for _, v in ipairs(self.comList) do
        if v.Destroy then v:Destroy() end
    end
    self:_OnDestroy()
end

-- 初始化时调用
function MainBehavior:_OnInit()
end

-- 每帧更新时调用
function MainBehavior:_OnUpdate()
end

-- 销毁时调用
function MainBehavior:_OnDestroy()
end

return MainBehavior
