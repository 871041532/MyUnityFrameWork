using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.U2D;
using System.Runtime.Serialization.Json;
using UnityEngine.Assertions;
using XLua;
using System.Xml.Serialization;
using Object = UnityEngine.Object;

public class ABManager:IManager
 {
    // 已经加载的ABItems
    private Dictionary<string, ABItem> m_loadedABs = new Dictionary<string, ABItem>();
    // 加载中的ABItems
    private HashSet<string> m_loadingABNames = new HashSet<string>();
    // 存放 assets全路径与ab包名的依赖关系
    private Dictionary<string, string> m_assetToABNames = new Dictionary<string, string>();
    // 存放AB依赖项
    private Dictionary<string, string[]> m_ABToDependence = new Dictionary<string, string[]>();

    public ABManager()
    {
        GameMgr.m_ObjectMgr.CreateOrGetClassPool<ABItem>();
        GameMgr.m_ObjectMgr.CreateOrGetClassPool<AssetItem>();
        PreHotFix();
        SpriteAtlasManager.atlasRequested += OnAtlasRequested;
    }

    public void PreHotFix()
    {
#if UNITY_EDITOR
        ABUtility.ResetInfoInEditor(UnityEditor.EditorUserBuildSettings.activeBuildTarget, true);
#else
        ABUtility.ResetInfoInDevice(Application.platform, true);
#endif
        ResetCfg();
    }

    public override void OnPatched()
    {
        PostHotFix();
    }

     public override void OnPatchedFailed()
     {
         PostHotFix();
     }

     public void PostHotFix()
    {
        if (ABUtility.LoadMode != LoadModeEnum.DeviceFullAotAB)
        {
            return;;
        }
#if UNITY_EDITOR
        ABUtility.ResetInfoInEditor(UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#else
        ABUtility.ResetInfoInDevice(Application.platform);
#endif
        ResetCfg();
    }

    private void ResetCfg()
    {
        if (ABUtility.LoadMode != LoadModeEnum.EditorOrigin)
        {
            // asset to ab name 读取
            if (m_assetToABNames.Count > 0)
            {
                UnloadAssetBundle("configs");
                m_assetToABNames.Clear();
                m_ABToDependence.Clear();
            }

            ABItem ab = LoadAssetBundle("configs");
            AssetBundleConfig cfg2 = ab.AssetBundle.LoadAsset<AssetBundleConfig>("AssetBundleConfig.asset");

            foreach (var item in cfg2.ResDict)
            {
                m_assetToABNames.Add(item.Path, item.ABName);
            }
            foreach (var item in cfg2.ABDict)
            {
                m_ABToDependence.Add(item.Name, item.DependenceNames);
            }
        }
    }

    /// <summary>
    /// 获取AB包的引用
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    public int GetABReferencedCount(string abName)
    {
        if (string.IsNullOrEmpty(abName))
        {
            return -1;
        }
        else
        {
            ABItem item = null;
            m_loadedABs.TryGetValue(abName, out item);
            if(item !=null)
            {
                return item.ReferencedCount;
            }
            else
            {
                return -1;
            }
        }
    }

    /// <summary>
    /// 卸载AssetItem
    /// </summary>
    /// <param name="item"></param>
    public void UnloadAsset(AssetItem item)
    {
        Assert.IsFalse(item is null || item.IsDestroyed, "不可对AssetItem重复unload！");
        if (ABUtility.LoadMode != LoadModeEnum.EditorOrigin)
        {
            UnloadAssetBundle(item.ABName);
        }
        else
        {
            if (item.GameObject is null)
            {
                // AssetBundle模式交由AB包管理不需要调这个，编辑器模式清空引用，实测Audio会卸载，texture不会。所以后续在恰当的时候需要调Resources.UnloadUnUsedAssets()
                Resources.UnloadAsset(item.Object);
            }
        }
        item.Unload();
        GameMgr.m_ObjectMgr.Recycle<AssetItem>(item);
    }

    /// <summary>
    /// 同步加载AssetItem
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    public AssetItem LoadAsset(string fullPath)
    {
        switch (ABUtility.LoadMode)
        {
# if UNITY_EDITOR
            case LoadModeEnum.EditorOrigin:
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPath);
                AssetItem assetItem = GameMgr.m_ObjectMgr.Spawn<AssetItem>();
                assetItem.Init("", fullPath, obj);
                return assetItem;
            case LoadModeEnum.EditorAB:
#endif
            case LoadModeEnum.StandaloneAB:
            case LoadModeEnum.DeviceFullAotAB:
                ABItem ABItem = LoadAssetBundleByAssetName(fullPath);
                UnityEngine.Object obj2 = null;
                if (!fullPath.EndsWith(".unity"))
                {
                    obj2 = ABItem.AssetBundle.LoadAsset(fullPath);
                }   
                AssetItem assetItem2 = GameMgr.m_ObjectMgr.Spawn<AssetItem>();
                assetItem2.Init(ABItem.ABName, fullPath, obj2);
                return assetItem2;
            default:
                return null;
        }
    }

    /// <summary>
    /// 异步加载AssetItem
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="successCall"></param>
    /// <param name="failCall"></param>
    public void LoadAssetAsync(string fullPath, Action<AssetItem> successCall, Action failCall = null)
    {
        switch (ABUtility.LoadMode)
        {
# if UNITY_EDITOR
            case LoadModeEnum.EditorOrigin:
                AssetItem item = GameMgr.m_ObjectMgr.Spawn<AssetItem>();
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPath);
                item.Init("", fullPath, obj);
                successCall(item);
                break;
            case LoadModeEnum.EditorAB:
#endif
            case LoadModeEnum.StandaloneAB:
            case LoadModeEnum.DeviceFullAotAB:
                LoadAssetBundleByAssetNameAsync(fullPath, (abItem)=> {
                   GameMgr.StartCoroutine(_loadAssetFromABItemAsync(abItem, fullPath, successCall));
                });
                break;
            default:
                failCall?.Invoke();
                break;
        }
    }

    private IEnumerator _loadAssetFromABItemAsync(ABItem item, string assetFullPath, Action <AssetItem> successCall)
    {
        var request = item.AssetBundle.LoadAssetAsync(assetFullPath);
        yield return request.isDone;
        AssetItem item2 = GameMgr.m_ObjectMgr.Spawn<AssetItem>();
        item2.Init(item.ABName, assetFullPath, request.asset);
        successCall?.Invoke(item2);
    }

    private ABItem LoadAssetBundleByAssetName(string assetFullPath)
    {
        Assert.IsTrue(m_assetToABNames.ContainsKey(assetFullPath), assetFullPath + "没有对应的AB包！");
        string abName = m_assetToABNames[assetFullPath];
        ABItem abItem = LoadAssetBundle(abName);
        return abItem;
    }

    private void LoadAssetBundleByAssetNameAsync(string assetFullPath, Action<ABItem> successCall, Action failCall= null)
    {
        Assert.IsTrue(m_assetToABNames.ContainsKey(assetFullPath), assetFullPath + "没有对应的AB包！");
        string abName = m_assetToABNames[assetFullPath];
        GameMgr.StartCoroutine(LoadAssetBundleAsync(abName, successCall));
    }

    private void UnloadAssetBundle(string abName)
    {
        // 卸载本包
        ABItem item = null;
        m_loadedABs.TryGetValue(abName, out item);
        if (item != null)
        {
            item.Release();
            Debug.Log(string.Format("释放AB包：{0}  引用: {1}", abName, item.ReferencedCount));
            if (item.ReferencedCount <= 0)
            {
                item.UnLoad();
                m_loadedABs.Remove(abName);
                GameMgr.m_ObjectMgr.Recycle<ABItem>(item);
                Debug.Log(string.Format("删除AB包：{0}", abName));
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

    private ABItem LoadAssetBundle(string abName)
    {
        Debug.Log("同步加载AB包: " + abName);
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
        ABItem abItem = null;
        m_loadedABs.TryGetValue(abName, out abItem);
        if(abItem != null)
        {
            abItem.Retain();
        }
        else
        {
            string abPath = ABUtility.ABAbsolutePath + abName;
            Debug.Log("Path：" + abPath);
            AssetBundle ab = AssetBundle.LoadFromFile(abPath);
            abItem = GameMgr.m_ObjectMgr.Spawn<ABItem>();
            abItem.Init(ab, abName);
            m_loadedABs[abName] = abItem;
        }
        Debug.Log(string.Format("同步加载AB包完毕: {0} 引用 {1}", abName, abItem.ReferencedCount));
        return abItem;
    }

    private IEnumerator LoadAssetBundleAsync(string abName, Action<ABItem> successCall, Action failCall = null)
    {
        Debug.Log("异步加载AB包: " + abName);
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
        ABItem abItem = null;
        m_loadedABs.TryGetValue(abName, out abItem);
        if (abItem != null)
        {
            abItem.Retain();
            Debug.Log(string.Format("异步加载AB包完毕: {0} 引用 {1}", abName, abItem.ReferencedCount));
            successCall?.Invoke(abItem);
        }
        else
        {
            string abPath = ABUtility.ABAbsolutePath + abName;
            // 加载中
            if (m_loadingABNames.Contains(abName))
            {
                yield return m_loadedABs.ContainsKey(abName);
                abItem = m_loadedABs[abName];
                abItem.Retain();
                Debug.Log(string.Format("异步加载AB包完毕: {0} 引用 {1}", abName, abItem.ReferencedCount));
                successCall?.Invoke(abItem);
            }
            else
            {
                // 从头加载
                Debug.Log("Path：" + abPath);
                m_loadingABNames.Add(abName);
                var abRequest = AssetBundle.LoadFromFileAsync(abPath);
                yield return abRequest.isDone;
                abItem = GameMgr.m_ObjectMgr.Spawn<ABItem>();
                abItem.Init(abRequest.assetBundle, abName);
                m_loadedABs[abName] = abItem;
                m_loadingABNames.Remove(abName);
                Debug.Log(string.Format("异步加载AB包完毕: {0} 引用 {1}", abName, abItem.ReferencedCount));
                successCall?.Invoke(abItem);
            }
        }
    }

    // 自定义Altas加载
    private void OnAtlasRequested(string tag, Action<SpriteAtlas> action)
    {
        Debug.Log("加载Altas：" + tag);
        string path = string.Format("Assets/GameData/UI/res/{0}/{0}.spriteatlas", tag);
        AssetItem item = LoadAsset(path);
        action(item.SpriteAtlas);
        UnloadAsset(item);
//        LoadAssetAsync(path, (item) => {
//            action(item.SpriteAtlas);
//            UnloadAsset(item);    
//        });
    }

    class ABItem
    {
        private string m_ABName;
        private AssetBundle m_assetBundle;
        private int m_referencedCount;
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
            m_ABName = "";
            m_referencedCount = 0;
        }
        public string ABName
        {
            get { return m_ABName; }
        }
        public AssetBundle AssetBundle
        {
            get { return m_assetBundle; }
        }
        public int ReferencedCount
        {
            get { return m_referencedCount; }
        }
        public void Retain()
        {
            m_referencedCount++;
        }
        public void Release()
        {
            m_referencedCount--;
        }
    }
}

[LuaCallCSharp]
public class AssetItem
{
    private string m_ABName;
    private string m_AssetName;
    private bool isScene = false;
    public bool IsScene => isScene;
    private bool isDestroyed = false;
    public bool IsDestroyed => isDestroyed;
    private UnityEngine.Object m_Object;

    public void Init(string abName, string assertName, UnityEngine.Object obj)
    {
        isDestroyed = false;
        isScene = assertName.EndsWith(".unity");
        bool right = (isScene) || (!isScene && obj != null);
        Assert.IsTrue(right, "AssetItem的Init函数obj传了null！");
        m_ABName = abName;
        m_AssetName = assertName;
        m_Object = obj;
    }

    public void Unload()
    {
        isDestroyed = true;
        m_AssetName = "";
        m_ABName = "";
        m_Object = null;
    }

    public int ABReferencedCount
    {
        get { return GameManager.Instance.m_ABMgr.GetABReferencedCount(m_ABName); }
    }

    public string ABName
    {
        get { return m_ABName; }
    }

    public string AssetName
    {
        get { return m_AssetName; }
    }

    public UnityEngine.Object Object
    {
        get { return m_Object; }
    }

    public GameObject GameObject
    {
        get { return m_Object as GameObject; }
    }

    public SpriteAtlas SpriteAtlas
    {
        get { return m_Object as SpriteAtlas; }
    }

    public AudioClip AudioClip
    {
        get { return m_Object as AudioClip; }
    }

    public Sprite Sprite
    {
        get { return m_Object as Sprite; }
    }

    public TextAsset TextAsset
    {
        get { return m_Object as TextAsset; }
    }
}

