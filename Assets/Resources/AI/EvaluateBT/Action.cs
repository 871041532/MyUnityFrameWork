using System;

namespace EvaluateBT
{
    public class BTNodeTerminal : Node
    {
        private EStatusNodeTerminal m_Status = EStatusNodeTerminal.Ready;

        private bool m_needExit = false;

        // 动态设置的OnExecute方法
        private Func<EStatusBTRunning> m_dynamicOnExecute = null;

        // 动态设置的OnEnter方法
        private Action m_dynamicOnEnter = null;

        // 动态设置的OnExit方法
        private Action m_dynamicOnExit = null;

        public BTNodeTerminal(Node parentNode) : base(parentNode)
        {
        }
        
        public BTNodeTerminal SetDynamicOnExecute(Func<EStatusBTRunning> call)
        {
            m_dynamicOnExecute = call;
            return this;
        }

        public BTNodeTerminal SetDynamicOnEnter(Action call)
        {
            m_dynamicOnEnter = call;
            return this;
        }

        public BTNodeTerminal SetDynamicOnExit(Action call)
        {
            m_dynamicOnExit = call;
            return this;
        }

        protected override void OnTransition()
        {
            if (m_needExit)
            {
                OnExit();
            }
            SetActiveNode(null);
            m_Status = EStatusNodeTerminal.Ready;
            m_needExit = false;
        }

        protected override EStatusBTRunning OnTick()
        {
            EStatusBTRunning status = EStatusBTRunning.Finish;
            
            if (m_Status == EStatusNodeTerminal.Ready)
            {
                OnEnter();
                m_needExit = true;
                m_Status = EStatusNodeTerminal.Running;
                SetActiveNode(this);
            }

            if (m_Status == EStatusNodeTerminal.Running)
            {
                status = OnExecute();
                if (status == EStatusBTRunning.Finish || status == EStatusBTRunning.ErrorTransition)
                {
                    m_Status = EStatusNodeTerminal.Finish;
                }
            }

            if (m_Status == EStatusNodeTerminal.Finish)
            {
                if (m_needExit)
                {
                    OnExit();
                }
                m_needExit = false;
                m_Status = EStatusNodeTerminal.Ready;
                SetActiveNode(null);
                return status;
            }

            return status;
        }

        protected virtual void OnEnter()
        {
            m_dynamicOnEnter?.Invoke();
        }

        protected virtual EStatusBTRunning OnExecute()
        {
            EStatusBTRunning returnStatus = EStatusBTRunning.Finish;
            if (m_dynamicOnExecute != null)
            {
                returnStatus = m_dynamicOnExecute();
            }

            return returnStatus;
        }

        protected virtual void OnExit()
        {
            m_dynamicOnExit?.Invoke();
        }
    };
}