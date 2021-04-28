using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(fileName = "xxxx", menuName = "CreateEditorTest", order = 0)]
    public partial class EditorTest:ScriptableObject 
    {
        public string Name = "test";

        public List<int> m_ints = new List<int>();
        public int id;
        
         public Profile profile = new Profile();
        
        public void Draw()
        {
            
        }
    }

     [System.Serializable]
    public class Profile
    {
        public string Name = "Profile";
        public int age = 1;
        [InnerProfileDraw]
        public InnerProfile profile = new InnerProfile();
        
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
        var editorTest = (EditorTest) target;
        var list = editorTest.m_ints;      
        Helper.DrawInspectorList("列表", list, (i) =>
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Label("hello");
            list[i] = EditorGUILayout.IntField(list[i]);  
            GUILayout.EndHorizontal();
        }); 
    }
}


public static class Helper
{
    public static void DrawInspectorList<T>(string name, List<T> list, Action<int> drawItemFunc, Action addItemFunc = null, Action<int> removeItemFunc = null)
    {
        int count = list.Count;
        int removeIndex = -1;
        bool isAddItem = false;
        int oldLevel = EditorGUI.indentLevel;
                
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        var length = name.Length;
        GUILayout.Label(name, GUILayout.Width(length * 12.5f));
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
        for (int i = 0; i < count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
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
        
        GUILayout.EndVertical();
        EditorGUI.indentLevel = oldLevel;
        
        if (removeIndex >= 0)
        {
            removeItemFunc(removeIndex);
        }

        if (isAddItem)
        {
            addItemFunc();
        } 
    }
}

