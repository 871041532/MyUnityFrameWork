namespace EvaluateBT
{
   public class Sequence : Node
    {
        private int m_currentNodeIndex = BT_InvalidChildNodeIndex;
        public Sequence(Node parentNode)
        : base(parentNode){}

        public override bool OnEvaluate()
        {
            int testNode;
            if (m_currentNodeIndex == BT_InvalidChildNodeIndex)
            {
                testNode = 0;
            }
            else
            {
                testNode = m_currentNodeIndex;
            }
            if (checkIndex(testNode))
            {
                Node child = m_childNodeList[testNode];
                if (child.Evaluate())
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnTransition()
        {
            if (checkIndex(m_currentNodeIndex))
            {
                Node child = m_childNodeList[m_currentNodeIndex];
                child.Transition();
            }
            m_currentNodeIndex = BT_InvalidChildNodeIndex;
        }

        public override EStatusBTRunning OnTick()
        {
            EStatusBTRunning bIsFinish = EStatusBTRunning.Finish;

            //First Time
            if (m_currentNodeIndex == BT_InvalidChildNodeIndex)
            {
                m_currentNodeIndex = 0;
            }

            Node child = m_childNodeList[m_currentNodeIndex];
            if (child != null)
            {
                bIsFinish = child.Tick();
            }
            if (bIsFinish == EStatusBTRunning.Finish)
            {
                ++m_currentNodeIndex;
                //sequence is over
                if (m_currentNodeIndex == m_childNodeCount)
                {
                    m_currentNodeIndex = BT_InvalidChildNodeIndex;
                }
                else
                {
                    bIsFinish = EStatusBTRunning.Executing;
                }
            }
            if (bIsFinish == EStatusBTRunning.ErrorTransition)
            {
                m_currentNodeIndex = BT_InvalidChildNodeIndex;
            }
            return bIsFinish;
        }
    };
}