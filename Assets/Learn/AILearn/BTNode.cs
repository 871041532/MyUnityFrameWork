using System;
using UnityEngine;

namespace AILearn
{
    static class BTGlobal
    {
        public const int BT_MaxBTChildNodeNum = 16;
        public const int BT_InvalidChildNodeIndex = BT_MaxBTChildNodeNum;
        public static bool RECURSION_OK = true;
    }

    public enum E_ParallelFinishCondition
    {
        Or,
        And
    };

    public enum StatusBTRunning
    {
        Executing,
        Finish,
        ErrorTransition = -1,
    };

    enum StausNodeTerminal
    {
        Ready,
        Running,
        Finish,
    };

    #region condition节点

    class BTPrecondition
    {
        public virtual bool ExternalCondition()
        {
            return m_dynamicJudge == null || m_dynamicJudge();
        }

        public void SetDynamicJudge(Func<bool> call)
        {
            m_dynamicJudge = call;
        }

        private Func<bool> m_dynamicJudge = null;
    };

    class BTPreconditionAnd : BTPrecondition
    {
        public BTPreconditionAnd(BTPrecondition lhs, BTPrecondition rhs)
        {
            m_lhs = lhs;
            m_rhs = rhs;
        }

        ~BTPreconditionAnd()
        {
        }

        public override bool ExternalCondition()
        {
            return m_lhs.ExternalCondition() && m_rhs.ExternalCondition();
        }

        private readonly BTPrecondition m_lhs = null;
        private readonly BTPrecondition m_rhs = null;
    };

    class BTPreconditionOr : BTPrecondition
    {
        public BTPreconditionOr(BTPrecondition lhs, BTPrecondition rhs)
        {
            m_lhs = lhs;
            m_rhs = rhs;
        }

        public override bool ExternalCondition()
        {
            return m_lhs.ExternalCondition() || m_rhs.ExternalCondition();
        }

        private readonly BTPrecondition m_lhs = null;
        private readonly BTPrecondition m_rhs = null;
    };

    class BTPreconditionXor : BTPrecondition
    {
        public BTPreconditionXor(BTPrecondition lhs, BTPrecondition rhs)

        {
            m_lhs = lhs;
            m_rhs = rhs;
        }

        public override bool ExternalCondition()
        {
            return m_lhs.ExternalCondition() ^ m_rhs.ExternalCondition();
        }

        private readonly BTPrecondition m_lhs = null;
        private readonly BTPrecondition m_rhs = null;
    };

    #endregion


    public class BTNode
    {
        public BTNode(BTNode parentNode)
        {
            m_childNodeList = new BTNode[BTGlobal.BT_MaxBTChildNodeNum];
            for (int i = 0; i < BTGlobal.BT_MaxBTChildNodeNum; ++i)
                m_childNodeList[i] = null;

            _SetParentNode(parentNode);
        }

        public void SetPreCondition(Func<bool> precondition)
        {
            m_precondition = precondition;
        }

        public bool Evaluate()
        {
            return (m_precondition == null || m_precondition()) && OnEvaluate();
        }

        public void Transition()
        {
            OnTransition();
        }

        public StatusBTRunning Tick()
        {
            return OnTick();
        }

        //---------------------------------------------------------------
        public BTNode AddChildNode(BTNode childNode)
        {
            if (m_ChildNodeCount == BTGlobal.BT_MaxBTChildNodeNum)
            {
                Debug.LogError($"The number of child BTNodes is up to {BTGlobal.BT_MaxBTChildNodeNum}.");
                return this;
            }

            m_childNodeList[m_ChildNodeCount] = childNode;
            ++m_ChildNodeCount;
            return this;
        }

        public BTNode SetDebugName(string debugName)
        {
            m_debugName = debugName;
            return this;
        }

        public BTNode GetLastActiveNode()
        {
            return m_lastActiveNode;
        }

        public void SetActiveNode(BTNode node)
        {
            m_lastActiveNode = m_activeNode;
            m_activeNode = node;
            if (m_parentNode != null)
                m_parentNode.SetActiveNode(node);
        }

