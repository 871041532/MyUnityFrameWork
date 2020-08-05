namespace AILearn
{
    namespace BehaviorTree
    {
        // 每次从队首开始选一个，然后tick
        class Selector : NodeBase
        {
            public Selector(NodeBase parent) : base(parent)
            {
            }

            protected override bool OnEvaluate()
            {
                // 每次从0开始选择，赋值给newSelectIndex
                m_newSelectIndex = Global.InvalidNodeIndex;
                for (int i = 0; i < m_ChildCount; ++i)
                {
                    NodeBase child = m_children[i];
                    if (child.Evaluate())
                    {
                        m_newSelectIndex = i;
                        return true;
                    }
                }

                return false;
            }

            protected override void OnTransition()
            {
                if (CheckIndexValid(m_selectIndex))
                {
                    NodeBase child = m_children[m_selectIndex];
                    child.Transition();
                }

                m_selectIndex = Global.InvalidNodeIndex;
            }

            protected override TickStatus OnTick()
            {
                TickStatus status = TickStatus.Finish;

                // 从curIndex和lastIndex中选出可用的，赋值给lastIndex
                if (m_selectIndex != m_newSelectIndex)
                {
                    if (CheckIndexValid(m_selectIndex))
                    {
                        NodeBase child = m_children[m_selectIndex];
                        child.Transition();
                    }

                    m_selectIndex = m_newSelectIndex;
                }

                if (CheckIndexValid(m_selectIndex))
                {
                    NodeBase child = m_children[m_selectIndex];
                    status = child.Tick();

                    if (status == TickStatus.Finish)
                    {
                        m_selectIndex = Global.InvalidNodeIndex;
                    }

                }

                return status;
            }

            protected int m_newSelectIndex = Global.InvalidNodeIndex;
            protected int m_selectIndex = Global.InvalidNodeIndex;
        };

        // 在Evaluate时如果precondition过了就认为是true
        // 如果子节点Evaluate不过，那么树tick时不执行
        class SelectorNoRecursion : Selector
        {
            public SelectorNoRecursion(NodeBase parent) : base(parent)
            {
            }

            protected override bool OnEvaluate()
            {
                bool ret = base.OnEvaluate();
                if (m_preCondition != null)
                {
                    // 走到这里证明前提条件已经通过，不需要子节点的递归结果参与判断
                    if (!ret)
                    {
                        // 如果子节点evaluate没过，那么这个树本次tick不执行
                        Global.IsRecursionOk = false;
                    }

                    ret = true;
                }

                return ret;
            }
        };
    }
}