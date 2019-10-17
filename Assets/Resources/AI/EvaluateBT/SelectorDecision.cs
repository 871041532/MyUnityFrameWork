namespace EvaluateBT
{
    public class SelectorDecision : Selector
    {
        private bool m_isRun = true;

        public SelectorDecision(Node parentNode) : base(parentNode)
        {
        }

        public override bool OnEvaluate()
        {
            bool ret = base.OnEvaluate();
            m_isRun = true;
            if (m_precondition != null)
            {
                // 走到这里证明前提条件已经通过，不需要子节点的递归结果参与判断
                if (!ret)
                {
                    // 如果子节点evaluate没过，那么这个树本次tick不执行
                    m_isRun = false;
                }

                ret = true;
            }
            return ret;
        }

        public override EStatusBTRunning OnTick()
        {
            EStatusBTRunning status = EStatusBTRunning.Finish;
            if (m_isRun)
            {
                status = base.OnTick();
            }

            return status;
        }
    };
}