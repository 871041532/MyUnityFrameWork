namespace AILearn
{
    namespace BehaviorTree
    {
        // 优先检测当前节点，不通过的话再从头检测
        class SelectorPriority : Selector
        {
            public SelectorPriority(NodeBase parent)
                : base(parent)
            {
            }

            protected override bool OnEvaluate()
            {
                if (CheckIndexValid(m_selectIndex))
                {
                    NodeBase child = m_children[m_selectIndex];
                    if (child.Evaluate())
                    {
                        return true;
                    }
                }
                return base.OnEvaluate();
            }
        };
    }
}