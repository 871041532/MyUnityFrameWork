using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class Node : IComparable
    {
        public float m_gCost = 0.0f;

        public float m_hCost = 1.0f;

        public bool m_isObstacle = false;

        public Node m_parent = null;

        public Vector3 m_position = default;

        public Node(Vector3 pos = default)
        {
            m_position = pos;
        }

        public void MarkAsObstacle()
        {
            m_isObstacle = true;
        }

        public int CompareTo(object obj)
        {
            Node node = obj as Node;
            if (m_hCost < node.m_hCost)
            {
                return -1;
            }
            else if (m_hCost > node.m_hCost)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}

