using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using System.Runtime.Serialization.Json;
using UnityEngine.Assertions;
using XLua;

[LuaCallCSharp]
public class AssetItem
{
    public string m_ABName;
    public UnityEngine.Object m_Object;
    public float m_LastUseTime = 0;
    public void Init(string abName, UnityEngine.Object obj)
    {
        Assert.IsTrue(obj != null, "AssetItem的Init函数obj传了null！");
        m_ABName = abName;
        m_Object = obj;
        m_LastUseTime = Time.time;
    }

    public void Unload()
    {
        m_ABName = "";
        m_Object = null;
        m_LastUseTime = 0;
    }

    public GameObject GetGameObject()
    {
        return m_Object as GameObject;
    }

    public SpriteAtlas GetSpriteAtlas()
    {
        return m_Object as SpriteAtlas;
    }
}

public class AssetBundleItem
{
    public string m_ABName;
    public AssetBundle m_assetBundle;
    public uint m_referencedCount;
    public void Init(AssetBundle ab, string abName)
    {
        Assert.IsTrue(ab != null, "AssetBundleItem的Init函数obj传了null！");
        m_ABName = abName;
        m_assetBundle = ab;
        m_referencedCount = 1;
    }
    public void UnLoad()
    {
        m_assetBundle.Unload(true);
        m_assetBundle = null;
        m_referencedCount = 0;
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
    // 已经加载的
    // 已经加载的ABItems
    private Dictionary<string, AssetBundleItem> m_loadedABs = new Dictionary<string, AssetBundleItem>();
    // 加载中的ABItems
    private HashSet<string> m_loadingABNames = new HashSet<string>();
    // 存放 assets全路径与ab包名的依赖关系
    private Dictionary<string, string> m_assetToABNames = new Dictionary<string, string>();
    // 存放AB依赖项
    private Dictionary<string, string[]> m_ABToDependence = new Dictionary<string, string[]>();

    public override void Awake() {
        SpriteAtlasManager.atlasRequested += OnAtlasRequested;
        GameMgr.m_ObjectMgr.CreateOrGetClassPool<AssetBundleItem>();
        GameMgr.m_ObjectMgr.CreateOrGetClassPool<AssetItem>();
        InitCfg();
    }

    public override void Start()
    {
    }

    private void InitCfg()
    {
        if (CfgLoadMode != LoadModeEnum.EditorOrigin)
        {
            // asset to ab name 读取
            AssetBundleItem abItem = LoadAssetBundle("configs");
            TextAsset asset = abItem.m_assetBundle.LoadAsset<TextAsset>("AssetBundleConfig.json");
            MemoryStream stream = new MemoryStream(asset.bytes);
            DataContractJsonSerializer jsonSerializer2 = new DataContractJsonSerializer(typeof(AssetBundleConfig));
            AssetBundleConfig cfg2 = jsonSerializer2.ReadObject(stream) as AssetBundleConfig;
            stream.Close();
            m_assetToABNames.Clear();
            foreach (var item in cfg2.ResDict)
            {
                m_assetToABNames.Add(item.Path, item.ABName);
            }
            m_ABToDependence.Clear();
            foreach (var item in cfg2.ABDict)
            {
                m_ABToDependence.Add(item.Name, item.DependenceNames);
            }
        }
    }

    static ABManager()
    {
        setAssetBundlePath();
    }

    public void UnloadAsset(AssetItem item)
    {
        Assert.IsFalse(item == null);
        UnityEngine.Object obj = item.m_Object;
         Resources.UnloadUnusedAssets();
        if (CfgLoadMode != LoadModeEnum.EditorOrigin)
        {
            UnloadAssetBundle(item.m_ABName);
        }
        item.Unload();
        GameMgr.m_ObjectMgr.Recycle<AssetItem>(item);
    }

    public AssetItem LoadAsset(string fullPath)
    {
        switch (CfgLoadMode)
        {
# if UNITY_EDITOR
            case LoadModeEnum.EditorOrigin:
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPath);
                AssetItem assetItem = GameMgr.m_ObjectMgr.Spawn<AssetItem>();
                assetItem.Init("", obj);
                return assetItem;
            case LoadModeEnum.EditorAB:
#endif
            case LoadModeEnum.StandaloneAB:
                AssetBundleItem ABItem = LoadAssetBundleByAssetName(fullPath);
                UnityEngine.Object obj2 = ABItem.m_assetBundle.LoadAsset(fullPath);
                AssetItem assetItem2 = GameMgr.m_ObjectMgr.Spawn<AssetItem>();
                assetItem2.Init(ABItem.m_ABName, obj2);
                return assetItem2;
            default:
                return null;
        }
    }

