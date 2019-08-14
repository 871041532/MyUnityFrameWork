using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

[DataContract]
public class AssetBundleConfig
{
    [DataMember]
    public List<ResData> ResDict { get; set; }
    [DataMember]
    public List<ABData> ABDict { get; set; }
}

[DataContract]
public class ResData
{
    [DataMember]
    public string Path { get; set; }
    [DataMember]
    public string ABName { get; set; }
    [DataMember]
    public string AssetName { get; set; }
}

[DataContract]
public class ABData
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string[] DependenceNames { get; set; }
}

[DataContract]
public class VersionData
{
    [DataMember]
    public string Version { get; set; }
    [DataMember]
    public string PackageName { get; set; }
    [DataMember]
    public Dictionary<string, ABMD5> ABMD5Dict { get; set; }
}

[DataContract]
public class ABMD5
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string MD5 { get; set; }
    [DataMember]
    public float Size { get; set; }
}

[DataContract]
public class ServerInfo
{
    public ServerVersionInfo[] GameVersion;
}

[DataContract]
public class ServerVersionInfo
{
    [DataMember]
    public string Version;
    [DataMember]
    public AllPatch[] Patches;
}

// 补丁包集合
[DataContract]
public class AllPatch
{
    [DataMember]
    public int Version;
    [DataMember]
    public string Description;
    [DataMember]
    public List<Patch> Files;
}

// 单个补丁
[DataContract]
public class Patch
{
    [DataMember]
    public string Name;
    [DataMember]
    public string URL;
    [DataMember]
    public string Platform;
    [DataMember]
    public string MD5;
    [DataMember]
    public float Size;
}