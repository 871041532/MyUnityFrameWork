using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

public class CMapList<T> where T: class, new()
{
    DoubleLinkedList<T> m_DLink = new DoubleLinkedList<T>();
    Dictionary<T, DoubleLinkedNode<T>> m_FindMap = new Dictionary<T,DoubleLinkedNode<T>>();
    public void InsertToHead(T t)
    {
        DoubleLinkedNode<T> node = null;
        m_FindMap.TryGetValue(t, out node);
        if (node != null)
        {
            m_DLink.AddFirst(node);
        }
        else
        {
            m_DLink.AddFirst(t);
            m_FindMap.Add(t, m_DLink.FirstNode);
        }
    }

    public T Pop()
    {
        if (m_DLink.Count > 0)
        {
            var t = m_DLink.Pop();
            m_FindMap.Remove(t);
            return t;
        }
        return null;
    }

    public void Remove(T t)
    {
        if (m_FindMap.ContainsKey(t))
        {
            var node = m_FindMap[t];
            m_DLink.Remove(node);
            m_FindMap.Remove(t);
        }
    }

    public T Last()
    {
        return m_DLink.Last;
    }

    public int Size()
    {
        return m_FindMap.Count;
    }

    public bool Find(T t)
    {
        DoubleLinkedNode<T> node = null;
        m_FindMap.TryGetValue(t, out node);
        return node != null;
    }

    public void Clear()
    {
        m_FindMap.Clear();
        m_DLink.Clear();
    }
}

public class ResourceItem
{
    public AssetItem m_AssetItem;
    public uint m_ReferenceCount;
    public float m_LastUseTime;
    public void Init(AssetItem item)
    {
        m_AssetItem = item;
        m_ReferenceCount = 1;
        m_LastUseTime = Time.time;
    }
}

public class GameObjectPool
{
    public AssetItem m_AssetItem;
    public DoubleLinkedList<GameObject> m_RecycledList;
    public DoubleLinkedList<GameObject> m_SpawnList;
    public GameObjectPool (AssetItem assetItem, DoubleLinkedList<GameObject> resourceList, DoubleLinkedList<GameObject> spawnList)
    {
        m_AssetItem = assetItem;
        m_RecycledList = resourceList;
        m_SpawnList = spawnList;
    }
}

public class ResourceManager : IManager
{
    class LoadGameObjectLambda
    {
        string m_AssetPath;
        Action<GameObject> m_Callback;
        ResourceManager m_Owner;
        public Action<AssetItem> m_LoadCall;
        public LoadGameObjectLambda()
        {
            m_LoadCall = (AssetItem item) =>
            {
                GameObject t = null;
                GameObjectPool resourcePool = null;
                m_Owner.m_GameObjectPools.TryGetValue(m_AssetPath, out resourcePool);
                if (resourcePool == null)
                {
                    DoubleLinkedList<GameObject> list = new DoubleLinkedList<GameObject>();
                    DoubleLinkedList<GameObject> list2 = new DoubleLinkedList<GameObject>();
                    resourcePool = new GameObjectPool(item, list, list2);
                    m_Owner.m_GameObjectPools.Add(m_AssetPath, resourcePool);
                }
                if (resourcePool.m_RecycledList.Count > 0)
                {
                    t = resourcePool.m_RecycledList.Pop();
                }
                else
                {
                    t = GameObject.Instantiate(resourcePool.m_AssetItem.m_Object as GameObject);
                }
                m_Callback?.Invoke(t);
                m_Callback = null;
                m_AssetPath = null;
                m_Owner.m_LambdaCache.AddLast(this);
                m_Owner = null;
            };
        }

        public void Init(ResourceManager owner, string path, Action<GameObject> callback)
        {
            m_Owner = owner;
            m_AssetPath = path;
            m_Callback = callback;
        }
    }
    Action<GameObject> endLoad = null;
    // 加载GameObject的Lambda池
    private DoubleLinkedList<LoadGameObjectLambda> m_LambdaCache;
    // GameObject池
    private Dictionary<string, GameObjectPool> m_GameObjectPools = new Dictionary<string, GameObjectPool>();

    // 引用计数为0的资源
    CMapList<ResourceItem> m_NoReferenceAssetMapList = new CMapList<ResourceItem>();
    // 使用中的资源
    Dictionary<string, ResourceItem> m_AssetDic = new Dictionary<string, ResourceItem>();

