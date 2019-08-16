using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class MyBuildApp : ScriptableObject
{
    [MenuItem("Assets/Build/Build AssetBundles", priority = 2)]
    public static void BuildAssetBundle()
    {
        ABUtility.ResetInfoInEditor(EditorUserBuildSettings.activeBuildTarget);
        ClearAndSetTag.ClearAndSetAllBundleTag();
        BuildAssetBundles(EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("Assets/Build/Build ActivePlayer", priority = 3)]
    public static void BuildCurrentPlatform()
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
    public static void BuildIos()
    {
        ABUtility.ResetInfoInEditor(BuildTarget.iOS);
        ClearAndSetTag.ClearAndSetAllBundleTag();
        BuildAssetBundles(BuildTarget.iOS);
        BuildPlayer(BuildTarget.iOS);
    }

    [MenuItem("Assets/Build/Build ActiveEditorPlayer", priority = 7)]
    public static void BuildCurrentEditorPlatform()
    {
        ABUtility.ResetInfoInEditor(EditorUserBuildSettings.activeBuildTarget);
        ClearAndSetTag.ClearAndSetAllBundleTag();
        BuildAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        BuildPlayer(EditorUserBuildSettings.activeBuildTarget, true);
    }

    #region 构建AB包
    static void BuildAssetBundles(BuildTarget target, AssetBundleBuild[] builds = null)
    {
        string outputPath = ABUtility.ABRelativePath;
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var options = BuildAssetBundleOptions.ChunkBasedCompression;
        bool shouldCheckOdr = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
#if UNITY_TVOS
            shouldCheckODR |= EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS;
#endif
        if (shouldCheckOdr)
        {
#if ENABLE_IOS_ON_DEMAND_RESOURCES
                if (PlayerSettings.iOS.useOnDemandResources)
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
#if ENABLE_IOS_APP_SLICING
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
        }

        try
        {
            PreProcessLua();
            if (builds is null || builds.Length == 0)
            {
                BuildPipeline.BuildAssetBundles(outputPath, options, target);
            }
            else
            {
                BuildPipeline.BuildAssetBundles(outputPath, builds, options, target);
            }
            PostProcessLua();
        }
        catch (System.Exception)
        {
            PostProcessLua();
            throw;
        }

        Debug.Log("Build AssetBundle Done!");
    }

    // 处理一下Lua文件
    private static void PreProcessLua()
    {
        DirectoryInfo dInfo = new DirectoryInfo("Assets/GameData/Scripts");
        if (dInfo.Exists)
        {
            FileInfo[] fileInfos = dInfo.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfos.Length; i++)
            {
                var fileInfo = fileInfos[i];
                if (fileInfo.Name.EndsWith(".lua"))
                {
                    fileInfo.CopyTo(fileInfo.FullName + ".txt");
                    fileInfo.Delete();
                }
            }
        }
    }
    
    private static void PostProcessLua()
    {
        DirectoryInfo dInfo = new DirectoryInfo("Assets/GameData/Scripts");
        if (dInfo.Exists)
        {
            FileInfo[] fileInfos = dInfo.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfos.Length; i++)
            {
                var fileInfo = fileInfos[i];
                if (fileInfo.Name.EndsWith(".txt"))
                {
                    long idx = fileInfo.FullName.Length - 4;
                    fileInfo.CopyTo(fileInfo.FullName.Substring(0, (int)idx));
                    fileInfo.Delete();
                }
            }
        }
    }
    #endregion

    #region 构建Player
    private static void BuildPlayer(BuildTarget target, bool buildEditorPlayer = false)
    {
        Resources.UnloadUnusedAssets();
        FileUtil.DeleteFileOrDirectory(Path.Combine(Application.streamingAssetsPath, "AssetBundles/"));
        var inStreamAssetsPath = Path.Combine(Application.streamingAssetsPath, ABUtility.ABRelativePath);
        CopyAssetBundlesTo(inStreamAssetsPath);
        BundleHotFix.SaveVersionWhenBuildPlayer(PlayerSettings.bundleVersion, PlayerSettings.applicationIdentifier);
        if (!buildEditorPlayer)
        {
            var targetFilePath = BuildTargetToAppName(target);
            var targetString = ABUtility.BuildTargetToString(target);
            var buildPath = $"Build/{targetString}/{targetFilePath}";
            var options = new BuildPlayerOptions
            {
                scenes = FindActiveScenes(), 
                locationPathName = buildPath, 
                target = target
            };
            BuildPipeline.BuildPlayer(options);
            FileUtil.DeleteFileOrDirectory(inStreamAssetsPath);
        }    
        Debug.Log("Build Player Done!");
    }

    // 用target获取build后的app名字
    private static string BuildTargetToAppName(BuildTarget target)
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