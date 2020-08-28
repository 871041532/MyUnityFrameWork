using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Reflection;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

public static class ClearAndSetTag
{
    static MethodInfo clearMethod = null;

    public static void ClearConsole()
    {
        if (clearMethod == null)
        {
            Type log = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries");
            clearMethod = log.GetMethod("Clear");
        }

        clearMethod.Invoke(null, null);
    }

    public static string ABCONFIGPATH = "Assets/Editor/AB/ABBuildConfig.asset";

    // key：ABName  value:文件夹路径，所有文件夹包dict
    public static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();

    // 过滤list
    public static List<string> m_FilterABPaths = new List<string>();

    // 单个prefab的ab包
    public static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();

    // 有效路径
    public static List<string> m_DynamicLoadPaths = new List<string>();

//    [MenuItem("Assets/Build/Clear And Set Tag Test", priority = 1)]
//    static public void ClearAndSetTagTest()
//    {
//        string[] allDepends = AssetDatabase.GetDependencies("Assets/GameData/Prefabs/xxxx.prefab");
//        var a = 1;
//    }

    [MenuItem("Assets/Build/Clear And Set Tag", priority = 1)]
    static public void ClearAndSetTagMenuItemFunc()
    {
        ABUtility.ResetInfoInEditor(EditorUserBuildSettings.activeBuildTarget);
        ClearAndSetAllBundleTag();
    }

    static public void ClearAndSetAllBundleTag()
    {
        ClearConsole();
        // s1.先把所有资源的ab签名置空
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string path in allAssetPaths)
        {
            AssetImporter ai = AssetImporter.GetAtPath(path);
            int length = path.Length;
            if (path.Substring(length - 3) != ".cs")
            {
                ai.assetBundleName = "";
            }
        }

        // s2.清空缓存
        m_AllFileDir.Clear();
        m_FilterABPaths.Clear();
        m_AllPrefabDir.Clear();
        m_DynamicLoadPaths.Clear();

        // s3.加载配置
        BuildConfig abConfig = AssetDatabase.LoadAssetAtPath<BuildConfig>(ABCONFIGPATH);

        // s4.将文件夹AB包读取出来
        foreach (BuildConfig.FileDirABName item in abConfig.m_AllFileDirAB)
        {
            if (m_AllFileDir.ContainsKey(item.ABName))
            {
                Debug.LogError("AB包名重复: " + item.ABName);
                return;
            }
            else
            {
                m_AllFileDir[item.ABName] = item.Path;
                m_FilterABPaths.Add(item.Path);
                m_DynamicLoadPaths.Add(item.Path);
            }
        }