    public T LoadResource<T>(string path) where T: UnityEngine.Object
    {
        ResourceItem item = GetCacheResourceItem(path);
        return item.m_AssetItem.m_Object as T;
    }

    ResourceItem GetCacheResourceItem(string path, uint addReference = 1)
    {
        ResourceItem item = null;
        m_AssetDic.TryGetValue(path, out item);
        if (item != null)
        {
            item.m_ReferenceCount += addReference;
            item.m_LastUseTime = Time.time;
        }
        else
        {
            AssetItem assetItem = GameMgr.m_ABMgr.LoadAsset(path);
            item = new ResourceItem();
            item.Init(assetItem);
            m_AssetDic.Add(path, item);
        }
        return item;
    }

    public override void Awake(){
        m_LambdaCache = new DoubleLinkedList<LoadGameObjectLambda>();
    }

    public override void Start()
    {
        //string path = "Assets/GameData/Prefabs/c1.prefab";
        //var obj1 = SpawnGameObject("Assets/GameData/Prefabs/c1.prefab");
        //RecycleGameObject("Assets/GameData/Prefabs/c1.prefab", obj1);
        endLoad = this.loadEnd;
        SpawnGameObjectAsync("Assets/GameData/Prefabs/c1.prefab", endLoad);
    }

    public override void Update()
    {
        SpawnGameObjectAsync("Assets/GameData/Prefabs/c1.prefab", endLoad);
    }
    private void loadEnd(GameObject obj)
    {
        //RecycleGameObject("Assets/GameData/Prefabs/c1.prefab", obj);
        ClearGameObjectPool("Assets/GameData/Prefabs/c1.prefab");
    }

    public override void OnDestroy(){ }

    public GameObject SpawnGameObject(string path)
    {
        GameObject t = null;
        GameObjectPool resourcePool = null;
        m_GameObjectPools.TryGetValue(path, out resourcePool);
        if (resourcePool == null)
        {
            AssetItem assetItem = GameMgr.m_ABMgr.LoadAsset(path);
            DoubleLinkedList<GameObject> list = new DoubleLinkedList<GameObject>();
            DoubleLinkedList<GameObject> list2 = new DoubleLinkedList<GameObject>();
            resourcePool = new GameObjectPool(assetItem, list, list2);
            m_GameObjectPools.Add(path, resourcePool);
        }
        if (resourcePool.m_RecycledList.Count > 0)
        {
            t = resourcePool.m_RecycledList.Pop();
        }
        else
        {
            t = GameObject.Instantiate(resourcePool.m_AssetItem.m_Object as GameObject);
        }
        return t;
    }


    public void SpawnGameObjectAsync(string path, Action<GameObject> callback)
    {
        GameObjectPool resourcePool = null;
        m_GameObjectPools.TryGetValue(path, out resourcePool);
        if (resourcePool == null)
        {
            LoadGameObjectLambda lambdaCall = m_LambdaCache.CreateOrPop();
            lambdaCall.Init(this, path, callback);
            GameMgr.m_ABMgr.LoadAssetAsync(path, lambdaCall.m_LoadCall);
        }
        else
        {
            GameObject t = null;
            if (resourcePool.m_RecycledList.Count > 0)
            {
                t = resourcePool.m_RecycledList.Pop();
            }
            else
            {
                t = GameObject.Instantiate(resourcePool.m_AssetItem.m_Object as GameObject);
            }
            callback?.Invoke(t);
        }
    }

    public void RecycleGameObject(string path, GameObject obj)
    {
        m_GameObjectPools[path].m_RecycledList.AddLast(obj);
    }

    /// <summary>
    /// 清理path对应的缓存
    /// </summary>
    /// <param name="path"></param>
    public void ClearGameObjectPool(string path)
    {
        GameObjectPool resourcePool = null;
        m_GameObjectPools.TryGetValue(path, out resourcePool);
        if (resourcePool != null)
        {
            GameMgr.m_ABMgr.UnloadAsset(resourcePool.m_AssetItem);
            while(resourcePool.m_RecycledList.Count > 0)
            {
                var obj = resourcePool.m_RecycledList.Pop();
                UnityEngine.Object.Destroy(obj);
            }
            m_GameObjectPools.Remove(path);
        }
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