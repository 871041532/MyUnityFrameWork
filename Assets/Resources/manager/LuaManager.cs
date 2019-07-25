using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System.IO;
using UnityEngine.Networking;

[CSharpCallLua]
public interface ItemCfg
{
     int id { get; set; }
     string name { get; set; }
}

[CSharpCallLua]
public interface Cfgs
{
    Dictionary<int, ItemCfg> itemCfgs { get; set; }
}

public class LuaManager : IManager
{
    private LuaEnv m_luaEnv;
    public Cfgs m_cfgs;

    public override void Awake() {
        m_luaEnv = new LuaEnv();
        m_luaEnv.AddLoader(MyLuaLoader);
    }
    public override void Start()
    {
        //m_luaEnv.DoString("require 'main.lua'");
        //m_cfgs = m_luaEnv.Global.Get<Cfgs>("Datas");
        //m_luaEnv.DoString(@"
        //    local GM = CS.GameManager.Instance
        //    local ABM = GM.m_ABMgr
        //    local GameObject = CS.UnityEngine.GameObject
        //    local prefabPath = 'Assets/GameData/Prefabs/c1.prefab'
        //    local asset = ABM:LoadAsset(prefabPath)
        //    local asset_object = asset.GameObject
        //    GameObject.Instantiate(asset_object)
        //    local asset2 = ABM:LoadAsset('Assets/GameData/Prefabs/c1.prefab')
        //    local asset_object2 = asset2.GameObject
        //    GameObject.Instantiate(asset_object2)
        //    ABM:UnloadAsset(asset);
        //    ABM:UnloadAsset(asset2);
        //    ABM:LoadAssetAsync(prefabPath, function(item)
	       //     GameObject.Instantiate(item.GameObject)
        //    end)
        //    ABM:LoadAssetAsync(prefabPath, function(item)
	       //     GameObject.Instantiate(item.GameObject)
        //    end)
        //");
    }

    // 自定义lua加载
    byte[] MyLuaLoader(ref string filepath)
    {
        string path = Path.Combine(ABManager.CfgstreamingAssets, "Scripts", filepath);
        byte[] data = File.ReadAllBytes(path);
        return data;
    }


    public override void OnDestroy()
    {
        m_luaEnv.Dispose();
    }
}