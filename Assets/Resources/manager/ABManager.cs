using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;

public class AssetBundleItem
{
    public AssetBundle m_assetBundle;
    public uint m_referencedCount;
    public event Action OnUnload;
    public AssetBundleItem(AssetBundle ab)
    {
        m_assetBundle = ab;
        m_referencedCount = 1;
    }
    public void UnLoad()
    {
        m_assetBundle.Unload(false);
        OnUnload?.Invoke();
    }
}

public enum LoadModeEnum
{  
    EditorOrigin,  // Editor下直接加载原始资源
    EditorAB,  // Editor下直接加载AB包
    StandaloneAB,  // 设备或Editor下直接使用AB包
    DeviceFullAotAB,  // 设备上预下载到PrensentData再加载
}

public class ABManager:IManager
 {
    public static LoadModeEnum CfgLoadMode = LoadModeEnum.EditorAB;

    public static string CfgServerURL = "127.0.0.1";
    public static string CfgServerPort = "7888";
    public static string CfgManifestAndPlatformName = "Windows";
    public static string CfgServerLoadPath = "127.0.0.1:7888/Windows/";
    public static string CfgAssetBundleRelativePath = "AssetBundles/Windows/";
    public static string CfgAssetBundleLoadAbsolutePath = "";
    public static string CfgstreamingAssets = "";
    static LogMode CfgLogMode = LogMode.All;

    private AssetBundleManifest m_manifest;
    Dictionary<string, AssetBundleItem> m_loadedABs = new Dictionary<string, AssetBundleItem>();
    Dictionary<string, string> m_assetToABNames = new Dictionary<string, string> {
        { "Assets/Charactar/c1.prefab", "prefabs"},
    };

    public override void Awake() {
        SpriteAtlasManager.atlasRequested += OnAtlasRequested;
        Init();

        // 测试
        var asset = LoadAsset<GameObject>("Assets/Charactar/c1.prefab");
        GameObject.Instantiate(asset);
    }

    private void Init()
    {
        if (CfgLoadMode != LoadModeEnum.EditorOrigin)
        {
            AssetBundleItem mainAB = LoadAssetBundle(CfgManifestAndPlatformName);
            AssetBundleManifest manifest = mainAB.m_assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            m_manifest = manifest;
        }
    }

    static ABManager()
    {
        setAssetBundlePath();
    }

    public T LoadAsset<T>(string fullPath) where T : UnityEngine.Object
    {
        switch (CfgLoadMode)
        {
# if UNITY_EDITOR
            case LoadModeEnum.EditorOrigin:
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);
            case LoadModeEnum.EditorAB:
#endif
            case LoadModeEnum.StandaloneAB:
                AssetBundleItem ABItem = LoadAssetBundleByAssetName(fullPath);
                return ABItem.m_assetBundle.LoadAsset<T>(fullPath);
            default:
                return null;
        }
    }

    public void LoadAssetAsync<T>(string path, Action<T> successCall, Action failCall = null) where T: UnityEngine.Object
    {
    }

    public bool LoadScene(string sceneName)
    {
        return false;
    }

    public void LoadSceneAsync(string sceneName, Action successCall, Action failCall)
    {
    }

    public AssetBundleItem LoadAssetBundleByAssetName(string assetFullPath)
    {
        string abName = m_assetToABNames[assetFullPath];
        AssetBundleItem abItem = LoadAssetBundle(abName);
        return abItem;
    }

    public void LoadAssetBundleByAssetNameAsync(string assetFullPath, Action<AssetBundleItem> successCall, Action failCall= null)
    {
    }

    public AssetBundleItem LoadAssetBundle(string abName)
    {
        Log(LogType.Info, "Loading Asset Bundle: " + abName);
        AssetBundleItem abItem = null;
        m_loadedABs.TryGetValue(abName, out abItem);
        if(abItem != null)
        {
            abItem.m_referencedCount++;
        }
        else
        {
            string abPath = CfgAssetBundleLoadAbsolutePath + abName;
            Log(LogType.Info, "Path：" + abPath);
            AssetBundle ab = AssetBundle.LoadFromFile(abPath);
            abItem = new AssetBundleItem(ab);
            m_loadedABs[abName] = abItem;
        }
        return abItem;
    }

    public void LoadAssetBundleAsync(Action<AssetBundleItem> successCall, Action failCall = null)
    {

    }

// 设置AB包名
private static void setAssetBundlePath()
    {
        string platformPath = "Default";
        CfgstreamingAssets = Application.streamingAssetsPath;
#if UNITY_EDITOR
        var target = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
        if (target == UnityEditor.BuildTarget.Android)
            platformPath = "Android";
        else if (target == UnityEditor.BuildTarget.iOS)
            platformPath = "iOS";
        else if (target == UnityEditor.BuildTarget.StandaloneWindows || target == UnityEditor.BuildTarget.StandaloneWindows64)
            platformPath = "Windows";
#else
        var target2 = Application.platform;
        if (target2 == RuntimePlatform.Android)
        {
            platformPath = "Android";
            CfgstreamingAssets = Application.dataPath + "!assets";
        }  
        else if (target2== RuntimePlatform.IPhonePlayer)
        {
            platformPath = "iOS";
        }
        else if (target2 == RuntimePlatform.WindowsPlayer)
        {
            platformPath = "Windows";
        }
        CfgLoadMode = LoadModeEnum.DeviceStandaloneAB;
#endif
        CfgManifestAndPlatformName = platformPath;
        CfgAssetBundleRelativePath = "AssetBundles/" + platformPath + "/";
        CfgServerLoadPath = string.Format("{0}:{1}/{2}/", CfgServerURL, CfgServerPort, platformPath);
        if (CfgLoadMode == LoadModeEnum.EditorAB)
        {
            CfgAssetBundleLoadAbsolutePath = Path.Combine(Environment.CurrentDirectory, CfgAssetBundleRelativePath);
        }
        else
        {
            CfgAssetBundleLoadAbsolutePath = Path.Combine(CfgstreamingAssets, CfgAssetBundleRelativePath);
        }    
    }

    // 自定义Altas加载
    void OnAtlasRequested(string tag, Action<SpriteAtlas> action)
    {
        Debug.Log("加载Altas：" + tag);
        string path = Path.Combine("Assets/UI/res", tag + ".spriteatlas");
#if UNITY_EDITOR
        SpriteAtlas sa = UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
        action(sa);
#endif
    }

    // 日志
    public static void Log(LogType logType, string text)
    {
        if (logType == LogType.Error)
            Debug.LogError("[ABMgr] " + text);
        else if (CfgLogMode == LogMode.All && logType == LogType.Warning)
            Debug.LogWarning("[ABMgr] " + text);
        else if (CfgLogMode == LogMode.All)
            Debug.Log("[ABMgr] " + text);
        GameManager.Instance.Log(text);
    }

    public override void OnDestroy() { }
}

