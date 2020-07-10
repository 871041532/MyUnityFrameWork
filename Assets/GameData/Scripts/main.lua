
require("FrameWork/Init/LuaGlobal.lua")

local uiManager =  require("FrameWork/UI/UIManager.lua").New()

local mainBehavior = require("FrameWork/Init/MainBehavior.lua")
mainBehavior.New({uiManager})

-- local win = uiManager:CreateWindow(Enum.WinCfg.demo)
-- win:CreateUIByPath("Assets/GameData/UI/prefabs/ConfirmSure.prefab", require("FrameWork/UI/View.lua"))
--win:Destroy()
GameManager.m_UIMgr:SwitchSingleWindow("sportTool")



