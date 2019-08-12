using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class MyBuildApp : ScriptableObject
{
    [MenuItem("Assets/Build/Build AssetBundles", priority = 2)]
    public static void BuildAB()
    {
        ABUtility.ResetInfoInEditor(EditorUserBuildSettings.activeBuildTarget);
        ClearAndSetTag.ClearAndSetAllBundleTag();
        BuildAssetBundles(EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("Assets/Build/Build ActivePlayer", priority = 3)]
    public static void BuildCurrentPlantform()
    {
        ABUtility.ResetInfoInEditor(EditorUserBuildSettings.activeBuildTarget);
        ClearAndSetTag.ClearAndSetAllBundleTag();
        BuildAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        BuildPlayer(EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("Assets/Build/Build Android", priority =4)]
    public static void BuildAndroid()
    {
        ABUtility.ResetInfoInEditor(BuildTarget.Android);
        ClearAndSetTag.ClearAndSetAllBundleTag();
        BuildAssetBundles(BuildTarget.Android);
        BuildPlayer(BuildTarget.Android);
    }

    [MenuItem("Assets/Build/Build Windows", priority = 5)]
    public static void BuildWindows()
    {
        ABUtility.ResetInfoInEditor(BuildTarget.StandaloneWindows64);
        ClearAndSetTag.ClearAndSetAllBundleTag();
        BuildAssetBundles(BuildTarget.StandaloneWindows64);
        BuildPlayer(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Assets/Build/Build IOS", priority = 6)]
    public static void BuildIOS()
    {
        ABUtility.ResetInfoInEditor(BuildTarget.iOS);
        ClearAndSetTag.ClearAndSetAllBundleTag();
        BuildAssetBundles(BuildTarget.iOS);
        BuildPlayer(BuildTarget.iOS);
    }

    #region 构建AB包
    public static void BuildAssetBundles(BuildTarget target, AssetBundleBuild[] builds = null)
    {
        string outputPath = ABUtility.ABRelativePath;
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var options = BuildAssetBundleOptions.ChunkBasedCompression;
        bool shouldCheckODR = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
#if UNITY_TVOS
            shouldCheckODR |= EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS;
#endif
        if (shouldCheckODR)
        {
#if ENABLE_IOS_ON_DEMAND_RESOURCES
                if (PlayerSettings.iOS.useOnDemandResources)
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
#if ENABLE_IOS_APP_SLICING
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
        }
        if (builds is null || builds.Length == 0)
        {
            BuildPipeline.BuildAssetBundles(outputPath, options, target);
        }
        else
        {
            BuildPipeline.BuildAssetBundles(outputPath, builds, options, target);
        }
        Debug.Log("Build AssetBundle Done!");
        ClearAndSetTag.SaveVersion(PlayerSettings.bundleVersion, PlayerSettings.applicationIdentifier);
    }
    #endregion

    #region 构建Player
    private static void BuildPlayer(BuildTarget target)
    {
        Resources.UnloadUnusedAssets();
        string targetFilePath = BuildTargetToAppName(target);
        string targetString = ABUtility.BuildTargetToString(target);
        string buildPath = string.Format("{0}/{1}/{2}", "Build", targetString, targetFilePath);

        string inStreamAssetsPath = Path.Combine(Application.streamingAssetsPath, ABUtility.ABRelativePath);
        CopyAssetBundlesTo(inStreamAssetsPath);
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = FindActiveScenes();
        options.locationPathName = buildPath;
        options.target = target;
        BuildPipeline.BuildPlayer(options);
        FileUtil.DeleteFileOrDirectory(inStreamAssetsPath);
        Debug.Log("Build Player Done!");
    }

    // 用target获取build后的app名字
    public static string BuildTargetToAppName(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "target.apk";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "target.exe";
            case BuildTarget.StandaloneOSX:
                return "target.app";
            case BuildTarget.WebGL:
            case BuildTarget.iOS:
                return "target";
            default:
                Debug.Log("Target not implemented.");
                return null;
        }
    }

    // 获取active场景名
    private static string[] FindActiveScenes()
    {
        List<string> sceneNames = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                sceneNames.Add(scene.path);
            }
        }
        return sceneNames.ToArray();
    }

    static void CopyAssetBundlesTo(string outputPath)
    {
        FileUtil.DeleteFileOrDirectory(outputPath);
        Directory.CreateDirectory(outputPath);
        var source = Path.Combine(System.Environment.CurrentDirectory, ABUtility.ABRelativePath);
        if (!System.IO.Directory.Exists(source))
            Debug.LogError("AssetBundle source not exits ! " + source);
        var destination = outputPath;
        if (System.IO.Directory.Exists(destination))
            FileUtil.DeleteFileOrDirectory(destination);
        FileUtil.CopyFileOrDirectory(source, destination);
    }
    #endregion
}