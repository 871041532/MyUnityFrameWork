using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Json;
using Boo.Lang;
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
    public static string ErrorInfo;
    static readonly string m_RelativeFilePath = "/version.json";
    public PackageState m_State = PackageState.Normal;
    VersionData m_streamingVersionData;
    VersionData m_persistentVersionData;
    VersionData m_serverVersionData;
    string m_serverVersionPath;
    string m_streamingVersionPath;
    string m_persistentReadVersionPath;
    string m_AssetBundlePrePath;
    private string m_persistentWriteVersionPath;

    public HotPatchManager(Action endCall)
    {
        m_streamingVersionPath = ABUtility.StreamingAssetsURLPath + m_RelativeFilePath;
        m_persistentReadVersionPath = ABUtility.PersistentDataURLPath + m_RelativeFilePath;
        m_persistentWriteVersionPath = ABUtility.PersistentDataFilePath  + m_RelativeFilePath;
        CheckVersion(endCall);
    }

    void SetServerPath(string packageName)
    {
        m_AssetBundlePrePath = $"http://10.231.10.87:7888/{packageName}/{ABUtility.PlatformName}/";
        m_serverVersionPath = m_AssetBundlePrePath + "version.json";
    }

    private void CheckVersion(Action endCall)
    {
        // s1:获取本地版本
        Debug.Log("开始获取本地版本信息...");
        GetLocalVersion(()=> {
            if (ABUtility.LoadMode != LoadModeEnum.DeviceFullAotAB)
            {
                Debug.Log("开始进入游戏！");
                endCall();
                return;
            }
            // s2：本地资源处理
            Debug.Log("本地版本已获取，开始准备本地资源...");
            CheckLocalRes((result1)=> {
                // s3：获取服务器资源版本
                Debug.Log("本地资源处理完毕，开始获取服务器资源信息...");
                Debug.Log("URL:" + m_serverVersionPath);
                GetPersistentAndServerInfo(()=> {
                    // s4：RunPatch
                    Debug.Log("开始向服务器获取Path...");
                    RunPatch((result2)=> {
                        if (result2)
                        {
                            Debug.Log("热更处理成功完毕，进入游戏！");
                        }
                        else
                        {
                            Debug.Log("热更处理失败完毕，进入游戏！");
                        }
                        endCall();
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
            SetServerPath(m_streamingVersionData.PackageName);
            call();
        }));
        GameMgr.StartCoroutine(ReadVersion(m_persistentReadVersionPath, (data) => {
            m_persistentVersionData = data;
            call();
        }));
    }

     // s2:检测本地版本，可能stream复制到Present
     void CheckLocalRes(Action<bool> okCall)
    {
        m_State = PackageState.Error;
        Debug.Log("Streaming Version: " + m_streamingVersionData.Version);
        if (m_persistentVersionData is null)
        {
            m_State = PackageState.FirstOrOverInstall;
        }
        else
        {
            Debug.Log("Persistent Version: " + m_persistentVersionData.Version);
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
                GameMgr.StartCoroutine(CopyStreamAssetsToPersistent(m_streamingVersionPath, m_persistentWriteVersionPath, okCall));
            };
            int num = 0;
            int count = m_streamingVersionData.FileInfoDict.Count - 1;
            Action<bool> call = (result) => {
                if ( ++ num == count)
                {
                    lastOneCall();
                }
            };
            foreach (var item in m_streamingVersionData.FileInfoDict)
            {
                string srcPath = $"{Application.streamingAssetsPath}/{item.Value.Name}";
                string descPath = $"{ABUtility.PersistentDataFilePath}/{item.Value.Name}";
                if (srcPath != m_streamingVersionPath)
                {
                   GameMgr.StartCoroutine(CopyStreamAssetsToPersistent(srcPath, descPath, call));
                }
            }
        }
        else
        {
            okCall(true);
        }
    }

    IEnumerator CopyStreamAssetsToPersistent(string source, string dest, Action<bool> okCall)
    {
        Debug.Log("开始写入：" + dest);
        var request = UnityWebRequest.Get(source);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.LogError(request.error);
            Debug.LogError("写入失败：" + dest);
            okCall(false);
        }
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
        Debug.Log("写入成功：" + dest);
        okCall(true);
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
        GameMgr.StartCoroutine(ReadVersion(m_persistentReadVersionPath, (data) => {
            m_persistentVersionData = data;
            call();
        }));
        GameMgr.StartCoroutine(ReadVersion(m_serverVersionPath, (data) => {
            m_serverVersionData = data;
            call();
        }));
    }

    // s4: 对比，Patch更新或整包更新
    void  RunPatch(Action<bool> okCall)
    {
        m_State = PackageState.Error;
        if (m_serverVersionData is null)
        {
            m_State = PackageState.Error;
        }
        else if (m_persistentVersionData is null)
        {
            Debug.Log("PersistentVersionData 读取失败！");
            m_State = PackageState.Error;
        }
        else
        {
            Debug.Log("Persistent Version: " + m_persistentVersionData.Version);
            Debug.Log("Server Version: " + m_serverVersionData.Version);
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
            okCall(false);
        }
        else if (m_State == PackageState.Normal)
        {
            Debug.Log($"已是最新版本，不需要热更。");
            okCall(true);
        }
        else if(m_State == PackageState.NeedFullInstall)
        {
            Debug.Log("版本差距过大，需要整包更新，不需要热更。");
            okCall(false);
        }
        else
        {
            // 需要热更
            Debug.Log("开始从服务器Path资源...");
            Action lastOneCall = () => {
                GameMgr.StartCoroutine(CopyStreamAssetsToPersistent(m_serverVersionPath, m_persistentWriteVersionPath, okCall));
            };
            int num = 0;
            int count = m_serverVersionData.FileInfoDict.Count - 1;
            Action<bool> call = (result) => {
                if (!result)
                {
                    okCall(false);
                }
                else if (++num == count)
                {
                    lastOneCall();
                }
            };
            int i = 0;
            float allSize = 0;
            foreach (var item in m_serverVersionData.FileInfoDict)
            {
                string key = item.Key;
                string srcPath = m_AssetBundlePrePath + "/" + item.Value.Name;
                string destPath = $"{ABUtility.PersistentDataFilePath}/{item.Value.Name}";
                if (m_persistentVersionData.FileInfoDict.ContainsKey(key) && m_persistentVersionData.FileInfoDict[key] == item.Value)
                {
                    call(true);
                }
                else
                {
                    if (destPath != m_persistentWriteVersionPath && !CheckLocalMD5(item.Value.MD5, destPath))
                    {
                        GameMgr.StartCoroutine(CopyStreamAssetsToPersistent(srcPath, destPath, call));
                        allSize += item.Value.Size;
                    }
                }
                i++;
            }
            Debug.Log($"总下载大小：{allSize}KB。");
        }
    }

    bool CheckLocalMD5(string serverMD5, string destPath)
    {
        string destMD5 = MD5Helper.MD5File(destPath);
        return serverMD5 == destPath;
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
        Debug.Log("Version Path: " + path);
        UnityWebRequest request = UnityWebRequest.Get(path);
        request.timeout = 2;
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
            // ignored
        }

        request.Dispose();
        call(data);
    }
}
