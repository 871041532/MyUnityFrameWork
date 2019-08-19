using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;

public class BundleHotFix : EditorWindow
{
    static string m_curVersionPath = "Assets/StreamingAssets/version.json";
    static string m_HotPath = "Hot";
    
    static string m_Version = "";
    static string m_PackageName = "1";

    [MenuItem("Tools/热更管理面板")]
    static void Init()
    {
        BundleHotFix win = EditorWindow.GetWindow(typeof(BundleHotFix), false, "生成热更包", true) as BundleHotFix;
        win.Show();
        m_Version = PlayerSettings.bundleVersion;
        m_PackageName = PlayerSettings.applicationIdentifier;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        m_Version = EditorGUILayout.TextField("Version:", m_Version, GUILayout.Width(500), GUILayout.Height(20));
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        m_PackageName = EditorGUILayout.TextField("PackageName:", m_PackageName, GUILayout.Width(500), GUILayout.Height(20));
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("确认设置", GUILayout.Width(300), GUILayout.Height(20)))
        {
        }
        GUILayout.EndHorizontal();
    }

    // BuildPlayer时将StreamingAssets部署到服务器
    public static void DeployStreamingAssetsToHotWhenBuildPlayer()
    {
        Debug.Log($"开始部署热更文件，Version:{PlayerSettings.bundleVersion}  PackageName:{PlayerSettings.applicationIdentifier}");
        string srcPath = ABUtility.StreamingAssetsFilePath + "/";
        string destPath = $"{m_HotPath}/{PlayerSettings.applicationIdentifier}/{ABUtility.PlatformName}/";
        if (Directory.Exists(destPath))
        {
            Directory.Delete(destPath, true);
        }
        if (!File.Exists(m_curVersionPath))
        {
            Debug.LogError($"文件不存在，部署失败：{m_curVersionPath}");
            return;
        }
        VersionData versionData = ReadJsonFile<VersionData>(m_curVersionPath);
        foreach (var item in versionData.FileInfoDict)
        {
            var md5Data = item.Value;
            string srcFile = srcPath + md5Data.Name;
            string destFile = destPath + md5Data.Name;
            if (!File.Exists(srcFile))
            {
                Debug.LogError($"文件不存在，部署失败{srcFile}");
                return;
            }
            int idx = destFile.LastIndexOf('/');
            string p1 = destFile.Substring(0, idx);
            Directory.CreateDirectory(p1);
            File.Copy(srcFile, destFile);
            Debug.Log("已部署：" + md5Data.Name);
        }
        Debug.Log("部署完毕。路径：" + destPath);
    }
    
    #region BuildPlayer时，将当前平台版本信息覆盖写入StreamingAssets/version.json
    public static void SaveVersionToStreamingAssetsWhenBuildPlayer(string version, string package)
    {
        // 依赖信息写入
        VersionData config = new VersionData()
        {
            Version = PlayerSettings.bundleVersion,
            PackageName = PlayerSettings.applicationIdentifier,
            FileInfoDict = new Dictionary<string, FileMD5>()
        };
        // 写入MD5
        var abMd5List = config.FileInfoDict;
        DirectoryInfo directory = new DirectoryInfo(ABUtility.StreamingAssetsFilePath);
        if (directory.Exists)
        {
            FileInfo[] fileInfos = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfos.Length; i++)
            {
                FileInfo info = fileInfos[i];
                if (!info.Name.EndsWith(".meta") && !info.Name.EndsWith(".manifest") && info.Name != "version.json")
                {
                    var abMd5 = new FileMD5();
                    string name = info.FullName.Replace(directory.FullName + "\\", "").Replace("\\", "/");
                    abMd5.Name = name;
                    abMd5.MD5 = MD5Helper.MD5File(info.FullName);
                    abMd5.Size = info.Length / 1024;
                    abMd5List.Add(name, abMd5);
                }
            }
            // 把自己也添加进去
            abMd5List.Add("version.json", new FileMD5()
            {
                Name = "version.json",
                MD5 = config.Version,
                Size = 1,
            });
        }
        else
        {
            Debug.Log("AB文件夹不存在，写入MD5为空！");
        }
        WriteJsonFile<VersionData>(config, m_curVersionPath);
        Debug.Log($"Version写入完毕。包名：{config.PackageName}； 平台：{ABUtility.PlatformName}； 版本：{config.Version}");
    }
    #endregion

    #region 读写VersionData
    // 从特定路径读取version_data
    public static T ReadJsonFile<T>(string path) where T: class, new()
    {
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
                T returnData = jsonSerializer.ReadObject(fileStream) as T;
                return returnData;
            }
        }
        catch (Exception)
        {
            var returnData = new T();
            return new T();
        }
    }

    // 写入version data
    static void WriteJsonFile<T>(T info, string path) where T:class
    {
        using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
        {

        }
        using (FileStream fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.Write))
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            jsonSerializer.WriteObject(fileStream, info);
        }
    }
    #endregion
}