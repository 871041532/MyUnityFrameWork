using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvaluateBT
{
    public class Node
    {
        protected static int BT_MaxBTChildNodeNum = 16;
        protected static int BT_InvalidChildNodeIndex = 16;
        protected List<Node> m_childNodeList = null;
        protected int m_childNodeCount = 0;
        protected Node m_parentNode = null;
        protected Node m_activeNode = null;
        protected Node m_lastActiveNode = null;
        protected Func<bool> m_precondition = null;
        protected string m_debugName = "defaultName";

        public Node(Node parentNode)
        {
            m_childNodeList = new List<Node>();
            setParentNode(parentNode);
        }

        public void Destroy()
        {
            for (int i = 0; i < m_childNodeCount; ++i)
            {
                m_childNodeList[i].Destroy();
            }
        }
        
        public bool Evaluate()
        {
            return (m_precondition == null || m_precondition()) && OnEvaluate();
        }

        public void Transition()
        {
            OnTransition();
        }

        public EStatusBTRunning Tick()
        {
            return OnTick();
        }

        Node AddChildNode(Node childNode)
        {
            if (m_childNodeCount == BT_MaxBTChildNodeNum)
            {
                Debug.LogError("The number of child BTNodes is up to " + BT_MaxBTChildNodeNum);
            }
            else
            {
                m_childNodeList[m_childNodeCount] = childNode;
                ++m_childNodeCount;
            }

            return this;
        }

        Node SetDebugName(string debugName)
        {
            m_debugName = debugName;
            return this;
        }

        string GetDebugName()
        {
            return m_debugName;
        }
        
        Node GetLastActiveNode()
        {
            return m_lastActiveNode;
        }

        protected void SetActiveNode(Node node)
        {
            m_lastActiveNode = m_activeNode;
            m_activeNode = node;
            if (m_parentNode != null)
                m_parentNode.SetActiveNode(node);
        }

        #region 模板方法
        protected virtual bool OnEvaluate()
        {
            return true;
        }
        
        protected virtual void OnTransition()
        {
        }
        
        protected virtual EStatusBTRunning OnTick()
        {
            return EStatusBTRunning.Finish;
        }
        #endregion
        

        protected void setParentNode(Node parentNode)
        {
            m_parentNode = parentNode;
        }

        protected bool checkIndex(int index)
        {
            return index >= 0 && index < m_childNodeCount;
        }
        
        public void SetPreCondition(Func<bool> precondition)
        {
            m_precondition = precondition;
        }
    };
}