        public string GetDebugName()
        {
            return m_debugName;
        }

        //--------------------------------------------------------------
        // virtual function
        //--------------------------------------------------------------
        protected virtual bool OnEvaluate()
        {
            return true;
        }

        protected virtual void OnTransition()
        {
        }

        protected virtual StatusBTRunning OnTick()
        {
            return StatusBTRunning.Finish;
        }

        protected void _SetParentNode(BTNode parentNode)
        {
            m_parentNode = parentNode;
        }

        protected bool _bCheckIndex(int index)
        {
            return index >= 0 && index < m_ChildNodeCount;
        }

        protected BTNode[] m_childNodeList = null;
        protected int m_ChildNodeCount = 0;
        protected BTNode m_parentNode = null;
        protected BTNode m_activeNode = null;
        protected BTNode m_lastActiveNode = null;
        protected Func<bool> m_precondition = null;
        protected string m_debugName = "defaultNodeName";
    };

    class BTNodePrioritySelector : BTNode
    {
        public BTNodePrioritySelector(BTNode parentNode) : base(parentNode)
        {
        }

        public virtual bool OnEvaluate()
        {
            m_currentSelectIndex = BTGlobal.BT_InvalidChildNodeIndex;
            for (int i = 0; i < m_ChildNodeCount; ++i)
            {
                BTNode child = m_childNodeList[i];
                if (child.Evaluate())
                {
                    m_currentSelectIndex = i;
                    return true;
                }
            }

            return false;
        }

        public virtual void OnTransition()
        {
            if (_bCheckIndex(m_currentSelectIndex))
            {
                BTNode child = m_childNodeList[m_currentSelectIndex];
                child.Transition();
            }

            m_lastSelectIndex = BTGlobal.BT_InvalidChildNodeIndex;
        }

        public virtual StatusBTRunning OnTick()
        {
            StatusBTRunning bIsFinish = StatusBTRunning.Finish;
            // 从cur_index和last_index中选出可用的，赋值给last_index
            if (_bCheckIndex(m_currentSelectIndex))
            {
                if (m_lastSelectIndex != m_currentSelectIndex) //new select result
                {
                    if (_bCheckIndex(m_lastSelectIndex))
                    {
                        BTNode child = m_childNodeList[m_lastSelectIndex];
                        child.Transition(); //we need transition
                    }

                    m_lastSelectIndex = m_currentSelectIndex;
                }
            }

            if (_bCheckIndex(m_lastSelectIndex))
            {
                //Running node
                BTNode child = m_childNodeList[m_lastSelectIndex];
                bIsFinish = child.Tick();
                //clear variable if finish
                if (bIsFinish == StatusBTRunning.Finish)
                    m_lastSelectIndex = BTGlobal.BT_InvalidChildNodeIndex;
            }

            return bIsFinish;
        }

        protected int m_currentSelectIndex = BTGlobal.BT_InvalidChildNodeIndex;
        protected int m_lastSelectIndex = BTGlobal.BT_InvalidChildNodeIndex;
    };

// 在Evaluate时如果precondition过了就认为是true
// 如果子节点Evaluate不过，那么树tick时不执行
    class BTNodeNoRecursionPrioritySelector : BTNodePrioritySelector
    {
        public BTNodeNoRecursionPrioritySelector(BTNode parentNode) : base(parentNode)
        {
        }

        public virtual bool OnEvaluate()
        {
            bool ret = base.OnEvaluate();
            if (m_precondition != null)
            {
                // 走到这里证明前提条件已经通过，不需要子节点的递归结果参与判断
                if (!ret)
                {
                    // 如果子节点evaluate没过，那么这个树本次tick不执行
                    BTGlobal.RECURSION_OK = false;
                }

                ret = true;
            }

            return ret;
        }
    };

    class BTNodeNonePrioritySelector : BTNodePrioritySelector
    {
        public BTNodeNonePrioritySelector(BTNode parentNode)
            : base(parentNode)
        {
        }

