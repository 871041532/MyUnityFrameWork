namespace AILearn
{
    namespace BehaviorTree
    {
        // 只要一个子节点，可以设置循环N次或无数次
        class Loop : NodeBase
        {
            public Loop(NodeBase parent, int loopCount = Loop.m_infiniteLoop)
                : base(parent)
            {
                m_loopCount = loopCount;
            }

            protected override bool OnEvaluate()
            {
                bool checkLoopCount = (m_loopCount == m_infiniteLoop) || m_currentCount < m_loopCount;

                if (!checkLoopCount)
                {
                    return false;
                }
                
                if (CheckIndexValid(0))
                {
                    NodeBase child = m_children[0];
                    if (child.Evaluate())
                    {
                        return true;
                    }     
                }

                return false;
            }

            protected override void OnTransition()
            {
                if (CheckIndexValid(0))
                {
                    NodeBase child = m_children[0];
                    child.Transition();
                }

                m_currentCount = 0;
            }

            protected override TickStatus OnTick()
            {
                TickStatus tickStatus = TickStatus.Finish;
                if (CheckIndexValid(0))
                {
                    NodeBase child = m_children[0];
                    tickStatus = child.Tick();

                    if (tickStatus == TickStatus.Finish)
                    {
                        if (m_loopCount != m_infiniteLoop)
                        {
                            // 有限循环
                            ++m_currentCount;
                            if (m_currentCount < m_loopCount) // AI分享站 C++版是 ==  
                            {
                                tickStatus = TickStatus.Executing;
                            }
                        }
                        else
                        {
                            // 无限循环
                            tickStatus = TickStatus.Executing;
                        }
                    }
                }

                if (tickStatus != TickStatus.Executing)
                {
                    m_currentCount = 0;
                }

                return tickStatus;
            }

            private int m_loopCount;
            private int m_currentCount = 0;
            private const int m_infiniteLoop = -1;
        };
    }
}