using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class AssetBundleConfig:ScriptableObject
{
    [SerializeField]
    public string a = "aaa";
    [SerializeField]
    public List<ResData> ResDict;
    [SerializeField]
    public List<ABData> ABDict;
    


}
[System.Serializable]
public class ResData
{
    [SerializeField]
    public string Path;
    [SerializeField]
    public string ABName;
    [SerializeField]
    public string AssetName;
}

[System.Serializable]
public class ABData
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public string[] DependenceNames;
}



