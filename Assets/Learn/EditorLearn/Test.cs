using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

    public class Test:MonoBehaviour
    {
//        [Label("名字")]
        [LabelText("名字")]
        public string Name = "test";

//        [HideInInspector]
//        [Label("Id")]
        public int id;
        
        [HideInInspector]  // 添加这个标签会隐藏
//        [ProfileDraw]
         public Profile profile = new Profile();
        
        public List<Profile> profile2 = new List<Profile>();
    }

     [System.Serializable]
    public class Profile
    {
        public string Name = "Profile";
        public int age = 1;
        [InnerProfileDraw]
        public InnerProfile profile = new InnerProfile();
        
        public List<InnerProfile> lists = new List<InnerProfile>();
    }

    [System.Serializable]
    public class InnerProfile
    {
        public string Name = "InnerProfile";
        public int age = 2;
    }

    public class InnerProfileDrawAttribute : PropertyAttribute
    {
        
    }
#if UNITY_EDITOR
    //************************* 内层的绘制
    [CustomPropertyDrawer(typeof(InnerProfileDrawAttribute))]
    public class InnerProfileDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
//            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Name"),new GUIContent("姓名inner："));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("age"),new GUIContent("年龄"));
            EditorGUILayout.EndVertical();
//            EditorGUI.EndProperty();
        }
    }
    
    // **********************外层的绘制
    public class ProfileDrawAttribute : PropertyAttribute
    {
        
    }


    [CustomPropertyDrawer(typeof(ProfileDrawAttribute))]
    public class ProfileDrawer : PropertyDrawer
    {
        private bool showChildFoldout = true;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Name"),new GUIContent("姓名："));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("age"),new GUIContent("性别："));
            // 注意：这一句实现了子项的嵌套
            showChildFoldout = EditorGUILayout.Foldout(showChildFoldout, "子项");
            if (showChildFoldout)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("profile"),new GUIContent("子项")); 
            }
            EditorGUILayout.EndVertical();
            EditorGUI.EndProperty();
        }
    }
    
    // ***********************************属性中文化标签
    [CustomPropertyDrawer(typeof(LabelAttribute),false)]
    public class LabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = (attribute as LabelAttribute).label;
            EditorGUI.PropertyField(position, property, label);
        }
    }
    public class LabelAttribute : PropertyAttribute
    {
        public string label;
        public LabelAttribute(string label)
        {
            this.label = label;
        }
    }

            #endif