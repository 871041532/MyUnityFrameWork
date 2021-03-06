﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;
using System.Diagnostics;

public class ResourceManager : IManager
{
    // GameObject池
    private CoreCompositePool m_CompositePool = new CoreCompositePool();
    // 资源池
    private Dictionary<int, ResourceCache> m_ResourceCaches = new Dictionary<int, ResourceCache>();
    public event Action UpdataEvent;

    public override void Start()
    {
    }

    public override void Update()
    {
        UpdataEvent?.Invoke();
        if (Input.anyKey)
        {
        }
    }

    /// <summary>
    /// 从Cache中加载一个资源
    /// </summary>
    /// <returns></returns>
    public AssetItem LoadAssetFromCache(int cacheId, string assetPath)
    {
        ResourceCache cache = null;
        m_ResourceCaches.TryGetValue(cacheId, out cache);
        if (cache is null)
        {
            cache = new ResourceCache();
            m_ResourceCaches.Add(cacheId, cache);
        }
        return cache.Load(assetPath);
    }

    /// <summary>
    /// 从Cache中异步加载一个资源
    /// </summary>
    /// <returns></returns>
    public void LoadAssetFromCacheAsync(int cacheId, string assetPath, Action<AssetItem> call)
    {
        ResourceCache cache = null;
        m_ResourceCaches.TryGetValue(cacheId, out cache);
        if (cache is null)
        {
            cache = new ResourceCache();
            m_ResourceCaches.Add(cacheId, cache);
        }
        cache.LoadAsync(assetPath, call);
    }

    /// <summary>
    /// 将一个资源放回cache
    /// </summary>
    /// <param name="cacheId"></param>
    /// <param name="item"></param>
    public void RecycleAsset(int cacheId, AssetItem item)
    {
        m_ResourceCaches[cacheId].Recycle(item);
    }

    /// <summary>
    /// 清理cache中引用计数为0的资源
    /// </summary>
    /// <param name="cacheId"></param>
    public void ClearCache(int cacheId)
    {
        m_ResourceCaches[cacheId].Clear();
    }

    public void ClearAllCache()
    {
        foreach (var item in m_ResourceCaches)
        {
            item.Value.Clear();
        }
    }

    /// <summary>
    /// 清理所有资源并销毁cache
    /// </summary>
    /// <param name="cacheId"></param>
    public void DestroyCache(int cacheId)
    {
        m_ResourceCaches[cacheId].Destroy();
    }

    /// <summary>
    /// 从池中获取GameObject对象
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public GameObject SpawnGameObject(string path, Transform parent = null)
    {
        return m_CompositePool.Spawn(path, parent);
    }

    public void SpawnGameObjectAsync(string path, Action<GameObject> callback, Transform parent = null)
    {
        m_CompositePool.SpawnAsync(path, callback, parent);
    }

    /// <summary>
    /// 回收GameObject对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    public void RecycleGameObject(string path, GameObject obj)
    {
        m_CompositePool.Recycle(obj);
    }

    /// <summary>
    /// 销毁Path对应的对象池
    /// </summary>
    /// <param name="path"></param>
    public void DestroyGameObjectPool(string path, bool onlyUnUsed = false)
    {
        m_CompositePool.DestroyOne(path, onlyUnUsed);
    }

    /// <summary>
    /// 清理path对应的缓存池中已经缓存但没使用的对象
    /// </summary>
    /// <param name="path"></param>
    public void ClearGameObjectPool(string path)
    {
        m_CompositePool.ClearOne(path);
    }
}

// 按优先级加载的资源cache
public class ResourcePrioritizedCache
{
    public enum LoadPriority
    {
        Hight,
        Middle,
        Low,
        VaildNum,  // 数量没有别的意思
    }
    private ResourceCache m_Cache;
    // 正在加载的优先级列表
    private Stack<AsyncLoadProcess>[] m_LoadingResList;
    // 正在加载的dict
    private Dictionary<string, AsyncLoadProcess> m_LoadingResDict;
    // 加载过程类缓存
    private ClassObjectPool<AsyncLoadProcess> m_ProcessPool;
    // preload action
    private Action<AssetItem> m_PreloadEndCall;
    private Action m_PreloadOkCall;
    private int m_PreloadTargetNum;
    private int m_PreloadCurNum;
    // 同时加载上限，每帧补充一次
    public int m_MaxLoadingCount = 10;
    private int m_CurrentLoadingCount = 0;