        public virtual bool OnEvaluate()
        {
            if (_bCheckIndex(m_currentSelectIndex))
            {
                BTNode child = m_childNodeList[m_currentSelectIndex];
                if (child.Evaluate())
                {
                    return true;
                }
            }

            return base.OnEvaluate();
        }
    };

    class BTNodeSequence : BTNode
    {
        public BTNodeSequence(BTNode parentNode)
            : base(parentNode)
        {
        }

        public virtual bool OnEvaluate()
        {
            int testNode;
            if (m_currentNodeIndex == BTGlobal.BT_InvalidChildNodeIndex)
                testNode = 0;
            else
                testNode = m_currentNodeIndex;

            if (_bCheckIndex(testNode))
            {
                BTNode child = m_childNodeList[testNode];
                if (child.Evaluate())
                    return true;
            }

            return false;
        }

        public virtual void OnTransition()
        {
            if (_bCheckIndex(m_currentNodeIndex))
            {
                BTNode child = m_childNodeList[m_currentNodeIndex];
                child.Transition();
            }

            m_currentNodeIndex = BTGlobal.BT_InvalidChildNodeIndex;
        }

        public virtual StatusBTRunning OnTick()
        {
            StatusBTRunning bIsFinish = StatusBTRunning.Finish;

            //First Time
            if (m_currentNodeIndex == BTGlobal.BT_InvalidChildNodeIndex)
                m_currentNodeIndex = 0;

            BTNode child = m_childNodeList[m_currentNodeIndex];
            if (child != null)
            {
                bIsFinish = child.Tick();
            }

            if (bIsFinish == StatusBTRunning.Finish)
            {
                ++m_currentNodeIndex;
                //sequence is over
                if (m_currentNodeIndex == m_ChildNodeCount)
                {
                    m_currentNodeIndex = BTGlobal.BT_InvalidChildNodeIndex;
                }
                else
                {
                    bIsFinish = StatusBTRunning.Executing;
                }
            }

            if (bIsFinish == StatusBTRunning.ErrorTransition)
            {
                m_currentNodeIndex = BTGlobal.BT_InvalidChildNodeIndex;
            }

            return bIsFinish;
        }

        private int m_currentNodeIndex = BTGlobal.BT_InvalidChildNodeIndex;
    };

    public class BTNodeTerminal : BTNode
    {
        public BTNodeTerminal() : base(null)
        {
        }

        public void SetParentNode(BTNode parent)
        {
            _SetParentNode(parent);
        }

        public BTNodeTerminal(BTNode parentNode)
            : base(parentNode)
        {
        }

        public BTNodeTerminal SetDynamicOnExecute(Func<StatusBTRunning> call)
        {
            m_dynamicOnExecute = call;
            return this;
        }

        public BTNodeTerminal SetDynamicOnEnter(Action call)
        {
            m_dynamicOnEnter = call;
            return this;
        }

        public BTNodeTerminal SetDynamicOnExit(Action<StatusBTRunning> call)
        {
            m_dynamicOnExit = call;
            return this;
        }

        public virtual void OnTransition()
        {
            if (m_needExit) //call Exit if we have called Enter
                OnExit(StatusBTRunning.ErrorTransition);

            SetActiveNode(null);
            m_Status = StausNodeTerminal.Ready;
            m_needExit = false;
        }

        public virtual StatusBTRunning OnTick()
        {
            StatusBTRunning bIsFinish = StatusBTRunning.Finish;

            if (m_Status == StausNodeTerminal.Ready)
            {
                OnEnter();
                m_needExit = true;
                m_Status = StausNodeTerminal.Running;
                SetActiveNode(this);
            }

            if (m_Status == StausNodeTerminal.Running)
            {
                bIsFinish = OnExecute();
                SetActiveNode(this);
                if (bIsFinish == StatusBTRunning.Finish || bIsFinish == StatusBTRunning.ErrorTransition)
                    m_Status = StausNodeTerminal.Finish;
            }

            if (m_Status == StausNodeTerminal.Finish)
            {
                if (m_needExit) //call Exit if we have called Enter
                    OnExit(bIsFinish);
                m_Status = StausNodeTerminal.Ready;
                m_needExit = false;
                SetActiveNode(null);
                return bIsFinish;
            }

            return bIsFinish;
        }

