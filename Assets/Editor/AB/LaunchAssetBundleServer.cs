using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System;

namespace AssetBundles
{
    internal class LaunchAssetBundleServer : ScriptableSingleton<LaunchAssetBundleServer>
    {
        const string kLocalAssetbundleServerMenu = "Assets/Build/Launch AB File Server";

        [SerializeField]
        int     m_ServerPID = 0;

        [MenuItem(kLocalAssetbundleServerMenu, priority = 7)]
        public static void ToggleLocalAssetBundleServer()
        {
            bool isRunning = IsRunning();
            if (!isRunning)
            {
                ABUtility.ResetInfoInEditor(EditorUserBuildSettings.activeBuildTarget);
                Run();
                UnityEngine.Debug.Log("文件server已开启。端口：" + ABUtility.ServerPort);
            }
            else
            {
                KillRunningAssetBundleServer();
                UnityEngine.Debug.Log("文件server已关闭。");
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
            try
            {
                var lastProcess = Process.GetProcessById(instance.m_ServerPID);
                Process[] child_process = Process.GetProcessesByName("python");
                foreach (var item in child_process)
                {
                    item.Kill();
                }
                lastProcess.Close();
            }
            catch (Exception)
            {
            }

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
            KillRunningAssetBundleServer();
            string fileDirectory = Path.Combine(Environment.CurrentDirectory, "Hot/");
            int id = LaunchAssetBundleServer.RunCmd(fileDirectory, ABUtility.ServerPort);
            instance.m_ServerPID = id;
        }
    }
}