    public ResourcePrioritizedCache()
    {
        m_Cache = new ResourceCache();
        m_LoadingResList = new Stack<AsyncLoadProcess>[(int)LoadPriority.VaildNum];
        for (int i = 0; i < m_LoadingResList.Length; i++)
        {
            m_LoadingResList[i] = new Stack<AsyncLoadProcess>();
        }
        m_LoadingResDict = new Dictionary<string, AsyncLoadProcess>();
        m_ProcessPool = GameManager.Instance.m_ObjectMgr.CreateOrGetClassPool<AsyncLoadProcess>();
        m_PreloadEndCall = _PreloadOk;
        RegisterTick();
    }

    private void RegisterTick()
    {
        GameManager.Instance.m_ResMgr.UpdataEvent += this.Update;
    }

    private void UnRegisterTick()
    {
        GameManager.Instance.m_ResMgr.UpdataEvent -= this.Update;
    }

    private void Update()
    {
        if (m_CurrentLoadingCount >= m_MaxLoadingCount)
        {
            return;
        }
        for (int i = 0; i < m_LoadingResList.Length; i++)
        {
            Stack<AsyncLoadProcess> processStack = m_LoadingResList[i];
            while (processStack.Count > 0)
            {
                if (m_CurrentLoadingCount >= m_MaxLoadingCount)
                {
                    return;
                }
                m_CurrentLoadingCount++;
                AsyncLoadProcess param = processStack.Pop();
                param.BeginLoad();
            }
        }
    }

    public void PreLoadAsync(List<string> assetPaths, Action okCall)
    {
        Assert.IsFalse(assetPaths is null || assetPaths.Count == 0, "PreLoadAsync参数禁止传空数组或null！");
        if (!(m_PreloadOkCall is null))
        {
            UnityEngine.Debug.LogError("ResourceCache不能在上次PreLoadAsync未完成时重复调用！");
            return;
        }
        m_PreloadOkCall = okCall;
        m_PreloadTargetNum = assetPaths.Count;
        m_PreloadCurNum = 0;
        for (int i = 0; i < assetPaths.Count; i++)
        {
            LoadAsync(assetPaths[i], m_PreloadEndCall);
        }
    }

    private void _PreloadOk(AssetItem item)
    {
        Recycle(item);
        if (++m_PreloadCurNum == m_PreloadTargetNum)
        {
            m_PreloadOkCall();
            m_PreloadCurNum = 0;
            m_PreloadTargetNum = 0;
            m_PreloadOkCall = null;
        }
    }

    public void LoadAsync(string assetPath, Action<AssetItem> call, LoadPriority priority = LoadPriority.Hight)
    {
        // 已加载
        if (m_Cache.IsHaveAsset(assetPath))
        {
            AssetItem item =  m_Cache.Load(assetPath);
            call?.Invoke(item);
        }
        // 加载中
        else if(m_LoadingResDict.ContainsKey(assetPath))
        {
            m_LoadingResDict[assetPath].AddCallback(call);
        }
        // 从头加载
        else
        {
            AsyncLoadProcess param = m_ProcessPool.Spawn();
            param.Init(assetPath, this, priority);
            param.AddCallback(call);
            m_LoadingResDict.Add(assetPath, param);
            m_LoadingResList[(int)priority].Push(param);
        }
    }

    public void Preload(string assetPath)
    {
        m_Cache.PreLoad(assetPath);
    }

    public AssetItem Load(string assetPath)
    {
        return m_Cache.Load(assetPath);
    }

    public void Recycle(AssetItem item)
    {
        m_Cache.Recycle(item);
    }

    public void Clear()
    {
        m_Cache.Clear();
    }

