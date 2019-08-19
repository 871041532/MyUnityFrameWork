
using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class VersionData
{
    [DataMember]
    public string Version { get; set; }
    [DataMember]
    public string PackageName { get; set; }
    [DataMember]
    public Dictionary<string, FileMD5> FileInfoDict { get; set; }
}

[DataContract]
public class FileMD5
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string MD5 { get; set; }
    [DataMember]
    public float Size { get; set; }
    
    public static bool operator ==(FileMD5 lhs, FileMD5 rhs)
    {
        return lhs.Name == rhs.Name && lhs.MD5 == rhs.MD5;
    }
    public static bool operator !=(FileMD5 lhs, FileMD5 rhs)
    {
        return lhs.Name != rhs.Name || (lhs.Name == rhs.Name && lhs.MD5 != rhs.MD5);
    }
}
