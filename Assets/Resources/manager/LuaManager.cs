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
    public string m_PrePath = "Assets/GameData/Scripts";

    public override void Awake() {
        m_luaEnv = new LuaEnv();
        m_luaEnv.AddLoader(MyLuaLoader);
    }
    
    public override void OnPatched()
    {
        m_luaEnv.DoString("require 'main.lua'");
        //m_cfgs = m_luaEnv.Global.Get<Cfgs>("Datas");
        //m_luaEnv.DoString(@"
        //    local GameMgr = CS.GameManager.Instance
        //    local ABMgr = GameMgr.m_ABMgr
        //    local UIMgr = GameMgr.m_UIMgr
        //    local GameObject = CS.UnityEngine.GameObject
        //    local Window = CS.Window
        //    local prefabPath = 'Assets/GameData/Prefabs/c1.prefab'
        //    UIMgr:RegisterWindow('window1', function() 
        //        return Window()
        //    end)
        //    local win = UIMgr:GetOrCreateWindow('window1')
        //    win:Show()
        //    UIMgr:UnRegisterWindow('window1');
        //");
    }

    // 自定义lua加载
    byte[] MyLuaLoader(ref string filepath)
    {
        byte[] data;
        if (ABUtility.LoadMode == LoadModeEnum.EditorOrigin)
        {
            string path = $"{m_PrePath}/{filepath}";
            data = File.ReadAllBytes(path);
        }
        else
        {
            string path = $"{m_PrePath}/{filepath}.txt";
            AssetItem item = GameMgr.m_ABMgr.LoadAsset(path);
            data = item.TextAsset.bytes;
        }
        return data;
    }
    
    public override void OnDestroy()
    {
        m_luaEnv.Dispose();
    }
}