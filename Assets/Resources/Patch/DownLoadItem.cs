﻿using System;
using System.Collections;
using System.IO;

public abstract class DownLoadItem
{
        protected string m_URL;
        protected string m_FilePath;
        protected long m_FileSize;
        protected bool m_StartDownLoad = false;

        protected DownLoadItem(string mURL, string filePath)
        {
                m_URL = mURL;
                m_FilePath = filePath;
        }

        public virtual IEnumerator DownLoad(Action callback)
        {
                yield return null;
        }
        
        public abstract float GetProgress();
        public abstract long GetCurLength();
        public abstract long GetLength();
        public abstract void Destroy();

        public void CreateDirectoryAndFile()
        {
                if (File.Exists(m_FilePath))
                {
                        File.Delete(m_FilePath);
                }
                else
                {
                        int idx = m_FilePath.LastIndexOf('/');
                        if (idx > 0)
                        {
                                string directoryPath = m_FilePath.Substring(0, idx);
                                Directory.CreateDirectory(directoryPath);
                        }
                }
        }
}