    public void Destroy()
    {
        m_Cache.Destroy();
        // 清理掉加载队列
        for (int i = 0; i < m_LoadingResList.Length; i++)
        {
            var stack = m_LoadingResList[i];
            while (stack.Count > 0)
            {
                AsyncLoadProcess process = stack.Pop();
                process.Reset();
                m_ProcessPool.Recycle(process);
            }
        }
        UnRegisterTick();
    }

    class AsyncLoadProcess
    {
        private string m_AssetPath;
        private LoadPriority m_Priority;
        private HashSet<Action<AssetItem>> m_CallbackSet = new HashSet<Action<AssetItem>>();
        private Dictionary<int, int> m_CallbackReference = new Dictionary<int, int>();
        private ResourcePrioritizedCache m_Owner;
        private Action<AssetItem> m_LoadedProcess;

        public void Init(string path, ResourcePrioritizedCache owner, LoadPriority priority)
        {
            m_AssetPath = path;
            m_Priority = priority;
            m_Owner = owner;
            m_LoadedProcess = LoadEnd;
        }

        public void AddCallback(Action<AssetItem> call)
        {
            int hashCode = call.GetHashCode();
            if (m_CallbackReference.ContainsKey(hashCode))
            {
                m_CallbackReference[hashCode] += 1;
                if (m_CallbackReference[hashCode] > 100)
                {
                    UnityEngine.Debug.LogError("回调数目已经大于100，检查是否停止加载时，在Update中仍不断调用LoadAsync！");
                }
            }
            else
            {
                m_CallbackSet.Add(call);
                m_CallbackReference.Add(hashCode, 1);
                if (m_CallbackSet.Count > 100)
                {
                    UnityEngine.Debug.LogError("回调数目已经大于100，检查是否停止加载时，在Update中仍不断调用LoadAsync！");
                }
            }
        }

        public void BeginLoad()
        {
            m_Owner.m_Cache.LoadAsync(m_AssetPath, m_LoadedProcess);
        }

        private void LoadEnd(AssetItem assetItem)
        {
            m_Owner.m_CurrentLoadingCount--;
            m_Owner.m_LoadingResDict.Remove(m_AssetPath);
            bool first = true;
            foreach (var callback in m_CallbackSet)
            {
                int hashCode = callback.GetHashCode();   
                while (m_CallbackReference[hashCode] > 0)
                {
                    m_CallbackReference[hashCode] -= 1;
                    if (first)
                    {
                        callback?.Invoke(assetItem);
                        first = false;
                    }
                    else
                    {
                        AssetItem temp = m_Owner.m_Cache.Load(m_AssetPath);
                        callback?.Invoke(temp);
                    }  
                }
            }
            m_Owner.m_ProcessPool.Recycle(this);
            Reset();
        }

        public void Reset()
        {
            m_AssetPath = "";
            m_Priority = LoadPriority.Hight;
            m_Owner = null;
            m_CallbackSet.Clear();
            m_CallbackReference.Clear();
            m_LoadedProcess = null;
        }
    }
}

// 资源缓存
public class ResourceCache
{
    private Dictionary<string, AssetItem> m_CacheItems;
    private Dictionary<int, int> m_CacheReference;
    private DoubleLinkedList<AsyncLoadingFunc> m_LoadingFuncs;
    private Action<AssetItem> m_PreloadEndCall;
    private Action m_PreloadOkCall;
    private int m_PreloadTargetNum;
    private int m_PreloadCurNum;
    private bool m_IsDestroyed = false;

    public bool IsDestroyed => m_IsDestroyed;

    public ResourceCache()
    {
        m_CacheItems = new Dictionary<string, AssetItem>();
        m_CacheReference = new Dictionary<int, int>();
        m_LoadingFuncs = new DoubleLinkedList<AsyncLoadingFunc>();
        m_PreloadEndCall = _PreloadOk;
    }

    public void PreLoadAsync(List<string> assetPaths, Action okCall)
    {
        Assert.IsFalse(assetPaths is null || assetPaths.Count == 0, "PreLoadAsync参数禁止传空数组或null！");
        if (!(m_PreloadOkCall is null))
        {
            UnityEngine.Debug.LogError("ResourceCache不能在上次PreLoadAsync未完成时重复调用！");
            return;
        }
        m_PreloadOkCall = okCall;
        m_PreloadTargetNum = assetPaths.Count;
        m_PreloadCurNum = 0;
        for (int i = 0; i < assetPaths.Count; i++)
        {
            LoadAsync(assetPaths[i], m_PreloadEndCall);
        }
    }

