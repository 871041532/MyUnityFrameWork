using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.Networking;

public enum PackageState
{
    FirstOrOverInstall = 1,  // 1初次安装Present为空或覆盖安装，小于Stream版本
    NormalRun = 2,  // 2.正常使用Present大于等于Stream版本
    BigVersionRefresh = 3,  // 3.不能热更的大版本更新
}

public class HotPatchManager:IManager
{
    static string m_RelativeFilePath = "/Configs/version.json";
    public PackageState m_State = PackageState.NormalRun;

    public override void Awake()
    {
        CheckVersion(null);
    }

    public void CheckVersion(Action<PackageState> call)
    {
        string streamingVersionPath = ABUtility.StreamingAssetsPath + m_RelativeFilePath;
        string persistentVersionPath = ABUtility.persistentDataPath + m_RelativeFilePath;
        VersionData streamingVersionData = null;
        VersionData persistentVersionData = null;
        GameMgr.StartCoroutine(ReadVersion(streamingVersionPath, (data1)=>{
            streamingVersionData = data1;
            GameMgr.StartCoroutine(ReadVersion(persistentVersionPath, (data2) => {
                persistentVersionData = data2;
               

            }));
        }));
    }

    IEnumerator ReadVersion(string path, Action<VersionData> call)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);
        request.timeout = 5000;
        yield return request.SendWebRequest();
        VersionData data = null;
        try
        {
            MemoryStream stream = new MemoryStream(request.downloadHandler.data);
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(VersionData));
            data = jsonSerializer.ReadObject(stream) as VersionData;
            stream.Close();
        }
        catch (Exception)
        {
            //Debug.LogError("获取VersionData失败：" + path);
        }
        request.Dispose();
        call(data);
    }
}
