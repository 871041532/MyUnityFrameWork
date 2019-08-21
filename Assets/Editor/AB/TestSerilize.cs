using System;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

[System.Serializable]
public class ABCfg
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Name")]
    public string Name { get; set; }

    [XmlElement("List")]
    public List<int> List { get; set; }
}

//[CreateAssetMenu(fileName = "TestAssets", menuName = "CreateAssets", order = 0)]
public class AssetSerializeCfg: ScriptableObject
{
    public int Id;
    public string Name;
    public List<string> TestList;
}

public class TestSerilize
{
    [MenuItem("Tools/Test AES")]
    static void TestAES()
    {
        string key = "12345";
        string s1 = AES.EncryptString("卧槽", key);
        string s2 = AES.DecryptString(s1, key);
        Debug.Log(s1);
        Debug.Log(s2);
    }
    
    [MenuItem("Tools/Test Job")]
    static void TestJob()
    {
        var j1 = new Job((job) =>
        {
            Debug.Log("j1");
            job.Success();
        });
        var j2 = new Job((job) =>
        {
            Debug.Log("j2");
            job.Success();
        });
        var j3 = new Job((job) =>
        {
            Debug.Log("j3");
            job.Fail();
        });
        var j4 = new Job((job) =>
        {
            Debug.Log("j4");
            job.Success();
        });
        var parallel = new ParallelJob();
        parallel.AddChild(j1);
        parallel.AddChild(j2);
        parallel.AddChild(j3);
        parallel.AddChild(j4);
        parallel.Run((job) =>
        {
            Debug.Log("parallel success");
        }, (Job) =>
        {
            var children = (Job as ParallelJob)?.m_ErrorChildren;
            Debug.Log("parallel error");
        }, (job) =>
        {
            Debug.Log("parallel progress：" + job.Progress);
        });
    }
    
    [MenuItem("Tools/Serialize/XmlSerialize_write")]
    static void XmlSerilize()
    {
        ABCfg cfg = new ABCfg();
        cfg.Id = 1;
        cfg.Name = "测试";
        cfg.List = new List<int>() { 1, 2, 3, 4 };

        FileStream fileStream = new FileStream("ABCfg.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        XmlSerializer xml = new XmlSerializer(typeof(ABCfg));
        xml.Serialize(sw, cfg);
        sw.Close();
        fileStream.Close();
    }

    [MenuItem("Tools/Serialize/XmlSerialize_read")]
    static void XmlSerilize_read()
    {
        FileStream fileStream = new FileStream("ABCfg.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
        StreamReader sr = new StreamReader(fileStream, System.Text.Encoding.UTF8);
        XmlSerializer xml = new XmlSerializer(typeof(ABCfg));
        ABCfg cfg =  xml.Deserialize(sr) as ABCfg;
        Debug.Log(cfg.Id);
        Debug.Log(cfg.Name);
        Debug.Log(cfg.List);
        sr.Close();
        fileStream.Close();
    }

    [MenuItem("Tools/Serialize/BinarySerialize_write")]
    static void BinarySerializeWrite()
    {
        ABCfg cfg = new ABCfg();
        cfg.Id = 3;
        cfg.Name = "测试3";
        cfg.List = new List<int>() { 2, 3, 4 };
        FileStream fileStream = new FileStream("ABCfg.bytes", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fileStream, cfg);
        fileStream.Close();
    }

    [MenuItem("Tools/Serialize/BinarySerialize_read")]
    static void BinarySerializeRead()
    {
        FileStream fileStream = new FileStream("ABCfg.bytes", FileMode.Open, FileAccess.Read, FileShare.Read);
        BinaryFormatter bf = new BinaryFormatter();
        ABCfg cfg = bf.Deserialize(fileStream) as ABCfg;
        Debug.Log(cfg.Id);
        Debug.Log(cfg.Name);
        foreach (var item in cfg.List)
        {
            Debug.Log(item);
        }     
    }

    [MenuItem("Tools/Serialize/ScriptableObjectSerialize_write")]
    static void CreateScriptableObjectAsset()
    {
        AssetSerializeCfg asetSerializecfg = ScriptableObject.CreateInstance<AssetSerializeCfg>();
        asetSerializecfg.Id = 1;
        asetSerializecfg.Name = "ceshi";
        asetSerializecfg.TestList = new List<string>() { "aa", "ww"};
        AssetDatabase.CreateAsset(asetSerializecfg, "Assets/TestAssets.asset");
    }

    [MenuItem("Tools/Serialize/ScriptableObjectSerialize_read")]
    static void ReadScriptableObjectAsset()
    {
        AssetSerializeCfg cfg = AssetDatabase.LoadAssetAtPath<AssetSerializeCfg>("Assets/TestAssets.asset");
        Debug.Log(cfg.Id);
        Debug.Log(cfg.Name);
    }
}