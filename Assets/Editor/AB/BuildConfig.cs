using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ABBuildConfig", menuName = "CreateABBuildConfig", order = 0)]
public class BuildConfig : ScriptableObject
{
    // 单个文件所在文件夹路径，所有的prefab名字具有唯一性
    [Header("文件夹路径列表")]
    [Tooltip("每个路径下的资源会整体打成一个包，打包时会先处理文件夹路径列表。")]
    public List<FileDirABName> m_AllFileDirAB = new List<FileDirABName>();

    [Header("Prefab路径列表")]
    [Tooltip("此路径列表下的Prefab会单独打包，考虑到依赖关系会最后打包，所有的prefab名称应具有唯一性。")]
    public List<string> m_AllPrefabPath = new List<string>();
    

    [System.Serializable]
    public class FileDirABName
    {
        public string ABName;
        public string Path;
    }
}

//[CustomEditor(typeof(BuildConfig))]
//public class BuildConfigInspector : Editor
//{
//    public SerializedProperty m_AllPrefabPath;
//    public SerializedProperty m_AllFileDirAB;

//    private void OnEnable()
//    {
//        m_AllPrefabPath = serializedObject.FindProperty("m_AllPrefabPath");
//        m_AllFileDirAB = serializedObject.FindProperty("m_AllFileDirAB");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        EditorGUILayout.PropertyField(m_AllPrefabPath, new GUIContent("Prefab路径列表"));
//        GUILayout.Space(6);
//        EditorGUILayout.PropertyField(m_AllFileDirAB, new GUIContent("文件夹路径列表"));
//        serializedObject.ApplyModifiedProperties();
//    }
//}