    public void LoadAssetAsync(string fullPath, Action<AssetItem> successCall, Action failCall = null)
    {
        switch (CfgLoadMode)
        {
# if UNITY_EDITOR
            case LoadModeEnum.EditorOrigin:
                AssetItem item = GameMgr.m_ObjectMgr.Spawn<AssetItem>();
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPath);
                item.Init("", obj);
                successCall(item);
                break;
            case LoadModeEnum.EditorAB:
#endif
            case LoadModeEnum.StandaloneAB:
                LoadAssetBundleByAssetNameAsync(fullPath, (abItem)=> {
                   GameMgr.StartCoroutine(_loadAssetFromABItemAsync(abItem, fullPath, successCall));
                });
                break;
            default:
                failCall?.Invoke();
                break;
        }
    }

    private IEnumerator _loadAssetFromABItemAsync(AssetBundleItem item, string assetFullPath, Action <AssetItem> successCall)
    {
        var request = item.m_assetBundle.LoadAssetAsync(assetFullPath);
        yield return request.isDone;
        AssetItem item2 = GameMgr.m_ObjectMgr.Spawn<AssetItem>();
        item2.Init(item.m_ABName, request.asset);
        successCall?.Invoke(item2);
    }

    private AssetBundleItem LoadAssetBundleByAssetName(string assetFullPath)
    {
        Assert.IsTrue(m_assetToABNames.ContainsKey(assetFullPath), assetFullPath + "没有对应的AB包！");
        string abName = m_assetToABNames[assetFullPath];
        AssetBundleItem abItem = LoadAssetBundle(abName);
        return abItem;
    }

    private void LoadAssetBundleByAssetNameAsync(string assetFullPath, Action<AssetBundleItem> successCall, Action failCall= null)
    {
        Assert.IsTrue(m_assetToABNames.ContainsKey(assetFullPath), assetFullPath + "没有对应的AB包！");
        string abName = m_assetToABNames[assetFullPath];
        GameMgr.StartCoroutine(LoadAssetBundleAsync(abName, successCall));
    }

    private void UnloadAssetBundle(string abName)
    {
        // 卸载本包
        AssetBundleItem item = null;
        m_loadedABs.TryGetValue(abName, out item);
        if (item != null)
        {
            item.m_referencedCount--;
            Log(LogType.Info, string.Format("ReleaseAB包：{0}  当前引用计数: {1}", abName, item.m_referencedCount));
            if (item.m_referencedCount <= 0)
            {
                item.UnLoad();
                m_loadedABs.Remove(abName);              GameMgr.m_ObjectMgr.Recycle<AssetBundleItem>(item);
                Log(LogType.Info, string.Format("移除AB包：{0}", abName));
            }
        }
        // 卸载依赖
        string[] dependence = null;
        m_ABToDependence.TryGetValue(abName, out dependence);
        if (dependence != null && dependence.Length > 0)
        {
            for (int i = 0; i < dependence.Length; i++)
            {
                UnloadAssetBundle(dependence[i]);
            }
        }
    }

    private AssetBundleItem LoadAssetBundle(string abName)
    {
        Log(LogType.Info, "同步加载AB包: " + abName);
        Assert.IsFalse(m_loadingABNames.Contains(abName), "在异步加载尚未结束时不能再同步加载AB包：" + abName);
        // AB包的依赖项
        string[] dependence = null;
        m_ABToDependence.TryGetValue(abName, out dependence);
        if (dependence != null && dependence.Length > 0)
        {
            for (int i = 0; i < dependence.Length; i++)
            {
                LoadAssetBundle(dependence[i]);
            }
        }
        // AB包本身
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
            abItem = GameMgr.m_ObjectMgr.Spawn<AssetBundleItem>();
            //abItem = new AssetBundleItem();
            abItem.Init(ab, abName);
            m_loadedABs[abName] = abItem;
        }
        Log(LogType.Info, string.Format("同步加载AB包完毕: {0} 引用 {1}", abName, abItem.m_referencedCount));
        return abItem;
    }

    private IEnumerator LoadAssetBundleAsync(string abName, Action<AssetBundleItem> successCall, Action failCall = null)
    {
        Log(LogType.Info, "处理异步加载AssetBundle: " + abName);
        // AB包的依赖项
        string[] dependence = null;
        m_ABToDependence.TryGetValue(abName, out dependence);
        if (dependence != null && dependence.Length > 0)
        {
            for (int i = 0; i < dependence.Length; i++)
            {
                GameMgr.StartCoroutine(LoadAssetBundleAsync(dependence[i], null));
            }
        }
        // AB包本身
        AssetBundleItem abItem = null;
        m_loadedABs.TryGetValue(abName, out abItem);
        if (abItem != null)
        {
            abItem.m_referencedCount++;
            Log(LogType.Info, string.Format("此包已存在 {0} 引用: {1}", abName, abItem.m_referencedCount));
            successCall?.Invoke(abItem);
        }
        else
        {
            string abPath = CfgAssetBundleLoadAbsolutePath + abName;
            Log(LogType.Info, "Path：" + abPath);
            // 加载中
            if (m_loadingABNames.Contains(abName))
            {
                Log(LogType.Info, "等待Other异步加载:  " + abName);
                yield return m_loadedABs.ContainsKey(abName);
                abItem = m_loadedABs[abName];
                abItem.m_referencedCount++;
                Log(LogType.Info, "Other异步加载完毕:  " + abName + " 引用: " + abItem.m_referencedCount);
                successCall?.Invoke(abItem);
            }
            else
            {
                // 从头加载
                Log(LogType.Info, "开始异步加载AB包:  " + abName);
                m_loadingABNames.Add(abName);
                var abRequest = AssetBundle.LoadFromFileAsync(abPath);
                yield return abRequest.isDone;
                abItem = GameMgr.m_ObjectMgr.Spawn<AssetBundleItem>();
                //abItem = new AssetBundleItem();
                abItem.Init(abRequest.assetBundle, abName);
                Log(LogType.Info, string.Format("异步加载AB包完毕: {0} 引用 {1}", abName, abItem.m_referencedCount));
                m_loadedABs[abName] = abItem;
                m_loadingABNames.Remove(abName);
                successCall?.Invoke(abItem);
            }
        }
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
        CfgLoadMode = LoadModeEnum.StandaloneAB;
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
    private void OnAtlasRequested(string tag, Action<SpriteAtlas> action)
    {
        Debug.Log("加载Altas：" + tag);
        string path = string.Format("Assets/GameData/UI/res/{0}/{0}.spriteatlas", tag);
#if UNITY_EDITOR
        if (CfgLoadMode == LoadModeEnum.EditorOrigin)
        {
            SpriteAtlas sa = UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            action(sa);
        }
        else if (CfgLoadMode == LoadModeEnum.EditorAB)
        {
            LoadAssetAsync(path, (item) => {
                SpriteAtlas sa = item.GetSpriteAtlas();
                action(sa);
            });
        }
        else
#endif
        if (CfgLoadMode == LoadModeEnum.StandaloneAB)
        {
            LoadAssetAsync(path, (item) => {
                SpriteAtlas sa = item.GetSpriteAtlas();
                action(sa);
            });
        }
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

