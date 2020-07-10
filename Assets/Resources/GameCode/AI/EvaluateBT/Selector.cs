namespace EvaluateBT
{
    public class Selector : Node
    {
        protected int m_currentSelectIndex = BT_InvalidChildNodeIndex;
        protected int m_lastSelectIndex = BT_InvalidChildNodeIndex;

        public Selector(Node parentNode)
            : base(parentNode)
        {
        }

        public override bool OnEvaluate()
        {
            m_currentSelectIndex = BT_InvalidChildNodeIndex;
            for (int i = 0; i < m_childNodeCount; ++i)
            {
                Node child = m_childNodeList[i];
                if (child.Evaluate())
                {
                    m_currentSelectIndex = i;
                    return true;
                }
            }

            return false;
        }

        public override void OnTransition()
        {
            if (checkIndex(m_currentSelectIndex))
            {
                Node child = m_childNodeList[m_currentSelectIndex];
                child.Transition();
            }

            m_lastSelectIndex = BT_InvalidChildNodeIndex;
        }

        public override EStatusBTRunning OnTick()
        {
            EStatusBTRunning status = EStatusBTRunning.Finish;

            // 从cur_index和last_index中选出可用的，赋值给last_index
            if (checkIndex(m_currentSelectIndex))
            {
                if (m_lastSelectIndex != m_currentSelectIndex) //new select result
                {
                    if (checkIndex(m_lastSelectIndex))
                    {
                        Node child = m_childNodeList[m_lastSelectIndex];
                        child.Transition(); //we need transition
                    }

                    m_lastSelectIndex = m_currentSelectIndex;
                }
            }

            if (checkIndex(m_lastSelectIndex))
            {
                //Running node
                Node child = m_childNodeList[m_lastSelectIndex];
                status = child.Tick();
                //clear variable if finish
                if (status == EStatusBTRunning.Finish)
                    m_lastSelectIndex = BT_InvalidChildNodeIndex;
            }

            return status;
        }
    };
}