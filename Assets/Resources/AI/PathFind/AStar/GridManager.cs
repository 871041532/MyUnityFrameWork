using System;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class GridManager: MonoBehaviour
    {
        [SerializeField]
        private int m_numberOfRows = 50;  // 行
        [SerializeField]
        private int m_numberOfColumns = 50;  // 列
        [SerializeField]
        private int m_gridCellSize = 2;
        [SerializeField]
        private bool m_showGrid = true;
        [SerializeField]
        private bool m_showObstacleBlocks = true;
        private Vector3 m_origin = new Vector3();
        private GameObject[] m_obstacleList;
        private Node[,] m_nodes { get; set; }

        private void Awake()
        {
            m_obstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
            InitializeNodes();
            CalculateObstacles();
        }

        public void GetNeighbors(Node node, ref List<Node> neighbors)
        {
            Vector3 neighborPosition = node.m_position;
            int neighborIndex = ConvertWorldPosToIndex(neighborPosition);
            int row = GetRowOfIndex(neighborIndex);
            int column = GetColumnOfIndex(neighborIndex);
            
            // bottom
            AssignNeighbor(row - 1, column, ref neighbors);
            // top
            AssignNeighbor(row + 1, column, ref neighbors);
            // right
            AssignNeighbor(row, column + 1, ref neighbors);
            // left
            AssignNeighbor(row, column - 1, ref neighbors);
        }

        private void AssignNeighbor(int row, int column, ref List<Node> neighbors)
        {
            if (row != -1 && column != -1 && row < m_numberOfRows && column < m_numberOfColumns)
            {
                Node nodeToAdd = m_nodes[row, column];
                if (!nodeToAdd.m_isObstacle)
                {
                    neighbors.Add(nodeToAdd);
                }
            }
        }
        
        private void InitializeNodes()
        {
            m_nodes = new Node[m_numberOfColumns, m_numberOfRows];
            int index = 0;
            for (int i = 0; i < m_numberOfColumns; i++)
            {
                for (int j = 0; j < m_numberOfRows; j++)
                {
                    Vector3 cellPosition = ConvertIndexToWorldCenterPos(index);
                    Node node = new Node(cellPosition);
                    m_nodes[i, j] = node;
                    index++;
                }
            }
        }

        private void CalculateObstacles()
        {
            if (m_obstacleList?.Length > 0)
            {
                foreach (GameObject data in m_obstacleList)
                {
                    int indexCell = ConvertWorldPosToIndex(data.transform.position);
                    int colum = GetColumnOfIndex(indexCell);
                    int row = GetRowOfIndex(indexCell);
                    m_nodes[row, colum].MarkAsObstacle();
                }
            }
        }

        public Vector3 ConvertIndexToWorldCenterPos(int index)
        {
            Vector3 pos = ConvertIndexToWorldPos(index);
            pos.x += m_gridCellSize * 0.5f;
            pos.z += m_gridCellSize * 0.5f;
            return pos;
        }
        
        public Vector3 ConvertIndexToWorldPos(int index)
        {
            int row = GetRowOfIndex(index);
            int col = GetColumnOfIndex(index);
            return new Vector3(row * m_gridCellSize, 0, col * m_gridCellSize);
        }

        public int ConvertWorldPosToIndex(Vector3 worldPos)
        {
            int x = (int) worldPos.x;
            int z = (int) worldPos.z;
            int row = x / m_gridCellSize;
            int col = z / m_gridCellSize;
            return row * m_numberOfColumns + col;
        }

        // 获取行
        public int GetRowOfIndex(int index)
        {
            return index / m_numberOfColumns;
        }

        // 获取列
        public int GetColumnOfIndex(int index)
        {
            return index % m_numberOfColumns;
        }
    }
}