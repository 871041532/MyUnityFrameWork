using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;
using System.Diagnostics;

public class ResourceManager : IManager
{
    // 加载GameObject的Lambda池
    private DoubleLinkedList<LoadGameObjectFunc> m_LambdaCache = new DoubleLinkedList<LoadGameObjectFunc>();
    // GameObject池
    private Dictionary<string, GameObjectPool> m_GameObjectPools = new Dictionary<string, GameObjectPool>();
    // 资源池

    public override void Start()
    {
        string path = "Assets/GameData/Prefabs/c1.prefab";
        //SpawnGameObjectAsync("Assets/GameData/Prefabs/c1.prefab", LoadEnd);
    }

    public override void Update()
    {
    }

    private void LoadEnd(GameObject obj)
    {
        RecycleGameObject("Assets/GameData/Prefabs/c1.prefab", obj);
        // DestroyGameObjectPool("Assets/GameData/Prefabs/c1.prefab");
    }

    /// <summary>
    /// 从池中获取GameObject对象
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public GameObject SpawnGameObject(string path)
    {
        GameObjectPool resourcePool = null;
        m_GameObjectPools.TryGetValue(path, out resourcePool);
        if (resourcePool == null)
        {
            AssetItem item = GameMgr.m_ABMgr.LoadAsset(path);
            resourcePool = new GameObjectPool(item);
            m_GameObjectPools.Add(path, resourcePool);
        }
        return resourcePool.Spawn();
    }

    /// <summary>
    /// 从池中异步获取GameObject对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void SpawnGameObjectAsync(string path, Action<GameObject> callback)
    {
        GameObjectPool resourcePool = null;
        m_GameObjectPools.TryGetValue(path, out resourcePool);
        if (resourcePool == null)
        {
            LoadGameObjectFunc lambda = m_LambdaCache.CreateOrPop();
            lambda.Init(this, path, callback);
            GameMgr.m_ABMgr.LoadAssetAsync(path, lambda.m_LoadCall);
        }
        else
        {
            GameObject t = resourcePool.Spawn();
            callback?.Invoke(t);
        }
    }

    /// <summary>
    /// 回收GameObject对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    public void RecycleGameObject(string path, GameObject obj)
    {
        m_GameObjectPools[path].Recycle(obj);
    }

    /// <summary>
    /// 销毁Path对应的对象池
    /// </summary>
    /// <param name="path"></param>
    public void DestroyGameObjectPool(string path)
    {
        GameObjectPool pool = m_GameObjectPools[path];
        GameMgr.m_ABMgr.UnloadAsset(pool.AssetItem);
        pool.Destroy();
        m_GameObjectPools.Remove(path);
    }

    /// <summary>
    /// 清理path对应的缓存池中已经缓存但没使用的对象
    /// </summary>
    /// <param name="path"></param>
    public void ClearGameObjectPool(string path)
    {
        m_GameObjectPools[path].Clear();
    }

    class LoadGameObjectFunc
    {
        string m_AssetPath;
        Action<GameObject> m_Callback;
        ResourceManager m_Owner;
        public Action<AssetItem> m_LoadCall;
        public LoadGameObjectFunc()
        {
            m_LoadCall = (AssetItem item) =>
            {
                GameObject t = null;
                GameObjectPool resourcePool = null;
                m_Owner.m_GameObjectPools.TryGetValue(m_AssetPath, out resourcePool);
                if (resourcePool == null)
                {
                    resourcePool = new GameObjectPool(item);
                    m_Owner.m_GameObjectPools.Add(m_AssetPath, resourcePool);
                }
                t = resourcePool.Spawn();
                m_Callback?.Invoke(t);
                m_Callback = null;
                m_AssetPath = null;
                m_Owner.m_LambdaCache.AddLast(this);
                m_Owner = null;
            };
        }

        public void Init(ResourceManager owner, string assetPath, Action<GameObject> callback)
        {
            m_Owner = owner;
            m_AssetPath = assetPath;
            m_Callback = callback;
        }
    }
}

public class ResourceCache
{
    private Dictionary<string, AssetItem> m_UsedItems;
    private Dictionary<string, int> m_UsedReference;
    private Dictionary<int, string> m_ObjectHashToAssetName;
    private Dictionary<string, AssetItem> m_UnUsedItems;

