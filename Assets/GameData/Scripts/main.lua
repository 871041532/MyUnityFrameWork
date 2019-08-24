itemCfgs = require("Datas/Item.lua")
Datas = {
    itemCfgs = itemCfgs
}

GameMgr = CS.GameManager.Instance
ABMgr = GameMgr.m_ABMgr
UIMgr = GameMgr.m_UIMgr
CallMgr = GameMgr.m_CallMgr
GameObject = CS.UnityEngine.GameObject
Window = CS.Window
SportToolPanel = CS.SportToolPanel
Class = require("ClassUtil.lua")

local menu = require("wins/menu.lua").new()
menu:show()