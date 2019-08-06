using UnityEngine;
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
    public static LoadModeEnum RunTimeLoadMode = LoadModeEnum.EditorOrigin;
    // 当前平台名字
    public static string PlatformName = "Windows";
    // 当前StreamingAssetsPath
    public static string StreamingAssetsPath = Application.streamingAssetsPath;
    

    public static void ResetInfo(RuntimePlatform platform)
    {
        PlatformName = RunTimePlatformToString(platform);
        if (platform == RuntimePlatform.Android)
        {
            StreamingAssetsPath = Application.dataPath + "!assets";
        }
        else
        {
            StreamingAssetsPath = Application.streamingAssetsPath;
        }
    }

    // 编辑器下获取target对应的string名字 
#if UNITY_EDITOR
    public static void ResetInfoInEditor(BuildTarget target)
    {
        PlatformName = BuildTargetToString(target);
        StreamingAssetsPath = Application.streamingAssetsPath;
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