require("step.lua")

itemCfgs = require("Datas/Item.lua")
Datas = {
    itemCfgs = itemCfgs
}

local GM = CS.GameManager.Instance
local ABM = GM.m_abMgr
local GameObject = CS.UnityEngine.GameObject
local prefabPath = "Assets/Charactar/c1.prefab"
-- local asset = ABM:LoadAssetGameObject(prefabPath)
-- GameObject.Instantiate(asset)

ABM:LoadAssetGameObjectAsync(prefabPath, function(asset_obj)
	GameObject.Instantiate(asset_obj)
end)

ABM:LoadAssetGameObjectAsync(prefabPath, function(asset_obj)
	GameObject.Instantiate(asset_obj)
end)