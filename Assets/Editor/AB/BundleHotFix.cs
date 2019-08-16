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

    [MenuItem("Tools/热更管理面板")]
    static void Init()
    {
        BundleHotFix win = EditorWindow.GetWindow(typeof(BundleHotFix), false, "生成热更包", true) as BundleHotFix;
        win.Show();
    }

    string targetVersionPath = "";
    string hotCount = "1";
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        targetVersionPath = EditorGUILayout.TextField("targetVersionPath", targetVersionPath, GUILayout.Width(500), GUILayout.Height(20));
        if (GUILayout.Button("选择版本文件", GUILayout.Width(150), GUILayout.Height(20)))
        {
            string file = EditorUtility.OpenFilePanel("选择版本文件", "Version/" + ABUtility.PlatformName, "*.json");
            targetVersionPath = file;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        hotCount = EditorGUILayout.TextField("热更补丁版本：", hotCount, GUILayout.Width(500), GUILayout.Height(20));
        if (GUILayout.Button("开始打热更包", GUILayout.Width(150), GUILayout.Height(20)))
        {
            ABUtility.ResetInfoInEditor(EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("开始生成热更包...");
            Debug.Log("路径：" + m_HotPath + "/" + ABUtility.PlatformName + "/" + PlayerSettings.bundleVersion + "/" + hotCount + "/");
            if (!string.IsNullOrEmpty(targetVersionPath) && File.Exists(targetVersionPath))
            {
                NormalBuild(targetVersionPath, hotCount);
            }
            else
            {
                Debug.LogError("打包失败目标版本文件为空！");
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("部署StreamingAssets到服务器", GUILayout.Width(300), GUILayout.Height(20)))
        {
            DeployStreamingAssetsToHot();
        }
        GUILayout.EndHorizontal();
    }

    static void DeployStreamingAssetsToHot()
    {
        Debug.Log($"开始部署，Version:{PlayerSettings.bundleVersion}  PackageName:{PlayerSettings.applicationIdentifier}");
        ABUtility.ResetInfoInEditor(EditorUserBuildSettings.activeBuildTarget);
        string srcPath = ABUtility.StreamingAssetsPath + "/";
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
        foreach (var item in versionData.ABMD5Dict)
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
    
    static void NormalBuild(string targetVersionPath, string hotCount)
    {
        VersionData targetVersionData = ReadJsonFile<VersionData>(targetVersionPath);
        string curVersionPath = m_curVersionPath;
        if (!File.Exists(curVersionPath))
        {
            Debug.LogError("打包失败，当前版本version文件为空！");
            return;
        }
        VersionData curVersionData = ReadJsonFile<VersionData>(curVersionPath);
        Dictionary<string, ABMD5> diferentDict = new Dictionary<string, ABMD5>();
        foreach (var item in curVersionData.ABMD5Dict.Values)
        {
            var targetDict = targetVersionData.ABMD5Dict;
            if (!targetDict.ContainsKey(item.Name) || (targetDict.ContainsKey(item.Name) && targetDict[item.Name].MD5 != item.MD5 ))
            {
                if (item.Name != ABUtility.PlatformName)
                {
                    diferentDict.Add(item.Name, item);
                }        
            }
        }
        CopyDifferentABToHot(diferentDict, hotCount);
        Debug.Log("热更包生成完毕！");
    }

    static void CopyDifferentABToHot(Dictionary<string, ABMD5> diferentDict, string hotCount)
    {
        string relativePath = ABUtility.PlatformName + "/" +PlayerSettings.bundleVersion + "/" + hotCount + "/";
        string hotPlatformPath = Path.Combine(m_HotPath, relativePath);
        if (Directory.Exists(hotPlatformPath))
        {
            Directory.Delete(hotPlatformPath, true);
        }
        Directory.CreateDirectory(hotPlatformPath);
        foreach (var item in diferentDict)
        {
            Debug.Log("AB包：" + item.Key);
            string sourceFIlePath = Path.Combine(ABUtility.StreamingAssetsPath, item.Key);
            string targetFilePath = Path.Combine(hotPlatformPath, item.Key);
            int idx = targetFilePath.LastIndexOf('/');
            string p1 = targetFilePath.Substring(0, idx);
            Directory.CreateDirectory(p1);
            File.Copy(sourceFIlePath, targetFilePath, true);
        }

        // 生成Patch
        AllPatch allPatch = new AllPatch();
        allPatch.Version = 1;
        allPatch.Files = new List<Patch>();
        string fileServerPath = string.Format("{0}:{1}/", ABUtility.ServerURL, ABUtility.ServerPort);
        foreach (var item in diferentDict)
        {
            Patch patch = new Patch();
            patch.Name = item.Value.Name;
            patch.MD5 = item.Value.MD5;
            patch.Size = item.Value.Size;
            patch.Platform = ABUtility.PlatformName;
            patch.URL = fileServerPath + relativePath + patch.Name;
            allPatch.Files.Add(patch);
        }
        WriteJsonFile<AllPatch>(allPatch, Path.Combine(hotPlatformPath, "allPath.json"));
        Debug.Log("Patch配置：allPath.json");
    }

    #region buildAB时，将当前平台版本信息覆盖写入Resources/version.json
    public static void SaveVersionWhenBuildPlayer(string version, string package)
    {
        // 依赖信息写入
        string filePath = m_curVersionPath;
        VersionData config = ReadJsonFile<VersionData>(filePath);
        config.Version = version;
        config.PackageName = package;
        // 写入MD5
        Dictionary<string, ABMD5> abmd5List = new Dictionary<string, ABMD5>();
        DirectoryInfo directory = new DirectoryInfo(ABUtility.StreamingAssetsPath);
        if (directory.Exists)
        {
            FileInfo[] fileInfos = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfos.Length; i++)
            {
                FileInfo info = fileInfos[i];
                if (!info.Name.EndsWith(".meta") && !info.Name.EndsWith(".manifest"))
                {
                    ABMD5 abmd5 = new ABMD5();
                    string name = info.FullName.Replace(directory.FullName + "\\", "").Replace("\\", "/");
                    abmd5.Name = name;
                    abmd5.MD5 = FileHelper.MD5Stream(info.FullName);
                    abmd5.Size = info.Length / 1024;
                    abmd5List.Add(name, abmd5);
                }
            }
        }
        else
        {
            Debug.Log("AB文件夹不存在，写入MD5为空！");
        }
        config.ABMD5Dict = abmd5List;
        WriteJsonFile<VersionData>(config, filePath);
        // 将版本文件拷贝到外部存储
        string outDirectory = Path.Combine("Version", ABUtility.PlatformName);
        if (!Directory.Exists(outDirectory))
        {
            Directory.CreateDirectory(outDirectory);
        }
        string outFilePath = string.Format("{0}/{1}{2}.json", outDirectory, "version_", config.Version);
        if (File.Exists(outFilePath))
        {
            File.Delete(outFilePath);
        }
        File.Copy(filePath, outFilePath);
        Debug.Log(string.Format("{0} version {1} 信息写入完毕。Path: {2}", ABUtility.PlatformName, config.Version, outFilePath));
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
        //if (!File.Exists(path))
        //{
        //    File.Create(path);
        //}
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