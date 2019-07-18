using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClassObjectPool<T> where T:class, new()
{
    protected Stack<T> m_Pool;
    protected int m_MaxCount = -1;
    protected int m_NotRecycleCount = 0;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="maxCount">最大值，-1不限数量</param>
    /// <param name="isPreLoad">是否提前预加载，maxCount大于0</param>
    public ClassObjectPool(int maxCount = -1, bool isPreLoad = true)
    {
        m_MaxCount = maxCount;
        if (maxCount > 0 && isPreLoad)
        {
            m_Pool = new Stack<T>(maxCount);
            for (int i = 0; i < maxCount; i++)
            {
                m_Pool.Push(new T());
            }
        }
        else
        {
            m_Pool = new Stack<T>();
        }
    }

    /// <summary>
    /// 出池，不限数量则直接出，限数量则看createWhenPoolEmpty
    /// </summary>
    /// <param name="createWhenPoolEmpty">true在池已空的情况下仍然返回对象，false池空的情况下返回null</param>
    /// <returns>池中创建或拿出的对象</returns>
    public T Spawn(bool createWhenPoolEmpty = true)
    {
        // 不限数量则直接出
        if (m_MaxCount < 0)
        {
            m_NotRecycleCount++;
            if (m_Pool.Count > 0)
            {
                return m_Pool.Pop();
            }
            else
            {
                return new T();
            }     
        }
        else
        {
            // 限数量，并且没有达到最大值
            if (m_NotRecycleCount < m_MaxCount)
            {
                m_NotRecycleCount++;
                if (m_Pool.Count > 0)
                {
                    return m_Pool.Pop();
                }
                else
                {
                    return new T();
                }
            }
            // 限数量，并且达到了最大值，但是可以再出来
            else if (createWhenPoolEmpty)
            {
                Debug.Log("类池已经达到了最大值，但仍然新建类并返回。");
                return new T();
            }
            // 限数量，达到了最大值，不能再出
            else
            {
                Debug.Log("类池已经达到了最大值，不能再出了！");
                return null;
            }
        }
    }

    /// <summary>
    /// 回收，不限数量则直接收，限数量则塞满为止。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Recycle(T obj)
    {
        if (obj == null)
        {
            Debug.Log("空的object放回类池毫无意义。");
            return false;
        }
        // 不限数量
        if (m_MaxCount < 0)
        {
            m_NotRecycleCount--;
            m_NotRecycleCount = m_NotRecycleCount < 0 ? 0 : m_NotRecycleCount;
            m_Pool.Push(obj);
            return true;
        }
        else
        {
            // 限数量， 池没有塞满
            if (m_NotRecycleCount > 0)
            {
                m_NotRecycleCount--;
                m_Pool.Push(obj);
                return true;
            }
            // 限数量，池已塞满
            else
            {
                Debug.Log("池已塞满，回收无效。");
                return false;
            }
        }
    }

}
