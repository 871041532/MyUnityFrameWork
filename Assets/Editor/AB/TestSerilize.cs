using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

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


public class TestSerilize : ScriptableObject
{
    [MenuItem("Tools/MyTool/XmlSerilize_write")]
    static void XmlSerilize()
    {
        ABCfg cfg = new ABCfg();
        cfg.Id = 1;
        cfg.Name = "测试";
        cfg.List = new List<int>() { 1, 2, 3, 4 };

        FileStream fileStream = new FileStream("ABCfg.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        XmlSerializer xml = new XmlSerializer(typeof(ABCfg));
        xml.Serialize(sw, cfg);
        sw.Close();
        fileStream.Close();
    }

    [MenuItem("Tools/MyTool/XmlSerilize_read")]
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
}