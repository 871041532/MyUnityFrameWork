---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by qqqqq.
--- DateTime: 2020/5/17 15:17
---
local Widget = require("FrameWork/UI/Widget.lua")
local Window = class("Window")

Window.uiCfg = nil

function Window:ctor(uim, winCfg)
    self.winCfg = winCfg
    -- layer
    local layerType = self:GetLayerType()
    local layerTrans = uim:GetLayerByType(layerType)
    self.gameObject = GameObject.Instantiate(layerTrans.gameObject, layerTrans)
    self.transform = self.gameObject.transform
    -- name
    self.gameObject.name = self:GetName()
    -- ui
    self.ui = Widget.New(self.transform)
    self:Init()
end

function Window:Init()
    local cfg = self.winCfg and self.winCfg.uiCfg or self.uiCfg
    if cfg then
        Inject.Inject(self.ui, cfg)
    end
    self:_OnInit()
end

function Window:Close()
    self:Destroy()
end

function Window:Destroy()
    self.ui:Destroy()
    self:_OnDestroy()
    GameObject.Destroy(self.gameObject)
end

function Window:GetName()
    return self.winCfg and self.winCfg.name or self:_OnGetName()
end

function Window:GetLayerType()
    return self.winCfg and self.winCfg.layerType or self:_OnGetLayerType()
end

function Window:GetReturnKeyOperation()
   return self.winCfg and self.winCfg.returnKeyOperation or self:_OnGetReturnKeyOperation()
end

------------------------- 下面是虚函数 ----------------------------
function Window:_OnInit()
end

function Window:_OnDestroy()
end

function Window:_OnGetName()
    return "namelessWindow"
end

function Window:_OnGetLayerType()
    return Enum.LayerType.UI
end

function Window:_OnGetReturnKeyOperation()
    return Enum.ReturnKeyOperation.Back
end
---------------------------------------------------------------------

return Window