        // s5.将prefab从单个prefab文件夹中找出，并查找依赖
        if (abConfig.m_AllPrefabPath.Count > 0)
        {
            string[] allPrefabGUIDs = AssetDatabase.FindAssets("t:Prefab t:Scene", abConfig.m_AllPrefabPath.ToArray());
            for (int i = 0; i < allPrefabGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allPrefabGUIDs[i]);
                EditorUtility.DisplayProgressBar("Find Prefab", "Prefab:" + path,
                    (i + 1) * 1.0f / allPrefabGUIDs.Length);
                // 防止路径被文件夹或其他prefab设置过
                if (!HavenInFilterABPaths(path))
                {
                    // prefab路径加入有效路径m_DynamicLoadPaths
                    m_DynamicLoadPaths.Add(path);
                    string[] allDepends = AssetDatabase.GetDependencies(path);
                    // 将此prefab的所有依赖项筛选没被使用的
                    List<string> allDependPath = new List<string>();
                    for (int j = 0; j < allDepends.Length; j++)
                    {
                        string dependItem = allDepends[j];
                        // 此处剔除掉已经设置过的、已在文件夹中的或者cs文件
                        if (!HavenInFilterABPaths(dependItem) && !dependItem.EndsWith(".cs"))
                        {
                            m_FilterABPaths.Add(dependItem);
                            allDependPath.Add(dependItem);
                        }
                        else
                        {
                            Debug.Log("已设置过，不再重复设置; 或是C#文件不需要设置：" + dependItem);
                        }
                    }

                    string[] temp = path.Split('/');
                    string prefabName = temp[temp.Length - 1].Split('.')[0];
                    if (m_AllPrefabDir.ContainsKey(prefabName) || m_AllFileDir.ContainsKey(prefabName))
                    {
                        Debug.LogError("存在同名prefab或AB包名: " + prefabName);
                        return;
                    }
                    else
                    {
                        m_AllPrefabDir.Add(prefabName, allDependPath);
                    }
                }
                else
                {
                    Debug.Log("已设置过，不再重复设置：" + path);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        // s6.设置AB签名m_AllFileDir、m_AllPrefabDir（上面的步骤生成了abName -> path、prefabName(abName)->allDependPaths的映射）
        SetALLABName();
        // s7.删除无用的AB包（利用UnityAssetBundle相关API，基本不依赖上面流程）
        DeleteUselessAssetBundle();
        // s8.生成资源配置（利用UnityAssetBundle的API，基本不依赖上面流程）
        GenerateResourceCfg();
        // s9.检测是否有循环引用（深度优先检测，不依赖上面流程）
        CheckRecycleReference.CheckRecycleAB();
        Debug.Log("Clear and Set All Bundle Tag done.");
    }

    /// <summary>
    /// 删除无用的AB包
    /// </summary>
    static void DeleteUselessAssetBundle()
    {
        DirectoryInfo directory = new DirectoryInfo(ABUtility.ABRelativePath);
        if (directory.Exists)
        {
            // 构造AB包名称Set
            string[] temp = AssetDatabase.GetAllAssetBundleNames();
            List<string> allBundleNames = new List<string>(temp);
            allBundleNames.Add(ABUtility.PlatformName);
            HashSet<string> allBundleNamesSet = new HashSet<string>();
            for (int i = 0; i < allBundleNames.Count; i++)
            {
                // AB签名设置到文件夹上，里面是空的话不打包，这里把之前的删除掉
                if (AssetDatabase.GetAssetPathsFromAssetBundle(allBundleNames[i]).Length > 0 ||
                    allBundleNames[i] == ABUtility.PlatformName)
                {
                    string p = Path.Combine(System.Environment.CurrentDirectory, ABUtility.ABRelativePath,
                        allBundleNames[i]);
                    string p2 = p.Replace("/", "\\");
                    allBundleNamesSet.Add(p2);
                    allBundleNamesSet.Add(p2 + ".manifest");
                    allBundleNamesSet.Add(p2 + ".meta");
                    allBundleNamesSet.Add(p2 + ".manifest.meta");
                }
            }

            // 删除文件
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!allBundleNamesSet.Contains(files[i].FullName))
                {
                    Debug.Log("删除冗余AB包文件: " + files[i].FullName);
                    File.Delete(files[i].FullName);
                }
            }

            // 删除空文件夹
            DirectoryInfo[] directories = directory.GetDirectories("*.*", SearchOption.AllDirectories);
            foreach (DirectoryInfo itemDir in directories)
            {
                FileSystemInfo[] subFiles = itemDir.GetFileSystemInfos();
                if (subFiles.Length == 0)
                {
                    itemDir.Delete();
                    Debug.Log("删除空文件夹: " + itemDir.FullName);
                }
            }
        }
    }

    /// <summary>
    /// 生成资源配置表
    /// </summary>
    static void GenerateResourceCfg()
    {
        // resPath对应的AB包
        Dictionary<string, string> res_to_bundle = new Dictionary<string, string>();
        Dictionary<string, string[]> ab_to_abDepences = new Dictionary<string, string[]>();
        string[] allBundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < allBundleNames.Length; i++)
        {
            string bundleName = allBundleNames[i];
            string[] inBundleAssetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            for (int j = 0; j < inBundleAssetPaths.Length; j++)
            {
                string assetPath = inBundleAssetPaths[j];
                if (!assetPath.EndsWith(".cs") && IsDynamicLoadPath(assetPath))
                {
                    // 只有在上面配置中的资源才会进入bundleName
                    res_to_bundle.Add(assetPath, bundleName);
                }
            }

            string[] dependBundleNames = AssetDatabase.GetAssetBundleDependencies(bundleName, false);
            ab_to_abDepences.Add(bundleName, dependBundleNames);
        }

        // 构造每个Res的依赖信息
        AssetBundleConfig configs = ScriptableObject.CreateInstance<AssetBundleConfig>();
        configs.ResDict = new List<ResData>();
        foreach (var item in res_to_bundle)
        {
            ResData abBase = new ResData();
            string assetFullPath = item.Key;
            abBase.Path = assetFullPath;
            abBase.ABName = item.Value;
            abBase.AssetName = assetFullPath.Remove(0, assetFullPath.LastIndexOf("/") + 1);
            configs.ResDict.Add(abBase);
        }

        // 构造每个AB包的依赖信息
        configs.ABDict = new List<ABData>();
        foreach (var item in ab_to_abDepences)
        {
            ABData abData = new ABData();
            abData.Name = item.Key;
            abData.DependenceNames = item.Value;
            configs.ABDict.Add(abData);
        }
        
        // lua文件加个.txt后缀
        foreach (var item in configs.ResDict)
        {
            if (item.Path.EndsWith(".lua"))
            {
                item.Path += ".txt";
                item.AssetName += ".txt";
            }
        }
        
        // 依赖信息写入json
        string filePath = Path.Combine("Assets/GameData/Configs", "AssetBundleConfig.asset");
        File.Delete(filePath);
        AssetDatabase.CreateAsset(configs, filePath);
        
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 设置AB包签名
    /// </summary>
    static void SetALLABName()
    {
        foreach (var item in m_AllFileDir)
        {
            SetABName(item.Key, item.Value);
        }

        foreach (var item in m_AllPrefabDir)
        {
            string name = item.Key;
            List<string> paths = item.Value;
            for (int i = 0; i < paths.Count; i++)
            {
                SetABName(name, paths[i]);
            }
        }
    }

    static void SetABName(string name, string path)
    {
        AssetImporter importer = AssetImporter.GetAtPath(path);
        if (importer is null)
        {
            Debug.LogError("文件不存在：" + path);
        }
        else
        {
            importer.assetBundleName = name;
        }
    }

    /// <summary>
    /// 是否已经包含了此路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns>路径名</returns>
    static bool HavenInFilterABPaths(string path)
    {
        for (int i = 0; i < m_FilterABPaths.Count; i++)
        {
            // path已经在路径中了，或者路径是个path的父目录
            if (path == m_FilterABPaths[i] ||
                (path.Contains(m_FilterABPaths[i]) && path.Replace(m_FilterABPaths[i], "")[0] == '/'))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 是否需要动态加载， 目的是只记录想要关心的资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool IsDynamicLoadPath(string path)
    {
        for (int i = 0; i < m_DynamicLoadPaths.Count; i++)
        {
            if (path.Contains(m_DynamicLoadPaths[i]))
            {
                return true;
            }
        }

        return false;
    }
}