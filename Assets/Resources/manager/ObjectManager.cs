using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectManager : IManager
{
    private Dictionary<Type, object> m_ClassPoolDic = new Dictionary<Type, object>();

    /// <summary>
    /// 创建类对象池， 取对象之前要先调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="maxCount">最大数量为-1不限量</param>
    /// <param name="isPreLoad">是否预加载, 最大数量大于0有效</param>
    public ClassObjectPool<T> CreateOrGetClassPool<T>(int maxCount = -1, bool isPreLoad = true) where T : class, new()
    {
        Type type = typeof(T);
        object pool = null;
        m_ClassPoolDic.TryGetValue(type, out pool);
        if (pool is null)
        {
            pool = new ClassObjectPool<T>(maxCount, isPreLoad);
            m_ClassPoolDic.Add(type, pool);
        }
        return pool as ClassObjectPool<T>;
    }

    public T Spawn<T>(bool createWhenPoolEmpty = true) where T:class, new()
    {
        object poolObj = null;
        Type type = typeof(T);
        m_ClassPoolDic.TryGetValue(type, out poolObj);
        if (poolObj is null)
        {
            Debug.LogError("需要先创建类池，才能从池中获取对象！");
            return null;
        }
        else
        {
            ClassObjectPool<T> pool = poolObj as ClassObjectPool<T>;
            return pool.Spawn(createWhenPoolEmpty);
        }
    }

    public bool Recycle<T>(T obj) where T: class, new()
    {
        object poolObj = null;
        Type type = typeof(T);
        m_ClassPoolDic.TryGetValue(type, out poolObj);
        if (poolObj is null)
        {
            Debug.LogError("类池不存在无需回收！");
            return false;
        }
        else
        {
            ClassObjectPool<T> pool = poolObj as ClassObjectPool<T>;
            return pool.Recycle(obj);
        }
    }

    public override void Awake() {
    }
    public override void Start() { }
    public override void Update() { }
    public override void OnDestroy() { }
}
