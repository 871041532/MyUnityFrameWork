using System;
using System.Collections;
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

    public HotPatchManager()
    {
        m_streamingVersionPath = ABUtility.StreamingAssetsURLPath + m_RelativeFilePath;
        m_persistentReadVersionPath = ABUtility.PersistentDataURLPath + m_RelativeFilePath;
        m_persistentWriteVersionPath = ABUtility.PersistentDataFilePath  + m_RelativeFilePath;
    }

    public override void Start()
    {
        CheckVersion();
    }

    void SetServerPath(string packageName)
         {
             m_AssetBundlePrePath = $"http://127.0.0.1:7888/{packageName}/{ABUtility.PlatformName}/";
             m_serverVersionPath = m_AssetBundlePrePath + "version.json";
         }

    public void CheckVersion()
    {
        SequenceJob sequenceJob = new SequenceJob();
        if (ABUtility.LoadMode == LoadModeEnum.DeviceFullAotAB)
        {
            // s1: 获取本地版本
            Job s1 = new Job(GetLocalVersion);
            sequenceJob.AddChild(s1);
            // s2：本地资源处理
            Job s2 = new Job(CheckLocalRes);
            sequenceJob.AddChild(s2);
            // s3：获取服务器资源版本
            Job s3 = new Job(GetPersistentAndServerInfo);
            sequenceJob.AddChild(s3);
            // s4：RunPatch
            Job s4 = new Job(RunPatch);
            sequenceJob.AddChild(s4);
        }
        sequenceJob.Run((job) =>
        {
            Debug.Log("Patch模块处理完毕，开始进入游戏！");
            GameMgr.m_CallMgr.TriggerEvent(EventEnum.OnPatched);
        }, (job) =>
        {
            Debug.Log("Patch模块处理失败，请检查网络连接！");
            GameMgr.m_CallMgr.TriggerEvent(EventEnum.OnPatched);
        });
    }

    // s1：获取本地版本文件
    void GetLocalVersion(Job job)
    {
        Debug.Log("开始获取本地版本信息...");
        SequenceJob seq = new SequenceJob();
        seq.AddChild(new Job((j) =>
        {
            GameMgr.StartCoroutine(ReadVersion(m_streamingVersionPath, (data) => {
                m_streamingVersionData = data;
                SetServerPath(m_streamingVersionData.PackageName);
                j.Success();
            }));
        }));
        seq.AddChild(new Job((j) =>
        {
            GameMgr.StartCoroutine(ReadVersion(m_persistentReadVersionPath, (data) => {
                m_persistentVersionData = data;
                j.Success();
            }));
        }));
        seq.Run((j) => { job.Success(); }, (j) => { job.Fail(); });
    }

     // s2:检测本地版本，可能stream复制到Present
     void CheckLocalRes(Job job2)
    {
        Debug.Log("本地版本已获取，开始准备本地资源...");
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
            var seqJob = new SequenceJob();
            foreach (var item in m_streamingVersionData.FileInfoDict)
            {
                string key = item.Key;
                string srcPath = $"{Application.streamingAssetsPath}/{item.Value.Name}";
                string destPath = $"{ABUtility.PersistentDataFilePath}/{item.Value.Name}";
                bool judge =
                    !(m_persistentVersionData != null && m_persistentVersionData.FileInfoDict.ContainsKey(key) &&
                      m_persistentVersionData.FileInfoDict[key] == item.Value) && srcPath != m_streamingVersionPath &&
                    !CheckLocalMD5(item.Value.MD5, destPath);
                if (judge)
                {
                    var job = new DownloadPatch(srcPath, destPath);
                    seqJob.AddChild(job);
                }
            }
            seqJob.AddChild(new DownloadPatch(m_streamingVersionPath, m_persistentWriteVersionPath));
            seqJob.Run((j) => { job2.Success(); }, (j)=>{ job2.Fail(); }, (j) =>
            {
                Debug.Log("进度：" + j.Progress);
            });
        }
        else
        {
            job2.Success();
        }
    }

     // s3: 获取server端与persistent信息
    void GetPersistentAndServerInfo(Job job)
    {
        Debug.Log("本地资源处理完毕，开始获取服务器资源信息...");
        SequenceJob seq = new SequenceJob();
        var s1 = new Job((j) =>
        {
            GameMgr.StartCoroutine(ReadVersion(m_persistentReadVersionPath, (data) => {
                m_persistentVersionData = data;
                j.Success();
            }));
        });
        seq.AddChild(s1);
        var s2 = new Job((j) =>
        {
            GameMgr.StartCoroutine(ReadVersion(m_serverVersionPath, (data) => {
                m_serverVersionData = data;
                j.Success();
            }));
        });
        seq.AddChild(s2);
        seq.Run((j) => { job.Success();}, (j) => { job.Fail();});
    }

    // s4: 对比，Patch更新或整包更新
    void  RunPatch(Job job)
    {
        Debug.Log("开始向服务器获取Path...");
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
            job.Fail();
        }
        else if (m_State == PackageState.Normal)
        {
            Debug.Log($"已是最新版本，不需要热更。");
            job.Success();
        }
        else if(m_State == PackageState.NeedFullInstall)
        {
            Debug.Log("版本差距过大，需要整包更新，不需要热更。");
            job.Fail();
        }
        else
        {
            // 需要热更
            Debug.Log("开始从服务器Path资源...");
            float allSize = 0;
            SequenceJob seq = new SequenceJob();
            foreach (var item in m_serverVersionData.FileInfoDict)
            {
                string key = item.Key;
                string srcPath = m_AssetBundlePrePath + "/" + item.Value.Name;
                string destPath = $"{ABUtility.PersistentDataFilePath}/{item.Value.Name}";
                if (!(m_persistentVersionData.FileInfoDict.ContainsKey(key) && m_persistentVersionData.FileInfoDict[key] == item.Value) && destPath != m_persistentWriteVersionPath && !CheckLocalMD5(item.Value.MD5, destPath))
                {
                    var j = new DownloadPatch(srcPath, destPath);
                    seq.AddChild(j);
                    allSize += item.Value.Size;
                }
            }
            seq.AddChild(new DownloadPatch(m_serverVersionPath, m_persistentWriteVersionPath));
            seq.Run((j) => { job.Success();}, (j) => { job.Fail();});
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
