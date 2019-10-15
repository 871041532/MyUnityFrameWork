using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;

public class EvaluateBTEditor: ScriptableObject
{
    [MenuItem("Tools/EvaluateBTEditor")]
    public static void OpenEditor()
    {
        string cmd = string.Format("cd ");
        Process p = new Process();
        p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
        p.StartInfo.UseShellExecute = false;  //不启用shell启动进程
        p.StartInfo.RedirectStandardInput = true;  // 重定向输入
        p.StartInfo.RedirectStandardOutput = true;  // 重定向标准输出
        p.StartInfo.RedirectStandardError = true;  // 重定向错误输出 
        p.StartInfo.CreateNoWindow = true;  // 不创建新窗口
        p.Start();  // 启动程序 
        string path = Path.Combine(Application.dataPath, "Editor/EvaluateBTEditorEditor/MainWindow.py");
        p.StandardInput.WriteLine(@"cd C:\Users\zhoukaibing\Desktop\MyUnityFrameWork\Assets\Editor\EvaluateBTEditor\Editor\");  //向cmd窗口写入命令
        p.StandardInput.WriteLine("python MainWindow.py");  //向cmd窗口写入命令
        p.StandardInput.AutoFlush = true;
    }
}
