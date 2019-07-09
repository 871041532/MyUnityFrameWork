using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

public class ABManager:IManager
 {
    public static string CfgServerURL = "127.0.0.1";
    public static string CfgServerPort = "7888";
    public static string CfgManifestName = "Windows";
    public static string CfgServerLoadPath = "127.0.0.1:7888/Windows/";
    public static string CfgAssetBundleBuildPath = "AssetBundles/Windows/";

    //public static string Cfg
    public AssetBundle testAb;

    public override void Awake() {
        Init();
        SpriteAtlasManager.atlasRequested += OnAtlasRequested;
    }

    // 初始化
    public static void Init()
    {
        setAssetBundlePath();
    }

    // 设置AB包名
    private static void setAssetBundlePath()
    {
        string platformPath = "Default";
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
            platformPath = "Android";
        else if (target2== RuntimePlatform.IPhonePlayer)
            platformPath = "iOS";
        else if (target2 == RuntimePlatform.WindowsPlayer)
            platformPath = "Windows";
#endif
        CfgManifestName = platformPath;
        CfgServerLoadPath = string.Format("{0}:{1}/{2}/", CfgServerURL, CfgServerPort, platformPath);
        CfgAssetBundleBuildPath = "AssetBundles/" + platformPath;
    }

    // 自定义Altas加载
    void OnAtlasRequested(string tag, Action<SpriteAtlas> action)
    {
        Debug.Log("加载Altas：" + tag);
        string path = Path.Combine("Assets/UI/res", tag + "_sd.spriteatlas");
#if UNITY_EDITOR
        SpriteAtlas sa = UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
        action(sa);
#endif
    }

    public override void OnDestroy() { }
}

