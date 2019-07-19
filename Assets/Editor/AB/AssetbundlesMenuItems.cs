using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace AssetBundles
{
    public class AssetBundlesMenuItems
    {
        public static string ABCONFIGPATH = "Assets/Editor/AB/ABConfig.asset";
        // key��ABName  value:�ļ���·���������ļ��а�dict
        public static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();
        // ����list
        public static List<string> m_FilterABPaths = new List<string>();
        // ����prefab��ab��
        public static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();
        // ��Ч·��
        public static List<string> m_DynamicLoadPaths = new List<string>();

        [MenuItem("Assets/AssetBundles/Clear and Set Tag")]
        static public void ClearAndSetAllBundleTag()
        {
            // s1.�Ȱ�������Դ��abǩ���ÿ�
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

            // s2.��ջ���
            m_AllFileDir.Clear();
            m_FilterABPaths.Clear();
            m_AllPrefabDir.Clear();
            m_DynamicLoadPaths.Clear();

            // s3.��������
            ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
           
            // s4.���ļ���AB����ȡ����
            foreach (ABConfig.FileDirABName item in abConfig.m_AllFileDirAB)
            {
                if (m_AllFileDir.ContainsKey(item.ABName))
                {
                    Debug.LogError("AB�����ظ�: " + item.ABName);
                    return;
                }
                else
                {
                    m_AllFileDir[item.ABName] = item.Path;
                    m_FilterABPaths.Add(item.Path);
                    m_DynamicLoadPaths.Add(item.Path);
                }
            }

            // s5.��prefab�ӵ���prefab�ļ������ҳ�������������
            string[] allPrefabGUIDs = AssetDatabase.FindAssets("t:Prefab", abConfig.m_AllPrefabPath.ToArray());
            for (int i = 0; i < allPrefabGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allPrefabGUIDs[i]);
                EditorUtility.DisplayProgressBar("Find Prefab", "Prefab:" + path, (i + 1) * 1.0f / allPrefabGUIDs.Length);
                if (!HavenInFilterABPaths(path))
                {
                    m_DynamicLoadPaths.Add(path);
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
                            Debug.Log("�����ù��������ظ����ã�" + dependItem);
                        }
                    }
                    string[] temp = path.Split('/');
                    string prefabName = temp[temp.Length -1].Split('.')[0];
                    if (m_AllPrefabDir.ContainsKey(prefabName) || m_AllFileDir.ContainsKey(prefabName))
                    {
                        Debug.LogError("����ͬ��prefab��AB����: " + prefabName);
                        return;
                    }
                    else
                    {
                        m_AllPrefabDir.Add(prefabName, allDependPath);
                    }              
                }
                else
                {
                    Debug.Log("�����ù��������ظ����ã�" + path);
                }
            }
            EditorUtility.ClearProgressBar();
            // s6.����ABǩ��
            SetALLABName();
            // s7.ɾ�����õ�AB��
            DeleteUselessAssetBundle();
            // s8.������Դ����
            GenerateResourceCfg();

            Debug.Log("Clear and Set All Bundle Tag done.");
        }

        /// <summary>
        /// ɾ�����õ�AB��
        /// </summary>
        static void DeleteUselessAssetBundle()
        {
            DirectoryInfo directory = new DirectoryInfo(ABManager.CfgAssetBundleRelativePath);
            if (directory.Exists)
            {
                // ����AB������Set
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
                // ɾ���ļ�
                FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    if (!allBundleNamesSet.Contains(files[i].FullName))
                    {
                        Debug.Log("ɾ������AB���ļ�: " + files[i].FullName);
                        File.Delete(files[i].FullName);
                    }
                }
                // ɾ�����ļ���
                DirectoryInfo[] directories = directory.GetDirectories("*.*", SearchOption.AllDirectories);
                foreach (DirectoryInfo itemDir in directories)
                {
                    FileSystemInfo[] subFiles = itemDir.GetFileSystemInfos();
                    if (subFiles.Length == 0)
                    {
                        itemDir.Delete();
                        Debug.Log("ɾ�����ļ���: " + itemDir.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// ������Դ���ñ�
        /// </summary>
        static void GenerateResourceCfg()
        {
            // resPath��Ӧ��AB��
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
                        res_to_bundle.Add(assetPath, bundleName);         
                    }
                }

                string[] dependBundleNames = AssetDatabase.GetAssetBundleDependencies(bundleName, false);
                ab_to_abDepences.Add(bundleName, dependBundleNames);
            }
            // ����ÿ��Res��������Ϣ
            AssetBundleConfig configs = new AssetBundleConfig();
            configs.ResDict = new Dictionary<string, ResData>();
            foreach (var item in res_to_bundle)
            {
                ResData abBase = new ResData();
                string assetFullPath = item.Key;
                abBase.Path = assetFullPath;
                abBase.MD5 = ABUtility.GetMD5FromFile(assetFullPath);
                abBase.ABName = item.Value;
                abBase.AssetName = assetFullPath.Remove(0, assetFullPath.LastIndexOf("/") + 1);
                configs.ResDict.Add(assetFullPath, abBase);
            }
            // ����ÿ��AB����������Ϣ
            configs.ABDict = new Dictionary<string, ABData>();
            foreach (var item in ab_to_abDepences)
            {
                ABData abData = new ABData();
                abData.Name = item.Key;
                abData.DependenceNames = item.Value;
                configs.ABDict.Add(abData.Name, abData);
            }

            // ������Ϣд��xml
            FileStream fileStream = new FileStream(Path.Combine("Assets/GameData/Configs", "AssetBundleConfig.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(AssetBundleConfig));
            jsonSerializer.WriteObject(fileStream, configs);
            fileStream.Close();
        }

        /// <summary>
        /// ����AB��ǩ��
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
                Debug.LogError("�ļ������ڣ�" + path);
            }
            else
            {
                importer.assetBundleName = name;
            }
        }

        /// <summary>
        /// �Ƿ��Ѿ������˴�·��
        /// </summary>
        /// <param name="path"></param>
        /// <returns>·����</returns>
        static bool HavenInFilterABPaths(string path)
        {
            for (int i = 0; i < m_FilterABPaths.Count; i++)
            {
                if (path == m_FilterABPaths[i] || (path.Contains(m_FilterABPaths[i]) && path.Replace(m_FilterABPaths[i], "")[0] == '/'))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// �Ƿ���Ҫ��̬����
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