require("step.lua")

itemCfgs = require("Datas/Item.lua")
Datas = {
    itemCfgs = itemCfgs
}

local GameMgr = CS.GameManager.Instance
local ABMgr = GameMgr.m_ABMgr
local UIMgr = GameMgr.m_UIMgr
local GameObject = CS.UnityEngine.GameObject
local Window = CS.Window
local SportToolPanel = CS.SportToolPanel
