namespace AILearn
{
    namespace BehaviorTree
    {
        // evaluate当前子节点，通过则tick，没通过则transition
        class Sequence : NodeBase
        {
            public Sequence(NodeBase parent)
                : base(parent)
            {
            }

            protected override bool OnEvaluate()
            {
                int index;
                if (m_currentIndex == Global.InvalidNodeIndex)
                {
                    index = 0;
                }
                else
                {
                    index = m_currentIndex;
                }

                if (CheckIndexValid(index))
                {
                    NodeBase child = m_children[index];
                    if (child.Evaluate())
                    {
                        return true;
                    }
                }

                return false;
            }

            protected override void OnTransition()
            {
                if (CheckIndexValid(m_currentIndex))
                {
                    NodeBase child = m_children[m_currentIndex];
                    child.Transition();
                }

                m_currentIndex = Global.InvalidNodeIndex;
            }

            protected override TickStatus OnTick()
            {
                TickStatus status = TickStatus.Finish;

                //First Time
                if (m_currentIndex == Global.InvalidNodeIndex)
                    m_currentIndex = 0;

                NodeBase child = m_children[m_currentIndex];
                if (child != null)
                {
                    status = child.Tick();
                }

                if (status == TickStatus.Finish)
                {
                    ++m_currentIndex;
                    //sequence is over
                    if (m_currentIndex == m_ChildCount)
                    {
                        m_currentIndex = Global.InvalidNodeIndex;
                    }
                    else
                    {
                        status = TickStatus.Executing;
                    }
                }

                if (status == TickStatus.ErrorTransition)
                {
                    m_currentIndex = Global.InvalidNodeIndex;
                }

                return status;
            }

            private int m_currentIndex = Global.InvalidNodeIndex;
        };
    }
}