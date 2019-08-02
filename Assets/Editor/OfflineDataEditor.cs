using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    class OfflineDataEditor
    {
        [MenuItem("Assets/生成离线数据")]
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
    }
}