        protected virtual bool OnEvaluate()
        {
            return true;
        }

        protected virtual void OnEnter()
        {
            if (m_dynamicOnEnter != null)
            {
                m_dynamicOnEnter();
            }
        }

        protected virtual StatusBTRunning OnExecute()
        {
            StatusBTRunning returnStatus = StatusBTRunning.Finish;
            if (m_dynamicOnExecute != null)
            {
                returnStatus = m_dynamicOnExecute();
            }

            return returnStatus;
        }

        protected virtual void OnExit(StatusBTRunning _ui_ExitID)
        {
            if (m_dynamicOnExit != null)
            {
                m_dynamicOnExit(_ui_ExitID);
            }
        }

        private StausNodeTerminal m_Status = StausNodeTerminal.Ready;

        private bool m_needExit = false;

// 动态设置的OnExecute方法
        private Func<StatusBTRunning> m_dynamicOnExecute = null;

// 动态设置的OnEnter方法
        private Action m_dynamicOnEnter = null;

// 动态设置的OnExit方法
        private Action<StatusBTRunning> m_dynamicOnExit = null;
    };

    class BTNodeParallel : BTNode
    {
        public BTNodeParallel(BTNode parentNode)
            : base(parentNode)
        {
            for (uint i = 0; i < BTGlobal.BT_MaxBTChildNodeNum; ++i)
                m_childNodeStatus[i] = StatusBTRunning.Executing;
        }

        public virtual bool OnEvaluate()
        {
            for (uint i = 0; i < m_ChildNodeCount; ++i)
            {
                BTNode child = m_childNodeList[i];
                if (m_childNodeStatus[i] == StatusBTRunning.Executing)
                {
                    if (!child.Evaluate())
                    {
                        return false;
                    }
                }
            }

// 只要有一个子节点不通过则返回false，全部通过返回true
            return true;
        }

        public virtual void OnTransition()
        {
            for (uint i = 0; i < BTGlobal.BT_MaxBTChildNodeNum; ++i)
                m_childNodeStatus[i] = StatusBTRunning.Executing;

            for (uint i = 0; i < m_ChildNodeCount; ++i)
            {
                BTNode child = m_childNodeList[i];
                child.Transition();
            }
        }

        public virtual StatusBTRunning OnTick()
        {
            int finishedChildCount = 0;
            for (int i = 0; i < m_ChildNodeCount; ++i)
            {
                BTNode oBN = m_childNodeList[i];
                if (m_finishCondition == E_ParallelFinishCondition.Or)
                {
                    if (m_childNodeStatus[i] == StatusBTRunning.Executing)
                    {
                        m_childNodeStatus[i] = oBN.Tick();
                    }

                    if (m_childNodeStatus[i] != StatusBTRunning.Executing)
                    {
                        for (int j = 0; j < BTGlobal.BT_MaxBTChildNodeNum; ++j)
                            m_childNodeStatus[j] = StatusBTRunning.Executing;
                        return StatusBTRunning.Finish;
                    }
                }
                else if (m_finishCondition == E_ParallelFinishCondition.And)
                {
                    if (m_childNodeStatus[i] == StatusBTRunning.Executing)
                    {
                        m_childNodeStatus[i] = oBN.Tick();
                    }

                    if (m_childNodeStatus[i] != StatusBTRunning.Executing)
                    {
                        finishedChildCount++;
                    }
                }
            }

            if (finishedChildCount == m_ChildNodeCount)
            {
                for (uint i = 0; i < BTGlobal.BT_MaxBTChildNodeNum; ++i)
                    m_childNodeStatus[i] = StatusBTRunning.Executing;
                return StatusBTRunning.Finish;
            }

            return StatusBTRunning.Executing;
        }

        public BTNodeParallel SetFinishCondition(E_ParallelFinishCondition condition)
        {
            m_finishCondition = condition;
            return this;
        }

