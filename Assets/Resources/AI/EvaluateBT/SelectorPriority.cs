namespace EvaluateBT
{
    public class SelectorPriority : Selector
    {
        public SelectorPriority(Node parentNode)
            : base(parentNode)
        {
        }

        public override bool OnEvaluate()
        {
            if (checkIndex(m_currentSelectIndex))
            {
                Node child = m_childNodeList[m_currentSelectIndex];
                if (child.Evaluate())
                {
                    return true;
                }
            }

            return base.OnEvaluate();
        }
    };
}