    private void _PreloadOk(AssetItem item)
    {
        Recycle(item);
        if (++m_PreloadCurNum == m_PreloadTargetNum)
        {
            m_PreloadOkCall();
            m_PreloadCurNum = 0;
            m_PreloadTargetNum = 0;
            m_PreloadOkCall = null;
        }
    }

    public void LoadAsync(string assetPath, Action<AssetItem> call)
    {
        AssetItem item = null;
        m_CacheItems.TryGetValue(assetPath, out item);
        if (item is null)
        {
            var func = m_LoadingFuncs.CreateOrPop();
            func.Init(this, assetPath, call);
            GameManager.Instance.m_ABMgr.LoadAssetAsync(assetPath, func.m_Process);  
        }
        else
        {
            m_CacheReference[item.GetHashCode()] += 1;
            call?.Invoke(item);
        }
    }

    public void PreLoad(string assetPath)
    {
        AssetItem item = Load(assetPath);
        Recycle(item);
    }

    public AssetItem Load(string assetPath)
    {
        AssetItem item = null;
        m_CacheItems.TryGetValue(assetPath, out item);
        if (item is null)
        {
            item = GameManager.Instance.m_ABMgr.LoadAsset(assetPath);
            m_CacheItems.Add(assetPath, item);
            m_CacheReference.Add(item.GetHashCode(), 1);
        }
        else
        {
            m_CacheReference[item.GetHashCode()] += 1;
        }
        return item;
    }

    public bool IsHaveAsset(string assetPath)
    {
        return m_CacheItems.ContainsKey(assetPath);
    }

    public void Recycle(AssetItem item)
    {
        int hashCode = item.GetHashCode();
        if (m_CacheReference.ContainsKey(hashCode))
        {
            m_CacheReference[hashCode] -= 1;
            if (m_CacheReference[hashCode] < 0)
            {
                UnityEngine.Debug.LogError("Cache中AssetItem Recycle多次导致引用计数小于0！");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("试图Recycle不在此Cache中的AssetItem！");
        }
    }

    public void Clear()
    {
        List<string> keys = new List<string>(m_CacheItems.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            string key = keys[i];
            AssetItem item = m_CacheItems[key];
            int hashCode = item.GetHashCode();
            if (m_CacheReference[hashCode] <= 0)
            {
                GameManager.Instance.m_ABMgr.UnloadAsset(m_CacheItems[key]);
                m_CacheItems.Remove(key);
                m_CacheReference.Remove(hashCode);
            }
        }
        //Resources.UnloadUnusedAssets();
    }

    public void Destroy()
    {
        foreach (var item in m_CacheItems)
        {
            GameManager.Instance.m_ABMgr.UnloadAsset(item.Value);
        }
        m_CacheItems.Clear();
        m_CacheReference.Clear();
        m_IsDestroyed = true;
        //Resources.UnloadUnusedAssets();
    }

    class AsyncLoadingFunc
    {
        private ResourceCache m_Owner;
        private string m_AssetPath;
        private Action<AssetItem> m_LoadedCall;
        public Action<AssetItem> m_Process;

        public void Init(ResourceCache ache, string ssetPath, Action<AssetItem> k_call)
        {
            m_Owner = ache;
            m_AssetPath = ssetPath;
            m_LoadedCall = k_call;
            m_Process = (AssetItem assetItem) =>
            {
                if (m_Owner.IsDestroyed)
                {
                    // 如果缓存已经销毁过，直接舍弃
                    GameManager.Instance.m_ABMgr.UnloadAsset(assetItem);
                    return;
                }
                AssetItem innerItem = null;
                m_Owner.m_CacheItems.TryGetValue(m_AssetPath, out innerItem);
                if (innerItem is null)
                {
                    m_Owner.m_CacheItems.Add(m_AssetPath, assetItem);
                    m_Owner.m_CacheReference.Add(assetItem.GetHashCode(), 1);
                    innerItem = assetItem;
                }
                else
                {
                    GameManager.Instance.m_ABMgr.UnloadAsset(assetItem);
                    m_Owner.m_CacheReference[innerItem.GetHashCode()] += 1;
                }
                m_Owner = null;
                m_AssetPath = "";
                m_Process = null;
                m_LoadedCall?.Invoke(innerItem);
                m_LoadedCall = null;
            };
        }
    }
}

/// <summary>
/// 聚合Pool
/// </summary>
public class CoreCompositePool
{    
    // 加载GameObject的Lambda缓存
    private DoubleLinkedList<LoadGameObjectFunc> m_LambdaCache = new DoubleLinkedList<LoadGameObjectFunc>();
    private Dictionary<string, CoreGameObjectPool> m_Pools = new Dictionary<string, CoreGameObjectPool>();
    private List<string> m_TempList = new List<string>();
    private string m_Name;

