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

