using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;

public class BundleHotFix : EditorWindow
{


    [MenuItem("Tools/热更管理面板")]
    static void Init()
    {
        BundleHotFix win = EditorWindow.GetWindow(typeof(BundleHotFix), false, "生成热更包", true) as BundleHotFix;
        win.Show();
    }

    string md5Path = "";
    string hotCount = "1";
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        md5Path = EditorGUILayout.TextField("MD5Path", md5Path, GUILayout.Width(500), GUILayout.Height(20));
        if (GUILayout.Button("选择版本文件", GUILayout.Width(150), GUILayout.Height(20)))
        {
            string file = EditorUtility.OpenFilePanel("选择版本文件", "Version/" + ABUtility.PlatformName, "*.json");
            md5Path = file;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        hotCount = EditorGUILayout.TextField("热更补丁版本：", hotCount, GUILayout.Width(500), GUILayout.Height(20));
        if (GUILayout.Button("开始打热更包", GUILayout.Width(150), GUILayout.Height(20)))
        {
            if (!string.IsNullOrEmpty(md5Path))
            {
                NormalBuild(md5Path, hotCount);
            }
            else
            {
                Debug.LogError("打包失败md5Path为空！");
            }
        }
        GUILayout.EndHorizontal();
    }

    static void NormalBuild(string md5Path, string hotCount)
    {
        VersionData versionData = ReadVersionFile(md5Path);
        var a = 1;
    }

    public static void SaveVersion(string version, string package)
    {
        // 依赖信息写入xml
        string filePath = Path.Combine(Application.dataPath, "Resources/version.json");
        VersionData config = ReadVersionFile(filePath);
        config.Version = version;
        config.PackageName = package;
        // 写入MD5
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo directory = new DirectoryInfo(ABUtility.ABAbsolutePath);
        if (directory.Exists)
        {
            FileInfo[] fileInfos = directory.GetFiles("*", SearchOption.AllDirectories);
            Dictionary<string, ABMD5> abmd5List = new Dictionary<string, ABMD5>();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                FileInfo info = fileInfos[i];
                if (!info.Name.EndsWith(".meta") && !info.Name.EndsWith(".manifest"))
                {
                    ABMD5 abmd5 = new ABMD5();
                    string name = info.FullName.Replace(directory.FullName, "").Replace("\\", "/");
                    abmd5.Name = name;
                    abmd5.MD5 = FileHelper.MD5Stream(info.FullName);
                    abmd5.Size = info.Length / 1024;
                    abmd5List.Add(name, abmd5);
                }
            }
            config.ABMD5Dict = abmd5List;
        }
        else
        {
            Debug.LogError("文件夹不存在，写入MD5失败！");
        }
        WriteVersionFile(config, filePath);
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

    // 从特定路径读取version_data
    public static VersionData ReadVersionFile(string path)
    {
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(VersionData));
                VersionData returnData = jsonSerializer.ReadObject(fileStream) as VersionData;
                return returnData;
            }
        }
        catch (Exception)
        {
            return new VersionData();
        }
    }

    // 写入version data
    static void WriteVersionFile(VersionData info, string path)
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.Write))
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(VersionData));
            jsonSerializer.WriteObject(fileStream, info);
        }
    }
}