using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ABBuildConfig", menuName = "CreateABBuildConfig", order = 0)]
public class BuildConfig : ScriptableObject
{
    // 单个文件所在文件夹路径，所有的prefab名字具有唯一性
    public List<string> m_AllPrefabPath = new List<string>();
    public List<FileDirABName> m_AllFileDirAB = new List<FileDirABName>();

    [System.Serializable]
    public class FileDirABName
    {
        public string ABName;
        public string Path;
    }
}