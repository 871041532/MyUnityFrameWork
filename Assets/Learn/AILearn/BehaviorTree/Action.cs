using System;

namespace AILearn
{
    namespace BehaviorTree
    {
        public class Action : NodeBase
        {
            public Action() : base(null)
            {
            }

            public Action(NodeBase parent)
                : base(parent)
            {
            }

            public Action SetDyOnExecute(Func<TickStatus> call)
            {
                m_dyOnExecute = call;
                return this;
            }

            public Action SetDyOnEnter(System.Action call)
            {
                m_dyOnEnter = call;
                return this;
            }

            public Action SetDyOnExit(Action<TickStatus> call)
            {
                m_dyOnExit = call;
                return this;
            }

            protected override void OnTransition()
            {
                if (m_needExit)
                {
                    OnExit(TickStatus.ErrorTransition);
                }
                SetActiveNode(null);
                m_status = ActionStatus.Ready;
                m_needExit = false;
            }

            protected override TickStatus OnTick()
            {
                TickStatus tickStatus = TickStatus.Finish;

                if (m_status == ActionStatus.Ready)
                {
                    OnEnter();
                    m_needExit = true;
                    m_status = ActionStatus.Running;
                    SetActiveNode(this);
                }

                if (m_status == ActionStatus.Running)
                {
                    tickStatus = OnExecute();
                    SetActiveNode(this);
                    if (tickStatus == TickStatus.Finish || tickStatus == TickStatus.ErrorTransition)
                    {
                        m_status = ActionStatus.Finish;
                    }          
                }

                if (m_status == ActionStatus.Finish)
                {
                    if (m_needExit)
                    {
                        OnExit(tickStatus); 
                    }
                    m_status = ActionStatus.Ready;
                    m_needExit = false;
                    SetActiveNode(null);
                    return tickStatus;
                }

                return tickStatus;
            }

            protected override bool OnEvaluate()
            {
                return true;
            }

            protected virtual void OnEnter()
            {
                m_dyOnEnter?.Invoke();
            }

            protected virtual TickStatus OnExecute()
            {
                TickStatus tickStatus = TickStatus.Finish;
                if (m_dyOnExecute != null)
                {
                    tickStatus = m_dyOnExecute();
                }

                return tickStatus;
            }

            protected virtual void OnExit(TickStatus status)
            {
                m_dyOnExit?.Invoke(status);
            }

            private ActionStatus m_status = ActionStatus.Ready;
            private bool m_needExit = false;
            // 动态设置的OnExecute方法
            private Func<TickStatus> m_dyOnExecute = null;
            // 动态设置的OnEnter方法
            private System.Action m_dyOnEnter = null;
            // 动态设置的OnExit方法
            private Action<TickStatus> m_dyOnExit = null;
        };
    }
}