using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "xxxx", menuName = "CreateEditorTest", order = 0)]
public partial class EditorTest : ScriptableObject
{
//    [LabelText("名称")] 
    public string Name = "test";
    [InnerProfileDraw]
    public InnerProfile innerProfile = new InnerProfile();

    [LabelText("m_ints列表")]
//    [SerializeField]
    public List<int> m_ints = new List<int>();
    
    [SerializeField]
    public List<Profile> m_profiles = new List<Profile>()
    {
        new Profile(), new Profile()
    };
    public int id;

    public Profile profile = new Profile();
}

[System.Serializable]
public class Profile
{
    public string Name = "Profile";
    public int age = 1;
    [InnerProfileDraw] public InnerProfile profile = new InnerProfile();

    public List<Profile> lists = new List<Profile>();
}

[System.Serializable]
public class InnerProfile
{
    public string Name = "InnerProfile";
    public int age = 2;
}

[CustomEditor(typeof(EditorTest))]
public class EditorTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        var editorTest = (EditorTest) target;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("innerProfile"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ints"), true);
//        var m_ints = serializedObject.FindProperty("m_ints");
        editorTest.m_ints = Helper.DrawGeneralList("列表", editorTest.m_ints, (i) =>
        {
//            var item = m_ints.GetProperTypeName()
//            serializedObject.serialized
            GUILayout.BeginHorizontal();
            GUILayout.Label("hello");
            editorTest.m_ints[i] = EditorGUILayout.IntField(editorTest.m_ints[i]);
            GUILayout.EndHorizontal();
        }, () =>
        {
            editorTest.m_ints.Add(0);
        }, (i) =>
        {
            editorTest.m_ints.RemoveAt(i);
        });
        var property = serializedObject.FindProperty("m_profiles");
        EditorGUILayout.PropertyField(property, true);
//        editorTest.m_profiles = Helper.DrawGeneralList("列表", editorTest.m_profiles, (i) =>
//        {
//            
//        }, () =>
//        {
//            editorTest.m_ints.Add(0);
//        }, (i) =>
//        {
//            editorTest.m_ints.RemoveAt(i);
//        });
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}

public class LabelTextAttribute : PropertyAttribute
{
    public string label;

    public LabelTextAttribute(string label)
    {
        this.label = label;
    }
}

[CustomPropertyDrawer(typeof(LabelTextAttribute), false)]
public class LabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = (attribute as LabelTextAttribute).label;
        string[] options = {"Popup选项一", "Popup选项二", "Popup选项三"};
        var oldValue = 1;
        oldValue = EditorGUILayout.Popup("下拉列表", oldValue, options);
    }
}

public static class Helper
{
    private static int IndentLevel = 0;
    public static List<T> DrawGeneralList<T>(string name, List<T> list, Action<int> drawItemFunc,
        Action addItemFunc = null, Action<int> removeItemFunc = null)
    {
        if (list == null)
        {
            list = new List<T>();
        }

        int count = list.Count;
        int removeIndex = -1;
        bool isAddItem = false;

        if (count == 0 && addItemFunc == null)
        {
            return list;
        }

//            GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        var length = name.Length;
        GUILayout.Label(name, GUILayout.Width(length * 13f));
        if (addItemFunc != null)
        {
            if (GUILayout.Button("+"))
            {
                isAddItem = true;
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(5);


        IndentLevel++; // 自己维护缩进值，unity的有bug
        for (int i = 0; i < count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(16 * IndentLevel);
            GUILayout.BeginVertical();
            var item = list[i];
            drawItemFunc(i);

            GUILayout.Space(10);
            GUILayout.EndVertical();
            if (removeItemFunc != null)
            {
                if (GUILayout.Button("-"))
                {
                    removeIndex = i;
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        IndentLevel--;
//            GUILayout.EndVertical();

        if (removeIndex >= 0)
        {
            removeItemFunc(removeIndex);
        }

        if (isAddItem)
        {
            addItemFunc();
        }

        return list;
    }
}