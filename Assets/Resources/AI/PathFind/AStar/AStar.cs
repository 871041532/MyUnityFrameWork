using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class AStar
    {
        public static PriorityQueue<Node> m_closedList;
        public static PriorityQueue<Node> m_openList;

        private static List<Node> CalculatePath(Node node_param)
        {
            List<Node> list = new List<Node>();
            Node node = node_param;
            while (node != null)
            {
                list.Add(node);
                node = node.m_parent;
            }
            list.Reverse();
            return list;
        }
        
        // 预估启发消耗
        private static float EstimateHeuristicCost(Node curNode, Node goalNode)
        {
            Vector3 cost = curNode.m_position - goalNode.m_position;
            return cost.magnitude;
        }

        public static List<Node> FindPath(Node start, Node goal)
        {
            m_openList = new PriorityQueue<Node>();
            m_openList.Push(start);
            start.m_gCost = 0;
            start.m_hCost = EstimateHeuristicCost(start, goal);
            m_closedList = new PriorityQueue<Node>();
            GridManager gridManager = GameObject.FindObjectOfType<GridManager>();
            if (gridManager is null)
            {
                return null;
            }
            
            Node node = null;
            while (m_openList.Length != 0)
            {
                node = m_openList.GetFirstNode();
                if (node.m_position == goal.m_position)
                {
                    return CalculatePath(node);
                }
                List<Node> neighbors = new List<Node>();
                gridManager.GetNeighbors(node, ref neighbors);

                for (int i = 0; i < neighbors.Count; i++)
                {
                    Node neighborNode = neighbors[i];
                    if (!m_closedList.Contains(neighborNode))
                    {
                        float cost = EstimateHeuristicCost(node, neighborNode);
                        float totalCost = node.m_gCost + cost;
                        float neighborNodeEstCost = EstimateHeuristicCost(neighborNode, goal);
                        neighborNode.m_gCost = totalCost;
                        neighborNode.m_parent = node;
                        neighborNode.m_hCost = totalCost + neighborNodeEstCost;
                        if (!m_openList.Contains(neighborNode))
                        {
                            m_openList.Push(neighborNode);
                        }
                    }
                }
                m_closedList.Push(node);
                m_openList.Remove(node);
            }

            if (node.m_position != goal.m_position)
            {
                Debug.LogError("Goal not found!");
                return null;
            }
            else
            {
                return CalculatePath(node);
            }
        }
    }
}