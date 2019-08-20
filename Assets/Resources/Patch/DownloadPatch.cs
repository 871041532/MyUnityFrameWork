
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadPatch:DownloadBase
{
    private UnityWebRequest m_WebRequest;
    
    public DownloadPatch(string mURL, string filePath) : base(mURL, filePath) { }

    protected override void OnRun()
    {
        GameManager.Instance.StartCoroutine(DownLoad());
        GameManager.Instance.m_CallMgr.AddUpdate(EvalProcess);
    }
    public IEnumerator DownLoad()
    {
        m_WebRequest = UnityWebRequest.Get(m_URL);
        m_StartDownLoad = true;
        m_WebRequest.timeout = 30;
        yield return m_WebRequest.SendWebRequest();
        m_StartDownLoad = false;
        byte[] bytes = m_WebRequest.downloadHandler.data;
        if (m_WebRequest.isNetworkError || m_WebRequest.isHttpError || bytes.Length == 0)
        {
            Debug.LogError(m_WebRequest.error);
            Debug.LogError("下载失败：" + m_URL);
            this.Fail();
        }
        else
        {
            CreateDirectoryAndFile();
            FileStream fs = new FileStream(m_FilePath, FileMode.Create, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
            Debug.Log("写入成功：" + m_FilePath);
            this.Success();
        }
    }

    protected override void OnReset()
    {
        GameManager.Instance.m_CallMgr.RemoveUpdate(EvalProcess);
        Destroy();
    }

    private void EvalProcess()
    {
        ProgressChange(m_WebRequest.downloadProgress);
    }

    public override long GetCurLength()
    {
        if (!(m_WebRequest is null))
        {
            return (long)m_WebRequest.downloadedBytes;
        }
        return 0;
    }

    public override long GetLength()
    {
        if (!(m_WebRequest is null))
        {
            return (long)m_WebRequest.downloadedBytes;
        }
        return 0;
    }

    public override void Destroy()
    {
        if (!(m_WebRequest is null))
        {
            m_WebRequest.Dispose();
            m_WebRequest = null;
        }
    }
}