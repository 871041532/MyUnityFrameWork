
require("FrameWork/Init/LuaGlobal.lua")

local uiManager =  require("FrameWork/UI/UIManager.lua").New()

local mainBehavior = require("FrameWork/Init/MainBehavior.lua")
mainBehavior.New({uiManager})

local win = uiManager:CreateWindow("Assets/GameData/UI/prefabs/MenuPanel.prefab", Enum.LayerType.UI)
--win:CreateUIByPath("Assets/GameData/UI/prefabs/LoadingPanel.prefab")
--win:Close()



