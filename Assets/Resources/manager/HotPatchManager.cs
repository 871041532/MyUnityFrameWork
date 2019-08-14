using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.Networking;

public enum PackageState
{
    FirstOrOverInstall = 1,  // 1初次安装Present为空或覆盖安装小于Stream版本
    NormalRun = 2,  // 2.正常使用Present大于等于Stream版本
}

public class HotPatchManager:IManager
{
    static string m_versionFilePath = "Assets/GameData/Configs/version.json";
    public PackageState m_State = PackageState.NormalRun;

    public override void Awake()
    {
        CheckVersion(null);
    }

    public void CheckVersion(Action<bool> call)
    {
        var data = ReadVersion();
        var a = 1;
    }

    VersionData ReadVersion()
    {
        AssetItem item = GameMgr.m_ABMgr.LoadAsset(m_versionFilePath);
        var mStream = new MemoryStream(item.TextAsset.bytes);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(VersionData));
        VersionData data = jsonSerializer.ReadObject(mStream) as VersionData;
        return data;
    }
}
