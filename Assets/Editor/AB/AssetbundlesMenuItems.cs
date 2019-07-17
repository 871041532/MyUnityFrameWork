using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace AssetBundles
{
    public class AssetBundlesMenuItems
    {
        public static string ABCONFIGPATH = "Assets/Editor/AB/ABConfig.asset";
        // key：ABName  value:文件夹路径，所有文件夹包dict
        public static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();
        // 过滤list
        public static List<string> m_FilterABPaths = new List<string>();
        // 单个prefab的ab包
        public static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();

        [MenuItem("Assets/AssetBundles/Clear and Set Tag")]
        static public void ClearAndSetAllBundleTag()
        {
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

            // s3.加载配置
            ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
           
            // s4.将文件夹AB包读取出来
            foreach (ABConfig.FileDirABName item in abConfig.m_AllFileDirAB)
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
                }
            }

            // s5.将prefab从单个prefab文件夹中找出，并查找依赖
            string[] allPrefabGUIDs = AssetDatabase.FindAssets("t:Prefab", abConfig.m_AllPrefabPath.ToArray());
            for (int i = 0; i < allPrefabGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allPrefabGUIDs[i]);
                EditorUtility.DisplayProgressBar("Find Prefab", "Prefab:" + path, (i + 1) * 1.0f / allPrefabGUIDs.Length);
                if (!HavenInFilterABPaths(path))
                {
                    string[] allDepends = AssetDatabase.GetDependencies(path);
                    List<string> allDependPath = new List<string>();
                    for (int j = 0; j < allDepends.Length; j++)
                    {
                        string dependItem = allDepends[j];
                        if (!HavenInFilterABPaths(dependItem) && !dependItem.EndsWith(".cs"))
                        {
                            m_FilterABPaths.Add(dependItem);
                            allDependPath.Add(dependItem);
                        }
                        else
                        {
                            Debug.Log("已设置过，不再重复设置：" + dependItem);
                        }
                    }
                    string[] temp = path.Split('/');
                    string prefabName = temp[temp.Length -1].Split('.')[0];
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
            // s6.设置AB签名
            SetALLABName();
            // s7.删除无用的AB包
            DeleteUselessAssetBundle();
            // s8.生成资源配置
            GenerateResourceCfg();

            Debug.Log("Clear and Set All Bundle Tag done.");
        }

        /// <summary>
        /// 删除无用的AB包
        /// </summary>
        static void DeleteUselessAssetBundle()
        {
            DirectoryInfo directory = new DirectoryInfo(ABManager.CfgAssetBundleRelativePath);
            if (directory.Exists)
            {
                // 构造AB包名称Set
                string[] allBundleNames = AssetDatabase.GetAllAssetBundleNames();
                HashSet<string> allBundleNamesSet = new HashSet<string>();
                for (int i = 0; i < allBundleNames.Length; i++)
                {
                    string p = Path.Combine(System.Environment.CurrentDirectory, ABManager.CfgAssetBundleRelativePath, allBundleNames[i]);
                    string p2 = p.Replace("/", "\\");
                    allBundleNamesSet.Add(p2);
                    allBundleNamesSet.Add(p2 + ".manifest");
                    allBundleNamesSet.Add(p2 + ".meta");
                    allBundleNamesSet.Add(p2 + ".manifest.meta");
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
            string[] allBundleNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < allBundleNames.Length; i++)
            {
                string bundleName = allBundleNames[i];
                string[] inBundleAssetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                for (int j = 0; j < inBundleAssetPaths.Length; j++)
                {
                    string assetPath = inBundleAssetPaths[j];
                    if (!assetPath.EndsWith(".cs"))
                    {
                        res_to_bundle.Add(assetPath, bundleName);         
                    }
                }
            }
            // 构造每个asset的依赖信息
            AssetBundleConfig configs = new AssetBundleConfig();
            configs.ABList = new List<ABBase>();
            foreach (var item in res_to_bundle)
            {
                ABBase abBase = new ABBase();
                string assetFullPath = item.Key;
                abBase.Path = assetFullPath;
                abBase.MD5 = ABUtility.GetMD5FromFile(assetFullPath);
                abBase.ABName = item.Value;
                abBase.AssetName = assetFullPath.Remove(0, assetFullPath.LastIndexOf("/") + 1);
                abBase.ABDependence = new List<string>();
                string[] resDepends = AssetDatabase.GetDependencies(assetFullPath);
                foreach (string itemPath in resDepends)
                {
                    if (itemPath == assetFullPath || itemPath.EndsWith(".cs"))
                    {
                        continue;
                    }
                    string abName = "";
                    if (res_to_bundle.TryGetValue(itemPath, out abName))
                    {
                        if (abName == item.Value)
                        {
                            continue;
                        }
                        if (!abBase.ABDependence.Contains(abName))
                        {
                            abBase.ABDependence.Add(abName);
                        }
                    }                    
                }
                configs.ABList.Add(abBase);
            }
            // 依赖信息写入xml
            FileStream fileStream = new FileStream(Path.Combine(ABManager.CfgAssetBundleRelativePath, "AssetBundleConfig.xml"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            XmlSerializer xml = new XmlSerializer(typeof(AssetBundleConfig));
            xml.Serialize(sw, configs);
            sw.Close();
            fileStream.Close();
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
            if (importer == null)
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
                if (path == m_FilterABPaths[i] || path.Contains(m_FilterABPaths[i]))
                {
                    return true;
                }
            }
            return false;
        }

        [MenuItem("Assets/AssetBundles/Build AssetBundles")]
        static public void BuildAssetBundles()
        {
            BuildScript.BuildAssetBundles();
        }

        [MenuItem ("Assets/AssetBundles/Build Player")]
        static public void BuildPlayer ()
        {
            BuildScript.BuildPlayer();
        }
    }
}