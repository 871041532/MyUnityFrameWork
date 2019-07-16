using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssetBundles
{
    public class AssetBundlesMenuItems
    {
        public static string ABCONFIGPATH = "Assets/Editor/AB/ABConfig.asset";
        // key��ABName  value:�ļ���·���������ļ��а�dict
        public static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();
        // ����list
        public static List<string> m_AllFilteAB = new List<string>();

        [MenuItem("Assets/AssetBundles/Clear and Set All Bundle Tag")]
        static public void ClearAndSetAllBundleTag()
        {
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

            // ��������
            ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
           
            // ���ļ���AB����ȡ����
            m_AllFileDir.Clear();
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
                    m_AllFilteAB.Add(item.Path);
                }
            }

            // ��prefab�ӵ���prefab�ļ������ҳ�������������
            string[] allPrefabGUIDs = AssetDatabase.FindAssets("t:Prefab", abConfig.m_AllPrefabPath.ToArray());
            for (int i = 0; i < allPrefabGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allPrefabGUIDs[i]);
                EditorUtility.DisplayProgressBar("Find Prefab", "Prefab:" + path, (i + 1) * 1.0f / allPrefabGUIDs.Length);
                if (!ContainAllFileAB(path))
                {
                    string[] allDepends = AssetDatabase.GetDependencies(path);
                    List<string> allDependPath = new List<string>();
                    for (int j = 0; j < allDepends.Length; j++)
                    {
                        string dependItem = allDepends[j];
                        if (!ContainAllFileAB(dependItem) && !dependItem.EndsWith(".cs"))
                        {
                            m_AllFilteAB.Add(dependItem);
                            allDependPath.Add(dependItem);
                        }
                    }
                }

            }
            EditorUtility.ClearProgressBar();
            Debug.Log("Clear and Set All Bundle Tag done.");
        }

        static bool ContainAllFileAB(string path)
        {
            for (int i = 0; i < m_AllFilteAB.Count; i++)
            {
                if (path == m_AllFilteAB[i] || path.Contains(m_AllFilteAB[i]))
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