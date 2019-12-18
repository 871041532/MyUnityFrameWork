using UnityEngine;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum LoadModeEnum
{
    EditorOrigin,  // Editor下直接加载原始资源
    EditorAB,  // Editor下直接加载AB包
    StandaloneAB,  // 设备或Editor下直接使用整个AB包
    DeviceFullAotAB,  // 设备上或Editor预下载到PrensentData再加载
}

public static class ABUtility
{
    // AB包加载模式
    public static LoadModeEnum LoadMode = LoadModeEnum.EditorOrigin;
    // 当前平台名字
    public static string PlatformName = "Windows";
    public static string ServerURL = "10.231.10.87";
    public static string ServerPort = "7888";
    public static string ServerLoadPath = "";
    // AB包相对编辑器根目录路径
    public static string ABRelativePath = "AssetBundles/Windows/";
    // AB包绝对路径
    public static string ABAbsolutePath = "";

    //  persistentDataPath File 访问目录
    public static string PersistentDataFilePath;
    //  streamingAssetsPath File 访问目录
    public static string StreamingAssetsFilePath;
    //  persistentDataPath File 访问目录
    public static string PersistentDataURLPath;
    //  streamingAssetsPath File 访问目录
    public static string StreamingAssetsURLPath;
    
    
    // 设备上初始化各种信息，资源都ok后的处理
    public static void ResetInfoInDevice(RuntimePlatform platform, bool isDeviceFullAotABPreInRun = false)
    {
        PlatformName = RunTimePlatformToString(platform);
        if (platform == RuntimePlatform.Android)
        {
            PersistentDataFilePath = Application.persistentDataPath;
            PersistentDataURLPath = "file://" + Application.persistentDataPath;
            
            StreamingAssetsFilePath = Application.dataPath + "!assets";
            StreamingAssetsURLPath = Application.streamingAssetsPath;
        }
        else
        {
            PersistentDataFilePath = Application.persistentDataPath;
            PersistentDataURLPath = Application.persistentDataPath;
            
            StreamingAssetsFilePath = Application.streamingAssetsPath;
            StreamingAssetsURLPath = Application.streamingAssetsPath;
        }
        ServerLoadPath = $"{ServerURL}:{ServerPort}/{PlatformName}/";
        ABRelativePath = $"AssetBundles/{ABUtility.PlatformName}/";
        if (LoadMode == LoadModeEnum.StandaloneAB)
        {
            ABAbsolutePath = Path.Combine(StreamingAssetsFilePath, ABUtility.ABRelativePath);
        }
        else if (LoadMode == LoadModeEnum.DeviceFullAotAB)
        {
            if (isDeviceFullAotABPreInRun)
            {
                ABAbsolutePath = Path.Combine(StreamingAssetsFilePath, ABUtility.ABRelativePath);
            }
            else
            {
                ABAbsolutePath = Path.Combine(PersistentDataFilePath, ABUtility.ABRelativePath);
            }
        }
    }

#if UNITY_EDITOR
    // 编辑器下初始化各种信息
    public static void ResetInfoInEditor(BuildTarget target, bool isDeviceFullAotABPreInRun = false)
    {
        PlatformName = BuildTargetToString(target);
        PersistentDataFilePath = Path.Combine(Environment.CurrentDirectory , "persistentDataPath");
        PersistentDataURLPath = PersistentDataFilePath;
        StreamingAssetsFilePath = Application.streamingAssetsPath;
        StreamingAssetsURLPath = Application.streamingAssetsPath;
        
        ServerLoadPath = $"{ServerURL}:{ServerPort}/{PlatformName}/";
        ABRelativePath = $"AssetBundles/{ABUtility.PlatformName}/";
        if (!Directory.Exists(PersistentDataFilePath))
        {
            Directory.CreateDirectory(PersistentDataFilePath);
        }
        if (LoadMode == LoadModeEnum.StandaloneAB)
        {
            ABAbsolutePath = Path.Combine(StreamingAssetsFilePath, ABUtility.ABRelativePath);
        }
        else if (LoadMode == LoadModeEnum.DeviceFullAotAB)
        {
            if (isDeviceFullAotABPreInRun)
            {
                ABAbsolutePath = Path.Combine(StreamingAssetsFilePath, ABUtility.ABRelativePath);
            }
            else
            {
                ABAbsolutePath = Path.Combine(PersistentDataFilePath, ABUtility.ABRelativePath);
            }
        }
        else
        {
            ABAbsolutePath = Path.Combine(Environment.CurrentDirectory, ABUtility.ABRelativePath);
        }
    }

    public static string BuildTargetToString(BuildTarget target)
    {
        var platformPath = "default";
        if (target == UnityEditor.BuildTarget.Android)
            platformPath = "Android";
        else if (target == UnityEditor.BuildTarget.iOS)
            platformPath = "iOS";
        else if (target == UnityEditor.BuildTarget.StandaloneWindows || target == UnityEditor.BuildTarget.StandaloneWindows64)
            platformPath = "Windows";
        return platformPath;
    }
#endif

    // 运行时获取platform对应的string名字
    private static string RunTimePlatformToString(RuntimePlatform target)
    {
        string platformPath = "default";
        if (target == RuntimePlatform.Android)
        {
            platformPath = "Android";
        }
        else if (target == RuntimePlatform.IPhonePlayer)
        {
            platformPath = "iOS";
        }
        else if (target == RuntimePlatform.WindowsPlayer || target == RuntimePlatform.WindowsEditor)
        {
            platformPath = "Windows";
        }
        return platformPath;
    }
}


    public class MD5Helper
        {
            /// <summary>
            /// 对文件流进行MD5加密
            /// </summary>
            public static string MD5Stream(Stream stream)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                md5.ComputeHash(stream); 
                byte[] b = md5.Hash;
                md5.Clear();
                StringBuilder sb = new StringBuilder(32);
                for (int i = 0; i<b.Length; i++)
                {
                    sb.Append(b[i].ToString("X2"));
                }
                return sb.ToString();
            }
             
            /// <summary>
            /// 对文件进行MD5加密
            /// </summary>
            public static string MD5File(string filePath)
            {
                if (!File.Exists(filePath))
                {
                    return "";
                }
                using (FileStream stream = File.Open(filePath, FileMode.Open))
                {
                    return MD5Stream(stream); 
                }
            }
        }