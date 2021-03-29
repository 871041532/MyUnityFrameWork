using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace EditorLearn
{
    /* ************************哪些资源能序列化的demo************************************/
    [CreateAssetMenu(order = 0)]
    [InlineEditor]  // 添加上这个标识，类对象就可以作为属性显示在Inspector窗口中
    [AssetsOnly]
    public class ExampleScriptableScript : SerializedScriptableObject
    {
        // 字典数据结构Unity不序列化 ，但是Odin可以序列化。
        public Dictionary<int,string> FirstDictionary;

        // Serializable的类型，Unity会序列化Odin就不序列化了
        public MyClass unitySerialized;

        //显式不让unity序列化，让Odin接管，所以这个字段会被序列化
        [NonSerialized, OdinSerialize]
        public MyClass odinSerrialized;
    }
    
    [Serializable]  // 如果取消Serializable标签，unity就不会序列化，转而由Odin去序列化
    public class MyClass
    {
        // 因为MyClass不是跟节点，在ExampleScript没序列化，所以这边即使有OdinSerialize标签，Odin也不序列化。
        [OdinSerialize]
        public Dictionary<int, string> SecondDictionary;
    }
    
    //********************自定义资源序列化反序列化demo，通过string索引*****************//
    // 除了string之外，还能使用Index，guid索引，官网上有教程
    public class ScriptableObjectStringReferenceResolver : IExternalStringReferenceResolver
    {
        // Multiple string reference resolvers can be chained together.
        public IExternalStringReferenceResolver NextResolver { get; set; } 

        public bool CanReference(object value, out string id)
        {
            if (value is ScriptableObject)
            {
                id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value as ScriptableObject));
                return true;
            }

            id = null;
            return false;
        }

        public bool TryResolveReference(string id, out object value)
        {
            value = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(id));
            return value != null;
        }

        public static byte[] Serialize(object obj)
        {
            var context = new SerializationContext()
            {
                StringReferenceResolver = new ScriptableObjectStringReferenceResolver(),
            };
            return SerializationUtility.SerializeValue(obj, DataFormat.Nodes, context);
        }

        public static object Deserialize(byte[] bytes)
        {
            var context = new DeserializationContext()
            {
               StringReferenceResolver  = new ScriptableObjectStringReferenceResolver(),
            };
            return SerializationUtility.DeserializeValue<object>(bytes, DataFormat.Nodes, context);
        }       
        
    }
    
    
/*******************************   序列化demo2       *******************************************/
public class MyData
{
    public string str = new string(Enumerable.Range(0, 20).Select(i => (char)UnityEngine.Random.Range(50, 150)).ToArray());
    
    public List<float> numbers = new List<float>(Enumerable.Range(0, 10).Select(i => UnityEngine.Random.Range(0f, 100f)));
    
    public GameObject unityObjectReference = UnityEngine.Object.FindObjectOfType<UnityEngine.GameObject>();
    
    public MyData reference;
    
    public static void SerializeDataDemo()
    {
        string path = Application.dataPath + "/Editor/EditorLearn/Odin/MyData.json";

        var originalData = new MyData();
        originalData.reference = new MyData();
        originalData.reference.reference = originalData;

        // Unity should be allowed to handle serialization and deserialization of its own weird objects.
        // So if your data-graph contains UnityEngine.Object types, you will need to provide Odin with
        // a list of UnityEngine.Object which it will then use as an external reference resolver.
        List<UnityEngine.Object> unityObjectReferences = new List<UnityEngine.Object>();

        // 序列化
        {
            var bytes = SerializationUtility.SerializeValue(originalData, DataFormat.JSON, out unityObjectReferences);
            File.WriteAllBytes(path, bytes);
            // If you want the json string, use UTF8 encoding
            // var jsonString = System.Text.Encoding.UTF8.GetString(bytes);
        }

        // 反序列化
        {
            var bytes = File.ReadAllBytes(path);
            // If you have a string to deserialize, get the bytes using UTF8 encoding
            // var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            var data = SerializationUtility.DeserializeValue<MyData>(bytes, DataFormat.JSON, unityObjectReferences);
            Debug.Log(data.reference.reference == data);
            Debug.Log(originalData.reference.reference == originalData);
        }
    }
}













}