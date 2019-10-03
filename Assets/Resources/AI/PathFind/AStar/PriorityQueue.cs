using System;
using System.Collections.Generic;

namespace AStar
{
    public class PriorityQueue<T> where T : class, IComparable
    {
        private List<T> m_nodes = new List<T>();

        public int Length
        {
            get { return m_nodes.Count; }
        }

        public bool Contains(T obj)
        {
            return m_nodes.Contains(obj);
        }

        public T GetFirstNode()
        {
            if (m_nodes.Count > 0)
            {
                return m_nodes[0];
            }
            return null;
        }

        public void Push(T obj)
        {
            m_nodes.Add(obj);
            m_nodes.Sort();
        }

        public void Remove(T obj)
        {
            m_nodes.Remove(obj);
            m_nodes.Sort();
        }
    }
}