    public GameObject Spawn(string path, Transform parent = null)
    {
        CoreGameObjectPool resourcePool = null;
        m_Pools.TryGetValue(path, out resourcePool);
        if (resourcePool is null)
        {
            AssetItem item = GameManager.Instance.m_ABMgr.LoadAsset(path);
            resourcePool = new CoreGameObjectPool(item, parent);
            m_Pools.Add(path, resourcePool);
        }
        return resourcePool.Spawn();
    }

    public void SpawnAsync(string path, Action<GameObject> callback, Transform parent = null)
    {
        CoreGameObjectPool resourcePool = null;
        m_Pools.TryGetValue(path, out resourcePool);
        if (resourcePool is null)
        {
            LoadGameObjectFunc lambda = m_LambdaCache.CreateOrPop();
            lambda.Init(this, path, callback);
            GameManager.Instance.m_ABMgr.LoadAssetAsync(path, lambda.m_LoadCall);
        }
        else
        {
            GameObject t = resourcePool.Spawn();
            callback?.Invoke(t);
        }
    }

    public void Recycle(GameObject obj)
    {
        foreach (var item in m_Pools)
        {
            CoreGameObjectPool pool = item.Value;
            if (pool.IsSpawnedObj(obj))
            {
                pool.Recycle(obj);
                return;
            }
        }
        UnityEngine.Debug.LogError("试图回收不在CoreCompositePool池中的GameObject、或重复回收！");
//        m_Pools[path].Recycle(obj);
    }

    public void DestroyOne(string path, bool OnlyUnUsed = false)
    {
        CoreGameObjectPool pool = m_Pools[path];
        if ((!OnlyUnUsed) || (OnlyUnUsed && pool.NotUsed()))
        {
            GameManager.Instance.m_ABMgr.UnloadAsset(pool.AssetItem);
            pool.Destroy();
            m_Pools.Remove(path);
        }
    }

    public void ClearOne(string path)
    {
        m_Pools[path].Clear();
    }

    public void DestroyAll(bool onlyUnUsed = false)
    {
        m_TempList.Clear();
        m_TempList.AddRange(m_Pools.Keys);
        for (int i = 0; i < m_TempList.Count; i++)
        {
            DestroyOne(m_TempList[i], onlyUnUsed);
        }
    }

    public void ClearAll()
    {
        foreach (var item in m_Pools)
        {
            item.Value.Clear();
        }
    }

    class LoadGameObjectFunc
    {
        string m_AssetPath;
        Action<GameObject> m_Callback;
        private GameObject m_Parent;
        private string m_Name;
        CoreCompositePool m_Owner;
        public Action<AssetItem> m_LoadCall;
        public LoadGameObjectFunc()
        {
            m_LoadCall = (AssetItem item) =>
            {
                GameObject t = null;
                CoreGameObjectPool resourcePool = null;
                m_Owner.m_Pools.TryGetValue(m_AssetPath, out resourcePool);
                if (resourcePool is null)
                {
                    resourcePool = new CoreGameObjectPool(item);
                    m_Owner.m_Pools.Add(m_AssetPath, resourcePool);
                }
                else
                {
                    GameManager.Instance.m_ABMgr.UnloadAsset(item);
                }
                t = resourcePool.Spawn();
                m_Callback?.Invoke(t);
                m_Callback = null;
                m_AssetPath = null;
                m_Parent = null;
                m_Name = null;
                m_Owner.m_LambdaCache.AddLast(this);
                m_Owner = null;
            };
        }

