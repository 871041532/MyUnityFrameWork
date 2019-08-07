using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum LoadModeEnum
{
    EditorOrigin,  // Editor下直接加载原始资源
    EditorAB,  // Editor下直接加载AB包
    StandaloneAB,  // 设备或Editor下直接使用AB包
    DeviceFullAotAB,  // 设备上预下载到PrensentData再加载
}

public static class ABUtility
{
    // AB包加载模式
    public static LoadModeEnum LoadMode = LoadModeEnum.EditorAB;
    // 当前平台名字
    public static string PlatformName = "Windows";
    // 当前StreamingAssetsPath
    public static string StreamingAssetsPath = Application.streamingAssetsPath;
    public static string ServerURL = "127.0.0.1";
    public static string ServerPort = "7888";
    public static string ServerLoadPath = "";
    // AB包相对编辑器根目录路径
    public static string ABRelativePath = "AssetBundles/Windows/";
    // AB包绝对路径
    public static string ABAbsolutePath = "";

    // 设备上初始化各种信息
    public static void ResetInfoInDevice(RuntimePlatform platform)
    {
        PlatformName = RunTimePlatformToString(platform);
        LoadMode = LoadModeEnum.StandaloneAB;
        if (platform == RuntimePlatform.Android)
        {
            StreamingAssetsPath = Application.dataPath + "!assets";
        }
        else
        {
            StreamingAssetsPath = Application.streamingAssetsPath;
        }
        ServerLoadPath = string.Format("{0}:{1}/{2}/", ServerURL, ServerPort,PlatformName);
        ABRelativePath = string.Format("AssetBundles/{0}/", ABUtility.PlatformName);
        ABAbsolutePath = Path.Combine(StreamingAssetsPath, ABUtility.ABRelativePath);
    }

#if UNITY_EDITOR
    // 编辑器下初始化各种信息
    public static void ResetInfoInEditor(BuildTarget target)
    {
        PlatformName = BuildTargetToString(target);
        StreamingAssetsPath = Application.streamingAssetsPath;
        ServerLoadPath = string.Format("{0}:{1}/{2}/", ServerURL, ServerPort, PlatformName);
        ABRelativePath = string.Format("AssetBundles/{0}/", ABUtility.PlatformName);
        ABAbsolutePath = Path.Combine(Environment.CurrentDirectory, ABUtility.ABRelativePath);
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
    public static string RunTimePlatformToString(RuntimePlatform target)
    {
        var platformPath = "default";
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