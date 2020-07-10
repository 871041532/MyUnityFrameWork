using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class AstarTest : MonoBehaviour
    {
        private Transform m_start;

        private Transform m_end;

        private List<Node> m_pathArray;
        private GameObject m_startCube;
        private GameObject m_endCube;
        private float m_elapsedTime = 0.0f;
        private float m_intervalTime = 1.0f;
        private GridManager m_gridManager;
        
        public Node m_startNode;

        public Node m_goalNode;
        
        // Start is called before the first frame update
        void Start()
        {
            m_gridManager = FindObjectOfType<GridManager>();
            m_startCube = GameObject.FindGameObjectWithTag("Start");
            m_endCube = GameObject.FindGameObjectWithTag("End");
            m_pathArray = new List<Node>();
            m_start = m_startCube.transform;
            m_end = m_endCube.transform;
            FindPath();
        }

        // Update is called once per frame
        void Update()
        {
            m_elapsedTime += Time.deltaTime;
            if (m_elapsedTime > m_intervalTime)
            {
                m_elapsedTime = 0.0f;
                FindPath();
            }
        }

        private void FindPath()
        {
            var idx = m_gridManager.ConvertWorldPosToIndex(m_start.position);
            var worldPos = m_gridManager.ConvertIndexToWorldCenterPos(idx);
           m_startNode = new Node(worldPos);
           var idx2 = m_gridManager.ConvertWorldPosToIndex(m_end.position);
           var worldPos2 = m_gridManager.ConvertIndexToWorldCenterPos(idx2);
           m_goalNode = new Node(worldPos2);
           m_pathArray = AStar.FindPath(m_startNode, m_goalNode);
        }

        private void OnDrawGizmos()
        {
            if (m_pathArray?.Count > 0)
            {
                for (int i = 0; i < m_pathArray.Count; i++)
                {
                    if (i+1 < m_pathArray.Count)
                    {
                        var node1 = m_pathArray[i];
                        var node2 = m_pathArray[i + 1];
                        Debug.DrawLine(node1.m_position, node2.m_position, Color.green);
                    }
                }
            }
        }
    }
}