    public ResourceCache()
    {
        m_UsedItems = new Dictionary<string, AssetItem>();
        m_UsedReference = new Dictionary<string, int>();
        m_ObjectHashToAssetName = new Dictionary<int, string>();
        m_UnUsedItems = new Dictionary<string, AssetItem>();
    }

    public UnityEngine.Object Load(string assetPath)
    {
        AssetItem item = null;
        if (m_UsedItems.ContainsKey(assetPath))
        {
            m_UsedReference[assetPath] += 1;
            item = m_UsedItems[assetPath];
        }
        else if (m_UnUsedItems.ContainsKey(assetPath))
        {
            item = m_UnUsedItems[assetPath];
            m_UsedItems.Add(assetPath, item);
            m_UsedReference.Add(assetPath, 1);
            m_UnUsedItems.Remove(assetPath);
        }
        else
        {
            item = GameManager.Instance.m_ABMgr.LoadAsset(assetPath);
            m_UsedItems.Add(assetPath, item);
            m_UsedReference.Add(assetPath, 1);
            m_ObjectHashToAssetName.Add(item.Object.GetHashCode(), assetPath);
        }
        return item.Object;
    }

    public void Recycle(UnityEngine.Object obj)
    {
        int code = obj.GetHashCode();
        string assetPath = m_ObjectHashToAssetName[code];
        m_UsedReference[assetPath] -= 1;
        if (m_UsedReference[assetPath] <= 0)
        {
            AssetItem item = m_UsedItems[assetPath];
            m_UnUsedItems.Add(assetPath, item);
            m_UsedItems.Remove(assetPath);
        }
    }

    public void Clear()
    {
        foreach (var key in m_UnUsedItems.Keys)
        {
            AssetItem item = m_UnUsedItems[key];
            m_ObjectHashToAssetName.Remove(item.Object.GetHashCode());
            GameManager.Instance.m_ABMgr.UnloadAsset(item);
        }
        m_UnUsedItems.Clear();
    }

    public void Destroy()
    {
        foreach (var item in m_UsedItems)
        {
            GameManager.Instance.m_ABMgr.UnloadAsset(item.Value);
        }
        foreach (var item in m_UnUsedItems)
        {
            GameManager.Instance.m_ABMgr.UnloadAsset(item.Value);
        }
        m_ObjectHashToAssetName.Clear();
        m_UnUsedItems.Clear();
        m_UsedItems.Clear();
        m_UsedReference.Clear();
    }
}

public class GameObjectPool
{
    private AssetItem m_AssetItem;
    public AssetItem AssetItem { get { return m_AssetItem; } }
    private DoubleLinkedList<GameObject> m_CacheObjects;
    private Dictionary<int, GameObject> m_SpawnedObjects;

    public GameObjectPool(AssetItem assetItem)
    {
        m_AssetItem = assetItem;
        m_CacheObjects = new DoubleLinkedList<GameObject>();
        m_SpawnedObjects = new Dictionary<int, GameObject>();
    }

    public GameObject Spawn()
    {
        GameObject obj = null;
        if (m_CacheObjects.Count > 0)
        {
            obj = m_CacheObjects.Pop();
        }
        else
        {
            obj = GameObject.Instantiate(m_AssetItem.GameObject);
        }
        m_SpawnedObjects.Add(obj.GetHashCode(), obj);
        return obj;
    }

    public void Recycle(GameObject obj)
    {
        m_CacheObjects.AddLast(obj);
        m_SpawnedObjects.Remove(obj.GetHashCode());
    }

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
        this.Clear();
        foreach (var item in m_SpawnedObjects)
        {
            UnityEngine.Object.Destroy(item.Value);
        }
        m_SpawnedObjects.Clear();
        m_AssetItem = null;
    }
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
        Assert.IsTrue(node != null && node.m_Next == null && node.m_Previous == null, "AddFirst方法要求node不为null, 并且Node不在链表中！");
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
        Assert.IsTrue(node != null && node.m_Next == null && node.m_Previous == null, "AddLast方法要求node不为null, 并且Node不在链表中！");
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