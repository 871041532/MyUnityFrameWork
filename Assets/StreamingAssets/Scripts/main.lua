require("step.lua")

itemCfgs = require("Datas/Item.lua")
Datas = {
    itemCfgs = itemCfgs
}

local GM = CS.GameManager.Instance
local ABM = GM.m_abMgr
local GameObject = CS.UnityEngine.GameObject
local asset = ABM:LoadAssetGameObject("Assets/Charactar/c1.prefab")
print(asset)
GameObject.Instantiate(asset)