        private E_ParallelFinishCondition m_finishCondition = E_ParallelFinishCondition.Or;
        private StatusBTRunning[] m_childNodeStatus = new StatusBTRunning[BTGlobal.BT_MaxBTChildNodeNum];
    };

    class BTNodeLoop : BTNode
    {
        public BTNodeLoop(BTNode parentNode, int loopCount = BTNodeLoop.kInfiniteLoop)
            : base(parentNode)
        {
            m_loopCount = loopCount;
        }

        public virtual bool OnEvaluate()
        {
            bool checkLoopCount = (m_loopCount == kInfiniteLoop) ||
                                  m_currentCount < m_loopCount;

            if (!checkLoopCount)
                return false;

            if (_bCheckIndex(0))
            {
                BTNode child = m_childNodeList[0];
                if (child.Evaluate())
                    return true;
            }

            return false;
        }

        public virtual void OnTransition()
        {
            if (_bCheckIndex(0))
            {
                BTNode child = m_childNodeList[0];
                child.Transition();
            }

            m_currentCount = 0;
        }

        public virtual StatusBTRunning OnTick()
        {
            StatusBTRunning bIsFinish = StatusBTRunning.Finish;
            if (_bCheckIndex(0))
            {
                BTNode oBN = m_childNodeList[0];
                bIsFinish = oBN.Tick();

                if (bIsFinish == StatusBTRunning.Finish)
                {
                    if (m_loopCount != kInfiniteLoop)
                    {
                        // 有限循环
                        ++m_currentCount;
                        if (m_currentCount < m_loopCount) // 作者原版是 ==  
                        {
                            bIsFinish = StatusBTRunning.Executing;
                        }
                    }
                    else
                    {
// 无限循环
                        bIsFinish = StatusBTRunning.Executing;
                    }
                }
            }

            if (bIsFinish != StatusBTRunning.Executing)
            {
                m_currentCount = 0;
            }

            return bIsFinish;
        }

        private int m_loopCount;
        private int m_currentCount = 0;
        public const int kInfiniteLoop = -1;
    };


    public static class BTNodeFactory
    {
        public static BTNode CreateParallelNode(BTNode parent, E_ParallelFinishCondition condition, string debugName)
        {
            BTNodeParallel pReturn = new BTNodeParallel(parent);
            pReturn.SetFinishCondition(condition);
            CreateNodeCommon(pReturn, parent, debugName);
            return pReturn;
        }

        public static BTNode CreatePrioritySelectorNode(BTNode parent, string debugName)
        {
            BTNodePrioritySelector pReturn = new BTNodePrioritySelector(parent);
            CreateNodeCommon(pReturn, parent, debugName);
            return pReturn;
        }

        public static BTNode CreateNonePrioritySelectorNode(BTNode parent, string debugName)
        {
            BTNodeNonePrioritySelector pReturn = new BTNodeNonePrioritySelector(parent);
            CreateNodeCommon(pReturn, parent, debugName);
            return pReturn;
        }

        public static BTNode CreateSequenceNode(BTNode parent, string debugName)
        {
            BTNodeSequence pReturn = new BTNodeSequence(parent);
            CreateNodeCommon(pReturn, parent, debugName);
            return pReturn;
        }

        public static BTNode CreateLoopNode(BTNode parent, string debugName, int loopCount)
        {
            BTNodeLoop pReturn = new BTNodeLoop(parent, loopCount);
            CreateNodeCommon(pReturn, parent, debugName);
            return pReturn;
        }

        public static BTNodeTerminal CreateTemBTNodeTerminalinalNode<T>(BTNode parent, string debugName)
            where T : BTNodeTerminal, new()
        {
            BTNodeTerminal pReturn = new T();
            pReturn.SetParentNode(parent);
            CreateNodeCommon(pReturn, parent, debugName);
            return pReturn;
        }

        private static void CreateNodeCommon(BTNode me, BTNode parent, string debugName)
        {
            if (parent != null)
                parent.AddChildNode(me);
            me.SetDebugName(debugName);
        }
    };
}