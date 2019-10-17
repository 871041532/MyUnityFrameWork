using System.Collections.Generic;

namespace EvaluateBT
{
    public class BTNodeParallel : Node
    {
        private EParallelFinishCondition m_finishCondition = EParallelFinishCondition.Or;
        private List<EStatusBTRunning> m_childNodeStatus;
        
        public BTNodeParallel(Node parentNode)
            : base(parentNode)
        {
            m_childNodeStatus = new List<EStatusBTRunning>(BT_MaxBTChildNodeNum);
            for (int i = 0; i < BT_MaxBTChildNodeNum; ++i)
            {
                m_childNodeStatus[i] = EStatusBTRunning.Executing;
            }
        }

        public override bool OnEvaluate()
        {
            for (int i = 0; i < m_childNodeCount; ++i)
            {
                Node child = m_childNodeList[i];
                if (m_childNodeStatus[i] == EStatusBTRunning.Executing)
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

        public override void OnTransition()
        {
            for (int i = 0; i < BT_MaxBTChildNodeNum; ++i)
            {
                m_childNodeStatus[i] = EStatusBTRunning.Executing;
            }

            for (int i = 0; i < m_childNodeCount; ++i)
            {
                Node child = m_childNodeList[i];
                child.Transition();
            }
        }

        public override EStatusBTRunning OnTick()
        {
            int finishedChildCount = 0;
            for (int i = 0; i < m_childNodeCount; ++i)
            {
                Node oBN = m_childNodeList[i];
                if (m_finishCondition == EParallelFinishCondition.Or)
                {
                    if (m_childNodeStatus[i] == EStatusBTRunning.Executing)
                    {
                        m_childNodeStatus[i] = oBN.Tick();
                    }
                    if (m_childNodeStatus[i] != EStatusBTRunning.Executing)
                    {
                        for (int j = 0; j < BT_MaxBTChildNodeNum; ++j)
                        {
                            m_childNodeStatus[j] = EStatusBTRunning.Executing;
                        }
                        return EStatusBTRunning.Finish;
                    }
                }
                else if (m_finishCondition == EParallelFinishCondition.And)
                {
                    if (m_childNodeStatus[i] == EStatusBTRunning.Executing)
                    {
                        m_childNodeStatus[i] = oBN.Tick();
                    }
                    if (m_childNodeStatus[i] != EStatusBTRunning.Executing)
                    {
                        finishedChildCount++;
                    }
                }
            }
            
            if (finishedChildCount == m_childNodeCount)
            {
                for (int i = 0; i < BT_MaxBTChildNodeNum; ++i)
                {
                    m_childNodeStatus[i] = EStatusBTRunning.Executing;
                }
                return EStatusBTRunning.Finish;
            }
            return EStatusBTRunning.Executing;
        }

        public BTNodeParallel SetFinishCondition(EParallelFinishCondition condition)
        {
            m_finishCondition = condition;
            return this;
        }
    };
}