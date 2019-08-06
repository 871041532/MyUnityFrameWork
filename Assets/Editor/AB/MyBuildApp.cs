using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MyBuildApp : ScriptableObject
{
    [MenuItem("Assets/Build Current Plantform")]
    public static void BuildCurrentPlantform()
    {
        //Build(EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("Assets/Build Android")]
    public static void BuildAndroid()
    {
        Build(BuildTarget.Android);
    }

    [MenuItem("Assets/Build Windows")]
    public static void BuildWindows()
    {
        Build(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Assets/Build IOS")]
    public static void BuildIOS()
    {
        Build(BuildTarget.iOS);
    }

    private static void Build(BuildTarget target)
    {
        string targetFilePath = BuildTargetToAppName(target);
        string targetString = ABUtility.BuildTargetToString(target);
        string buildPath = string.Format("{0}/{1}/{2}", "Build", targetString, targetFilePath);

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = FindActiveScenes();
        options.locationPathName = buildPath;
        options.target = target;
        BuildPipeline.BuildPlayer(options);
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

   
}