        public void Init(CoreCompositePool owner, string assetPath, Action<GameObject> callback, GameObject parent=null, string name = null)
        {
            m_Owner = owner;
            m_AssetPath = assetPath;
            m_Callback = callback;
            m_Parent = parent;
            m_Name = name;
        }
    }
}

/// <summary>
/// 极简Core对象池，出入池不作任何特殊处理
/// </summary>
public class CoreGameObjectPool
{
    protected AssetItem m_AssetItem;
    protected GameObject m_OriginalObject;
    protected Transform m_ObjectParent;

    public GameObject OriginalObject { get { return m_OriginalObject; } }
    public AssetItem AssetItem { get { return m_AssetItem; } }
    protected DoubleLinkedList<GameObject> m_CacheObjects = new DoubleLinkedList<GameObject>();
    protected Dictionary<int, GameObject> m_SpawnedObjects = new Dictionary<int, GameObject>();

//    public CoreGameObjectPool(GameObject originalObject, Transform ObjParent = null)
//    {
//        m_OriginalObject = originalObject;
//        m_ObjectParent = ObjParent;
//    }

    public CoreGameObjectPool(AssetItem assetItem, Transform ObjParent = null, string poolName = null)
    {
        m_AssetItem = assetItem;
        m_ObjectParent = ObjParent;
    }

    public bool NotUsed()
    {
        return m_SpawnedObjects.Count == 0;
    }

    public GameObject Spawn()
    {
        GameObject obj = null;
        if (m_CacheObjects.Count > 0)
        {
            obj = m_CacheObjects.Pop();
        }
        else if (m_OriginalObject is null)
        {
            if (m_ObjectParent is null)
            {
                obj = GameObject.Instantiate(m_AssetItem.GameObject);
            }
            else
            {
                obj = GameObject.Instantiate(m_AssetItem.GameObject, m_ObjectParent);
            }    
        }
        else
        {
            if (m_ObjectParent is null)
            {
                obj = GameObject.Instantiate(m_OriginalObject);
            }
            else
            {
                obj = GameObject.Instantiate(m_OriginalObject, m_ObjectParent);
            }
        }
        m_SpawnedObjects.Add(obj.GetHashCode(), obj);
        OnSpawn(obj);
        return obj;
    }

    protected virtual void OnSpawn(GameObject obj) { }

    public bool IsSpawnedObj(GameObject obj)
    {
        int hashCode = obj.GetHashCode();
        return m_SpawnedObjects.ContainsKey(hashCode);
    }

    public void Recycle(GameObject obj)
    {
        int hashCode = obj.GetHashCode();
        if (m_SpawnedObjects.ContainsKey(hashCode))
        {
            m_CacheObjects.AddLast(obj);
            m_SpawnedObjects.Remove(hashCode);
            OnRecycle(obj);
        }
        else
        {
            UnityEngine.Debug.LogError("试图回收不在池中的GameObject、或重复回收！");
        }
    }

    protected virtual void OnRecycle(GameObject obj) { }

    public void Clear()
    {
        while (m_CacheObjects.Count > 0)
        {
            var obj = m_CacheObjects.Pop();
            UnityEngine.Object.Destroy(obj);
        }
    }

    public void Destroy()
    {
        while (m_CacheObjects.Count > 0)
        {
            var obj = m_CacheObjects.Pop();
            UnityEngine.Object.Destroy(obj);
        }
        foreach (var item in m_SpawnedObjects)
        {
            UnityEngine.Object.Destroy(item.Value);
        }
        m_SpawnedObjects.Clear();
        OnDestroy();
        m_AssetItem = null;
        m_OriginalObject = null;
        //Resources.UnloadUnusedAssets();
    }

