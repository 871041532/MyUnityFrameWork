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
    static string m_RelativeFilePath = "/version.json";
    public PackageState m_State = PackageState.Normal;
    VersionData m_streamingVersionData = null;
    VersionData m_persistentVersionData = null;
    VersionData m_serverVersionData = null;
    string m_serverVersionPath = "http://127.0.0.1:7888/Windows/version.json";
    string m_streamingVersionPath;
    string m_persistentVersionPath;
    string m_AssetBundlePrePath;

    public HotPatchManager()
    {
        m_streamingVersionPath = ABUtility.StreamingAssetsPath + m_RelativeFilePath;
        m_persistentVersionPath = ABUtility.persistentDataPath + m_RelativeFilePath;
        m_AssetBundlePrePath = "http://127.0.0.1:7888/" + ABUtility.PlatformName + "/";
        m_serverVersionPath = m_AssetBundlePrePath + "version.json";
        CheckVersion(null);
    }

    public void CheckVersion(Action<PackageState> call)
    {
        if (ABUtility.LoadMode != LoadModeEnum.StandaloneAB && ABUtility.LoadMode != LoadModeEnum.DeviceFullAotAB)
        {
            return;
        }
        // s1:获取本地版本
        Debug.Log("开始获取本地版本信息...");
        GetLocalVersion(()=> {
            // s2：本地资源处理
            Debug.Log(string.Format("本地版本已获取，开始准备本地资源..."));
            CheckLocalRes(()=> {
                // s3：获取服务器资源版本
                Debug.Log("本地资源处理完毕。");
                if (ABUtility.LoadMode == LoadModeEnum.StandaloneAB)
                {
                    return;
                }
                Debug.Log("开始获取服务器资源信息...");
                GetPersistentAndServerInfo(()=> {
                    // s4：RunPatch
                    Debug.Log("开始向服务器获取Path...");
                    RunPatch(()=> {
                        Debug.Log("热更处理完毕，进入游戏！");
                    });
                });
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
            Action lastOneCall = ()=>{
                GameMgr.StartCoroutine(CopyStreamAssetsToPersistent(m_streamingVersionPath, m_persistentVersionPath, okCall));
            };
            int num = 0;
            int count = m_streamingVersionData.ABMD5Dict.Count - 1;
            Action call = () => {
                if ( ++ num == count)
                {
                    lastOneCall();
                }
            };
            foreach (var item in m_streamingVersionData.ABMD5Dict)
            {
                string srcPath = ABUtility.StreamingAssetsPath + "/" + item.Value.Name;
                string descPath = ABUtility.persistentDataPath + "/" + item.Value.Name;
                if (srcPath != m_streamingVersionPath)
                {
                   GameMgr.StartCoroutine(CopyStreamAssetsToPersistent(srcPath, descPath, call));
                }
            }
        }
        else
        {
            okCall();
        }
    }

    IEnumerator CopyStreamAssetsToPersistent(string source, string dest, Action okCall)
    {
        var request = UnityWebRequest.Get(source);
        yield return request.SendWebRequest();
        byte[] results = request.downloadHandler.data;
        int idx = dest.LastIndexOf('/');
        string p1 = dest.Substring(0, idx);
        Directory.CreateDirectory(p1);
        if (File.Exists(dest))
        {
            File.Delete(dest);
        }
        FileStream fs = new FileStream(dest, FileMode.Create, FileAccess.Write);
        fs.Write(results, 0, results.Length);
        fs.Flush();
        fs.Close();
        Debug.Log(dest + "写入成功！");
        okCall();
    }

    // s3: 获取server端与persistent信息
    void GetPersistentAndServerInfo(Action okCall)
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
        GameMgr.StartCoroutine(ReadVersion(m_persistentVersionPath, (data) => {
            m_persistentVersionData = data;
            call();
        }));
        GameMgr.StartCoroutine(ReadVersion(m_serverVersionPath, (data) => {
            m_serverVersionData = data;
            call();
        }));
    }

    // s4: 对比，Patch更新或整包更新
    void  RunPatch(Action okCall)
    {
        m_State = PackageState.Error;
        if (m_serverVersionData is null)
        {
            m_State = PackageState.Error;
        }
        else
        {
            int[] pv = GetNumVersion(m_persistentVersionData.Version);
            int[] sv = GetNumVersion(m_serverVersionData.Version);
            int contrastValue = ContrastNumVersion(pv, sv);
            m_State = PackageState.Normal;
            if (contrastValue < 0)
            {
                m_State = PackageState.NeedPatch;
            }
            if (pv[0] < sv[0])
            {
                m_State = PackageState.NeedFullInstall;
            }
        }
        if (m_State == PackageState.Error)
        {
            Debug.Log("连接服务器失败");
            okCall();
        }
        else if (m_State == PackageState.Normal)
        {
            Debug.Log("已是最新版本，不需要热更。");
            okCall();
        }
        else if(m_State == PackageState.NeedFullInstall)
        {
            Debug.Log("版本差距过大，需要整包更新，不需要热更。");
            okCall();
        }
        else
        {
            // 需要热更
            Debug.Log("开始从服务器Path资源...");
            Action lastOneCall = () => {
                GameMgr.StartCoroutine(CopyStreamAssetsToPersistent(m_serverVersionPath, m_persistentVersionPath, okCall));
            };
            int num = 0;
            int count = m_serverVersionData.ABMD5Dict.Count - 1;
            Action call = () => {
                if (++num == count)
                {
                    lastOneCall();
                }
            };
            foreach (var item in m_serverVersionData.ABMD5Dict)
            {
                string srcPath = m_AssetBundlePrePath + "/" + item.Value.Name;
                string descPath = ABUtility.persistentDataPath + "/" + item.Value.Name;
                if (descPath != m_persistentVersionPath)
                {
                    GameMgr.StartCoroutine(CopyStreamAssetsToPersistent(srcPath, descPath, call));
                }
            }
        }
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
                count -= num;
            }
            else if(version1[i] > version2[i])
            {
                count += num;
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
