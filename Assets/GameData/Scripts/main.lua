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
local prefabPath = 'Assets/GameData/Prefabs/c1.prefab'
UIMgr:RegisterWindow('window1', function()
	return SportToolPanel()
end)
UIMgr:RegisterWindow('window2', function()
	return SportToolPanel()
end)
local win = UIMgr:GetOrCreateWindow('window1')
win.m_OnInit = function()
	print("m_OnInit")
end
win.m_OnShow = function()
	print("m_OnShow")
end
win:Show()
win.m_OnInit = nil
win.m_OnShow = nil
UIMgr:UnRegisterWindow('window1')
UIMgr:UnRegisterWindow('window2')