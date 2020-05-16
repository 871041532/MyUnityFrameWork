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
class = require("ClassUtil.lua")
Class = class

local logIdx = 1
log = function(str)
	str = string.format("log%d：%s", logIdx, str)
	CS.UnityEngine.Debug.Log(str)
	logIdx = logIdx + 1
end


local menu = require("wins/menu.lua").new()
menu:show()
----require("JobTest.lua")