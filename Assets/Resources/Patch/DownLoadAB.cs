
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownLoadAB:DownLoadItem
{
    private UnityWebRequest m_WebRequest;
    
    public DownLoadAB(string mURL, string filePath) : base(mURL, filePath)
    {
    }

    public override IEnumerator DownLoad(Action callback)
    {
        m_WebRequest = UnityWebRequest.Get(m_URL);
        m_StartDownLoad = true;
        m_WebRequest.timeout = 30;
        yield return m_WebRequest.SendWebRequest();
        m_StartDownLoad = false;
        if (m_WebRequest.isNetworkError || m_WebRequest.isHttpError)
        {
            Debug.LogError(m_WebRequest.error);
            Debug.LogError("下载失败：" + m_URL);
        }
        else
        {
            CreateDirectoryAndFile();
            byte[] bytes = m_WebRequest.downloadHandler.data;
            callback?.Invoke();
        }
    }

    public override float GetProgress()
    {
        if (!(m_WebRequest is null))
        {
            return m_WebRequest.downloadProgress;
        }
        return 0;
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