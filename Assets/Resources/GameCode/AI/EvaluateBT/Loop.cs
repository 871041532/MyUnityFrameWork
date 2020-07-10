namespace EvaluateBT
{
    public class Loop : Node
    {
        private int m_loopCount;
        private int m_currentCount = 0;
        public static int kInfiniteLoop = -1;

        public Loop(Node parentNode, int loopCount = -1)
            : base(parentNode)
        {
            m_loopCount = loopCount;
        }

        public override bool OnEvaluate()
        {
            bool checkLoopCount = (m_loopCount == kInfiniteLoop) ||
                                  m_currentCount < m_loopCount;

            if (!checkLoopCount)
                return false;

            if (checkIndex(0))
            {
                Node child = m_childNodeList[0];
                if (child.Evaluate())
                {
                    return true;
                }
            }

            return false;
        }

        public override void OnTransition()
        {
            if (checkIndex(0))
            {
                Node child = m_childNodeList[0];
                child.Transition();
            }

            m_currentCount = 0;
        }

        public override EStatusBTRunning OnTick()
        {
            EStatusBTRunning bIsFinish = EStatusBTRunning.Finish;
            if (checkIndex(0))
            {
                Node oBN = m_childNodeList[0];
                bIsFinish = oBN.Tick();

                if (bIsFinish == EStatusBTRunning.Finish)
                {
                    if (m_loopCount != kInfiniteLoop)
                    {
                        // 有限循环
                        ++m_currentCount;
                        if (m_currentCount < m_loopCount) // 作者原版是 == 
                        {
                            bIsFinish = EStatusBTRunning.Executing;
                        }
                    }
                    else
                    {
                        // 无限循环
                        bIsFinish = EStatusBTRunning.Executing;
                    }
                }
            }

            if (bIsFinish != EStatusBTRunning.Executing)
            {
                m_currentCount = 0;
            }

            return bIsFinish;
        }
    };
}