using UnityEngine;
using UnityEditor;

public class BundleHotFix : EditorWindow
{
    

    [MenuItem("Tools/生成热更包")]
    static void Init()
    {
        BundleHotFix win = EditorWindow.GetWindow(typeof(BundleHotFix), false, "生成热更包", true) as BundleHotFix;
        win.Show();
    }

    static string md5Path = "wocaoasdasds";
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        md5Path = EditorGUILayout.TextField("MD5Path    ", md5Path, GUILayout.Width(50), GUILayout.Height(20));
        GUILayout.EndHorizontal();
    }
}