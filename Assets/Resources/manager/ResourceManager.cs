using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class ResourceManager : IManager
{
    public override void Awake() {
     }
    public override void Start() { }
    public override void Update() { }
    public override void OnDestroy() { }
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
}