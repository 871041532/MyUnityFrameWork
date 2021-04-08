using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.Serialization;
using UnityEngine;
using UnityEditor;

namespace EditorLearn
{
    public static class EditorLearnIndex
    {
        /*
         * 
        MenuItem(string itemName, bool isValidateFunction, int priority) 
        itemName：菜单名称路径 
        isValidateFunction：是否为校验函数。不写为false点击函数才会调用，true则点击菜单前就会调用 
        priority：菜单项显示排序
         */
        //返回当前屏幕上第一个 T 类型的 EditorWindow。
        // 如果没有，则创建并显示新窗口，然后返回其实例。
        [MenuItem("EditorLearn/MyWindow #m")]
        public static void ShowMyWindow()
        {
            // 第一个参数为true，不可停靠，为false可停靠
            MyWindow window =  EditorWindow.GetWindow<MyWindow>(false, "我的Unity窗口", true);
        }
        
        // 获取一个带Rect的Window
        [MenuItem("EditorLearn/MyWindowRect")]
        public static void ShowMyWindowWithRect()
        {
            var rect = new Rect(100, 100, 300, 300);
            MyWindow window =  EditorWindow.GetWindowWithRect<MyWindow>(rect, false, "我的窗口Rect");
        }
        
        // 打开一个节点编辑窗口
        [MenuItem("EditorLearn/节点编辑窗口")]
        public static void ShowNodeWindow()
        {
            EditorWindow.GetWindow<NodeWindow.NodeWindow>(false, "节点编辑demo", true);
        }
        
        // 打开Odin窗口
        [MenuItem("EditorLearn/打开Odin窗口")]
        public static void ShowOdinWindow()
        {
            EditorWindow.GetWindow<MyOdinWindow>(false, "Odin窗口", true);
        }
        
        // 打开Odin菜单窗口
        [MenuItem("EditorLearn/打开Odin菜单窗口")]
        public static void ShowOdinMenuEditorWindow()
        {
            EditorWindow.GetWindow<MyOdinMenuWindow>(false, "Odin菜单窗口", true);
        }
        
        // 判断按钮什么时候显示，返回false时对应按钮不显示
        [MenuItem("EditorLearn/PrintSelect", true)]
        static bool IsShowPrintSelectButton()
        {
            return Selection.activeGameObject != null;
        }
        // 点击按钮时要做的事情
        [MenuItem("EditorLearn/PrintSelect", false)]
        static void OnClickPrintSelectButton()
        {
            Debug.Log(Selection.activeGameObject.name);
        }
        
        // 为组件添加菜单项 [MenuItem("CONTEXT/组件名/显示方法名")]
        [MenuItem("CONTEXT/Transform/TranslateFarAway")]
        static void TranslateFarAway(MenuCommand cmd)
        {
             (cmd.context as Transform).position = Vector3.down * 1000f;
        }
        
        // 为Assets窗口右键添加菜单选项（打印选择的文件或文件夹路径）
        [MenuItem("Assets/打印选择路径", false, 1)]
        static void PrintSelect(MenuCommand cmd)
        {
            var guid = Selection.assetGUIDs[0];
            Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
        }
        
        // Odin反序列化测试
        [MenuItem("EditorLearn/Odin反序列化ScriptableObject")]
        static void OdinUnserializedScriptableObject()
        {
            var data = AssetDatabase.LoadAssetAtPath<ExampleScriptableScript>("Assets/Editor/EditorLearn/Odin/SerializedTestExampleScriptableScript.asset");
            
            // 使用自定义类反序列化
            byte[] serializeData = ScriptableObjectStringReferenceResolver.Serialize(data);
            var deserializeData = ScriptableObjectStringReferenceResolver.Deserialize(serializeData) as ExampleScriptableScript;
            Debug.Log(deserializeData == data);
            Debug.Log(ReferenceEquals(deserializeData, data));
        }

        [MenuItem("EditorLearn/Odin序列化自定义类1")]
        static void OdinUnserializedMyClass()
        {
            // 使用系统工具进行序列化反序列化
            MyClass myClass = new MyClass();
            myClass.SecondDictionary = new Dictionary<int, string>();
            myClass.SecondDictionary[1] = "aaaa";
            
            var path = "Assets/Editor/EditorLearn/Odin/myClassSerializeTest.json"; 
            var json = SerializationUtility.SerializeValue(myClass, DataFormat.JSON);
            File.WriteAllBytes(path, json);
            json = File.ReadAllBytes(path);
            var data2 = SerializationUtility.DeserializeValue<MyClass>(json, DataFormat.JSON);
            Debug.Log(data2 == myClass);
        }

        [MenuItem("EditorLearn/Odin序列化自定义类2")]
        static void OdinUnserializedMyData()
        {
            MyData.SerializeDataDemo(); 
        }
        
    }
}