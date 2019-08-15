using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.Networking;

public enum PackageState
{
    Error, // 错误
    Normal,  // 不需要更新 （Patch版等于Server）
    NeedPatch, // 需要Patch （Patch版本低于Server）
    NeedFullInstall, // 需要整包更新 （Server有大版本更新）
    FirstOrOverInstall,  // 需要覆盖（初次安装Present为空或覆盖安装，小于Stream版本）
}

public class HotPatchManager:IManager
{
    static string m_RelativeFilePath = "/Configs/version.json";
    public PackageState m_State = PackageState.Normal;
    VersionData m_streamingVersionData = null;
    VersionData m_persistentVersionData = null;
    VersionData m_serverVersionData = null;
    string m_serverVersionPath = "http://127.0.0.1:7888/Windows/version.json";
    string m_streamingVersionPath;
    string m_persistentVersionPath;

    public HotPatchManager()
    {
        m_streamingVersionPath = ABUtility.StreamingAssetsPath + m_RelativeFilePath;
        m_persistentVersionPath = ABUtility.persistentDataPath + m_RelativeFilePath;
        CheckVersion(null);
    }

    public void CheckVersion(Action<PackageState> call)
    {
        // s1
        Debug.Log("开始获取本地版本信息...");
        GetLocalVersion(()=> {
            Debug.Log(string.Format("本地版本已获取，开始准备本地资源..."));
            CheckLocalRes(()=> {
                Debug.Log("本地信息处理完毕。");
            });
        });
    }

    // s1：获取本地版本文件
    void GetLocalVersion(Action okCall)
    {
        var count = 2;
        var num = 0;
        Action call = () =>
        {
            if (++num == count)
            {
                okCall();
            }
        };
        GameMgr.StartCoroutine(ReadVersion(m_streamingVersionPath, (data) => {
            m_streamingVersionData = data;
            call();
        }));
        GameMgr.StartCoroutine(ReadVersion(m_persistentVersionPath, (data) => {
            m_persistentVersionData = data;
            call();
        }));
    }

     // s2:检测本地版本，可能stream复制到Present
     void CheckLocalRes(Action okCall)
    {
        m_State = PackageState.Error;
        if (m_persistentVersionData is null)
        {
            m_State = PackageState.FirstOrOverInstall;
        }
        else
        {
            int[] pv = GetNumVersion(m_persistentVersionData.Version);
            int[] sv = GetNumVersion(m_streamingVersionData.Version);
            int contrastValue = ContrastNumVersion(pv, sv);
            if (contrastValue < 0)
            {
                m_State = PackageState.FirstOrOverInstall;
            }
        }
        if (m_State == PackageState.FirstOrOverInstall)
        {
            Debug.Log("首次安装开始解压资源...");
        }
        else
        {
            okCall();
        }
    }

    // s3: 获取server端与persistent信息
    void GetPersistentAndServerInfo()
    {

    }

    // s4: 对比，Patch更新或整包更新
    void  RunPatch()
    {

    }


    // <0 小于；0等于；>0大于
    int ContrastNumVersion(int[] version1, int[] version2)
    {
        int len = version1.Length;
        int count = 0;
        int num = 256;
        for (int i = 0; i < len; i++)
        {
            if (version1[i] < version2[i])
            {
                count += num;
            }
            else if(version1[i] > version2[i])
            {
                count -= num;
            }
            num /= 2;
        }
        return count;
    }

    int[] GetNumVersion(string versionString)
    {
        string[] array = versionString.Split('.');
        int[] returnData = new int[3] {Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2])};
        return returnData;
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
        }
        request.Dispose();
        call(data);
    }
}
