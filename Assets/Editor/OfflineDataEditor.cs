using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    class OfflineDataEditor
    {
        [MenuItem("Assets/生成离线Prefab数据")]
        public static void AssetCreateOfflineData()
        {
            GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < objects.Length; i++)
            {
                EditorUtility.DisplayProgressBar("添加离线数据", "正在修改: " + objects[i].name, 1.0f / objects.Length);
                CreateOfflineData(objects[i]);
            }
            EditorUtility.ClearProgressBar();
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateOfflineData(GameObject Obj)
        {
            OfflineData offlineData = Obj.GetComponent<OfflineData>();
            if (offlineData is null)
            {
                offlineData = Obj.AddComponent<OfflineData>();
            }
            offlineData.BindDataInEditor();
            EditorUtility.SetDirty(Obj); // 保存
            Debug.Log("修改了" + Obj.name + " prefab！");
        }

        [MenuItem("Assets/生成离线UI数据")]
        public static void AssetCreateOfflineUIData()
        {
            GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < objects.Length; i++)
            {
                EditorUtility.DisplayProgressBar("添加离线数据", "正在修改: " + objects[i].name, i / objects.Length);
                CreateOfflineUIData(objects[i]);
            }
            EditorUtility.ClearProgressBar();
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/生成全部离线UI数据")]
        public static void AssetCreateAllOfflineUIData()
        {
            string[] strs = Selection.assetGUIDs;
            string dirPath = AssetDatabase.GUIDToAssetPath(strs[0]);
            DirectoryInfo directory = new DirectoryInfo(dirPath);
            if (directory.Exists)
            {
                string[] allPrefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { dirPath });
                for (int i = 0; i < allPrefabGUIDs.Length; i++)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGUIDs[i]);
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    EditorUtility.DisplayProgressBar("添加离线数据", "正在修改: " + obj.name,i / allPrefabGUIDs.Length);
                    CreateOfflineUIData(obj);
                }
                Debug.Log("UI离线数据全部生成完毕！");
                EditorUtility.ClearProgressBar();
                Resources.UnloadUnusedAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("没有选择文件夹！");
            }
        }

        public static void CreateOfflineUIData(GameObject obj)
        {
            obj.layer = LayerMask.NameToLayer("UI");
            OfflineData uiData = obj.GetComponent<OfflineUIData>();
            if (uiData is null)
            {
                uiData = obj.AddComponent<OfflineUIData>();
            }
            uiData.BindDataInEditor();
            EditorUtility.SetDirty(obj);
            Debug.Log("修改了" + obj.name + "UI Prefab!");
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/生成离线Effect数据")]
        public static void AssetCreateOfflineEffectData()
        {
            GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < objects.Length; i++)
            {
                EditorUtility.DisplayProgressBar("添加离线数据", "正在修改: " + objects[i].name, i / objects.Length);
                CreateOfflineEffectData(objects[i]);
            }
            EditorUtility.ClearProgressBar();
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/生成全部离线Effect数据")]
        public static void AssetCreateAllOfflineEffectData()
        {
            string[] strs = Selection.assetGUIDs;
            string dirPath = AssetDatabase.GUIDToAssetPath(strs[0]);
            DirectoryInfo directory = new DirectoryInfo(dirPath);
            if (directory.Exists)
            {
                string[] allPrefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { dirPath });
                for (int i = 0; i < allPrefabGUIDs.Length; i++)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGUIDs[i]);
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    EditorUtility.DisplayProgressBar("添加离线数据", "正在修改: " + obj.name, i / allPrefabGUIDs.Length);
                    CreateOfflineEffectData(obj);
                }
                Debug.Log("Effect离线数据全部生成完毕！");
                EditorUtility.ClearProgressBar();
                Resources.UnloadUnusedAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("没有选择文件夹！");
            }
        }

        public static void CreateOfflineEffectData(GameObject obj)
        {
            OfflineEffectData effectData = obj.GetComponent<OfflineEffectData>();
            if (effectData is null)
            {
                effectData = obj.AddComponent<OfflineEffectData>();
            }
            effectData.BindDataInEditor();
            Debug.Log("修改了" + obj.name + "Effect Prefab!");
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }
    }
}
