namespace AILearn
{
    namespace BehaviorTree
    {
        // 并行节点
        // 只要有一个子节点不通过则返回false，全部通过返回true
        // m_conditionEnum是或, 则一个完成就算完成了, 没完成的继续tick
        // m_conditionEnum是与，则全部完成才算完成
        class Parallel : NodeBase
        {
            public Parallel(NodeBase parent)
                : base(parent)
            {
                for (int i = 0; i < Global.MaxChildNum; ++i)
                    m_childTickStatus[i] = TickStatus.Executing;
            }

            // 只要有一个子节点不通过则返回false，全部通过返回true
            protected override bool OnEvaluate()
            {
                for (int i = 0; i < m_ChildCount; ++i)
                {
                    NodeBase child = m_children[i];
                    if (m_childTickStatus[i] == TickStatus.Executing)
                    {
                        if (!child.Evaluate())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            protected override void OnTransition()
            {
                for (int i = 0; i < Global.MaxChildNum; ++i)
                {
                    m_childTickStatus[i] = TickStatus.Executing;
                }

                for (int i = 0; i < m_ChildCount; ++i)
                {
                    NodeBase child = m_children[i];
                    child.Transition();
                }
            }

            // m_conditionEnum是或, 则一个完成就算完成了, 没完成的继续tick
            // m_conditionEnum是与，则全部完成才算完成
            protected override TickStatus OnTick()
            {
                int finishedChildCount = 0;
                for (int i = 0; i < m_ChildCount; ++i)
                {
                    NodeBase child = m_children[i];
                    if (m_conditionEnum == ParallelConditionEnum.Or)
                    {
                        if (m_childTickStatus[i] == TickStatus.Executing)
                        {
                            m_childTickStatus[i] = child.Tick();
                        }

                        if (m_childTickStatus[i] != TickStatus.Executing)
                        {
                            for (int j = 0; j < Global.MaxChildNum; ++j)
                            {
                                m_childTickStatus[j] = TickStatus.Executing;
                            }
                            return TickStatus.Finish;
                        }
                    }
                    else if (m_conditionEnum == ParallelConditionEnum.And)
                    {
                        if (m_childTickStatus[i] == TickStatus.Executing)
                        {
                            m_childTickStatus[i] = child.Tick();
                        }

                        if (m_childTickStatus[i] != TickStatus.Executing)
                        {
                            finishedChildCount++;
                        }
                    }
                }

                if (finishedChildCount == m_ChildCount)
                {
                    for (int i = 0; i < Global.MaxChildNum; ++i)
                    {
                        m_childTickStatus[i] = TickStatus.Executing;
                    }
                    return TickStatus.Finish;
                }

                return TickStatus.Executing;
            }

            public Parallel SetConditionEnum(ParallelConditionEnum conditionEnum)
            {
                m_conditionEnum = conditionEnum;
                return this;
            }

            private ParallelConditionEnum m_conditionEnum = ParallelConditionEnum.Or;
            private TickStatus[] m_childTickStatus = new TickStatus[Global.MaxChildNum];
        };
    }
}