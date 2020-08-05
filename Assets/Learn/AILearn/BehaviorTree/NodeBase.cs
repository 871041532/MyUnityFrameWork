using System;
using UnityEngine;

namespace AILearn
{
    namespace BehaviorTree
    {
        public class NodeBase
        {
            public NodeBase(NodeBase parent)
            {
                m_children = new NodeBase[Global.MaxChildNum];
                for (int i = 0; i < Global.MaxChildNum; ++i)
                {
                    m_children[i] = null;
                }

                SetParent(parent);
            }

            public void SetParent(NodeBase parent)
            {
                m_parent = parent;
            }

            public void SetPreCondition(Func<bool> precondition)
            {
                m_preCondition = precondition;
            }

            public bool Evaluate()
            {
                return (m_preCondition == null || m_preCondition()) && OnEvaluate();
            }

            public void Transition()
            {
                OnTransition();
            }

            public TickStatus Tick()
            {
                return OnTick();
            }


            public NodeBase AddChild(NodeBase childNodeBase)
            {
                if (m_ChildCount == Global.MaxChildNum)
                {
                    Debug.LogError($"The number of child BTNodes is up to {Global.MaxChildNum}.");
                    return this;
                }

                m_children[m_ChildCount] = childNodeBase;
                ++m_ChildCount;
                return this;
            }

            public NodeBase SetName(string debugName)
            {
                m_name = debugName;
                return this;
            }

            public NodeBase GetLastActiveNode()
            {
                return m_lastActiveNode;
            }

            public void SetActiveNode(NodeBase nodeBase)
            {
                m_lastActiveNode = m_curActiveNode;
                m_curActiveNode = nodeBase;
                m_parent?.SetActiveNode(nodeBase);
            }

            public string GetDebugName()
            {
                return m_name;
            }

            protected bool CheckIndexValid(int index)
            {
                return index >= 0 && index < m_ChildCount;
            }

            // -------------------------- 虚函数 ------------------------------
            protected virtual bool OnEvaluate()
            {
                return true;
            }

            protected virtual void OnTransition()
            {
            }

            protected virtual TickStatus OnTick()
            {
                return TickStatus.Finish;
            }
            // --------------------------------------------------------------

            protected readonly NodeBase[] m_children = null;
            protected int m_ChildCount = 0;
            private NodeBase m_parent = null;
            private NodeBase m_curActiveNode = null;
            private NodeBase m_lastActiveNode = null;
            protected Func<bool> m_preCondition = null;
            private string m_name = "defaultNodeName";
        };
    }
}