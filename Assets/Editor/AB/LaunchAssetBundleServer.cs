using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System;

namespace AssetBundles
{
    internal class LaunchAssetBundleServer : ScriptableSingleton<LaunchAssetBundleServer>
    {
        const string kLocalAssetbundleServerMenu = "Assets/AssetBundles/Local AssetBundle Server";

        [SerializeField]
        int     m_ServerPID = 0;

        [MenuItem(kLocalAssetbundleServerMenu)]
        public static void ToggleLocalAssetBundleServer()
        {
            bool isRunning = IsRunning();
            if (!isRunning)
            {
                Run();
            }
            else
            {
                KillRunningAssetBundleServer();
            }
        }

        [MenuItem(kLocalAssetbundleServerMenu, true)]
        public static bool ToggleLocalAssetBundleServerValidate()
        {
            bool isRunnning = IsRunning();
            Menu.SetChecked(kLocalAssetbundleServerMenu, isRunnning);
            return true;
        }

        static bool IsRunning()
        {
            if (instance.m_ServerPID == 0)
                return false;

            try
            {
                var process = Process.GetProcessById(instance.m_ServerPID);
                if (process is null)
                    return false;

                return !process.HasExited;
            }
            catch
            {
                return false;
            }
        }

        static void KillRunningAssetBundleServer()
        {
                if (instance.m_ServerPID == 0)
                    return;
                var lastProcess = Process.GetProcessById(instance.m_ServerPID);
                Process[] child_process = Process.GetProcessesByName("python");  
                foreach (var item in child_process)
                {
                    item.Kill();
                }
                lastProcess.Close();
                instance.m_ServerPID = 0;
        }

        static int RunCmd(string path, string port)
        {
            string cmd = string.Format("python -m http.server --directory {0} {1}", path, port);
            Process p = new Process();
            p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            p.StartInfo.UseShellExecute = false;  //不启用shell启动进程
            p.StartInfo.RedirectStandardInput = true;  // 重定向输入
            p.StartInfo.RedirectStandardOutput = true;  // 重定向标准输出
            p.StartInfo.RedirectStandardError = true;  // 重定向错误输出 
            p.StartInfo.CreateNoWindow = true;  // 不创建新窗口
            p.Start();  // 启动程序 
            p.StandardInput.WriteLine(cmd);  //向cmd窗口写入命令
            p.StandardInput.AutoFlush = true;
           return p.Id;
        }

        static void Run()
        {
            string assetBundlesDirectory = Path.Combine(Environment.CurrentDirectory, ABManager.CfgAssetBundleRelativePath);

            KillRunningAssetBundleServer();

            BuildScript.CreateAssetBundleDirectory();
            int id = LaunchAssetBundleServer.RunCmd(assetBundlesDirectory, ABManager.CfgServerPort);
            instance.m_ServerPID = id;
        }
    }
}
