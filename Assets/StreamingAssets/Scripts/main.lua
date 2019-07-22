require("step.lua")

itemCfgs = require("Datas/Item.lua")
Datas = {
    itemCfgs = itemCfgs
}

local GM = CS.GameManager.Instance
local ABM = GM.m_abMgr
local GameObject = CS.UnityEngine.GameObject
local prefabPath = "Assets/GameData/Prefabs/c1.prefab"
-- local asset = ABM:LoadAssetGameObject(prefabPath)
-- GameObject.Instantiate(asset)

ABM:LoadAssetAsync(prefabPath, function(item)
	GameObject.Instantiate(item:GetGameObject())
end)

ABM:LoadAssetAsync(prefabPath, function(item)
	GameObject.Instantiate(item:GetGameObject())
end)