    protected virtual void OnDestroy() { }
}

public class DoubleLinkedNode<T> where T:class, new()
{
    public DoubleLinkedNode<T> m_Previous = null;
    public DoubleLinkedNode<T> m_Next = null;
    private T m_Value = null;
    public T Value { get { return m_Value; } set { m_Value = value; } }
}

public class DoubleLinkedList<T> where T : class, new()
{
    private DoubleLinkedNode<T> m_Head = null;
    private DoubleLinkedNode<T> m_Tail = null;
    private int m_Count;
    private ClassObjectPool<DoubleLinkedNode<T>> m_Pool = null;
    public int Count { get { return m_Count; } }
    public DoubleLinkedNode<T> FirstNode {
        get { return m_Count > 0 ? m_Head.m_Next : null; }
    }
    public DoubleLinkedNode<T> LastNode
    {
        get { return m_Count > 0 ? m_Tail.m_Previous : null; }
    }
    public T First
    {
        get { return m_Count > 0 ? m_Head.m_Next.Value : null; }
    }
    public T Last
    {
        get { return m_Count > 0 ? m_Tail.m_Previous.Value : null; }
    }

    public DoubleLinkedList()
    {
        m_Count = 0;
        m_Head = new DoubleLinkedNode<T>();
        m_Tail = new DoubleLinkedNode<T>();
        m_Head.m_Next = m_Tail;
        m_Tail.m_Previous = m_Head;
        m_Pool = GameManager.Instance.m_ObjectMgr.CreateOrGetClassPool<DoubleLinkedNode<T>>();
    }

    public DoubleLinkedNode<T> AddFirst(T value)
    {
        DoubleLinkedNode<T> node = m_Pool.Spawn();
        node.Value = value;
        AddFirst(node);
        return node;
    }

    public void AddFirst(DoubleLinkedNode<T> node)
    {
        Assert.IsTrue(node != null && node.m_Next is null && node.m_Previous is null, "AddFirst方法要求node不为null, 并且Node不在链表中！");
        node.m_Next = m_Head.m_Next;
        node.m_Previous = m_Head;
        m_Head.m_Next = node;
        node.m_Next.m_Previous = node;
        m_Count++;
    }

    public DoubleLinkedNode<T> AddLast(T value)
    {
        DoubleLinkedNode<T> node = m_Pool.Spawn();
        node.Value = value;
        AddLast(node);
        return node;
    }

    public void AddLast(DoubleLinkedNode<T> node)
    {
        Assert.IsTrue(node != null && node.m_Next is null && node.m_Previous is null, "AddLast方法要求node不为null, 并且Node不在链表中！");
        node.m_Previous = m_Tail.m_Previous;
        node.m_Next = m_Tail;
        m_Tail.m_Previous = node;
        node.m_Previous.m_Next = node;
        m_Count++;
    }
    public void Remove(DoubleLinkedNode<T> node)
    {
        if (m_Count > 0)
        {
            Assert.IsTrue(node != null && node.m_Next != null && node.m_Previous != null, "Remove方法要求node不为null, 并且Node在链表中！");
            node.m_Previous.m_Next = node.m_Next;
            node.m_Next.m_Previous = node.m_Previous;
            node.m_Next = null;
            node.m_Previous = null;
            m_Count--;
            m_Pool.Recycle(node);
        }
    }
    public void RemoveFirst()
    {
        Remove(m_Head.m_Next);
    }
    public void RemoveLast()
    {
        Remove(m_Tail.m_Previous);
    }

    public DoubleLinkedNode<T> PopNode()
    {
        if (m_Count > 0)
        {
            var node = m_Tail.m_Previous;
            Remove(node);
            return node;
        }
        else
        {
            return null;
        }
    }

    public T CreateOrPop()
    {
        if (m_Count > 0)
        {
            return Pop();
        }
        else
        {
            return new T();
        }
    }

    public T Pop()
    {
        DoubleLinkedNode<T> node = PopNode();
        T returnData = null;
        if (node != null)
        {
            returnData = node.Value;
        }
        return returnData;
    }

    public void Clear()
    {
        for (int i = 0; i < m_Count; i++)
        {
            RemoveLast();
        }
    }
}