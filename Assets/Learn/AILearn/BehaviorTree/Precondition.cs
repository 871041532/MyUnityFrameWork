using System;

namespace AILearn
{
    namespace BehaviorTree
    {
        // 判断
        class Precondition
        {
            public bool Judge()
            {
                return this.OnJudge();
            }

            public void SetDyJudgeFunc(Func<bool> call)
            {
                m_dyJudge = call;
            }

            protected virtual bool OnJudge()
            {
                return m_dyJudge == null || m_dyJudge();
            }

            private Func<bool> m_dyJudge = null;
        };

        // 与
        class PreconditionAnd : Precondition
        {
            public PreconditionAnd(Precondition lhs, Precondition rhs)
            {
                m_lhs = lhs;
                m_rhs = rhs;
            }

            protected override bool OnJudge()
            {
                return m_lhs.Judge() && m_rhs.Judge();
            }

            private readonly Precondition m_lhs = null;
            private readonly Precondition m_rhs = null;
        };

        // 或
        class PreconditionOr : Precondition
        {
            public PreconditionOr(Precondition lhs, Precondition rhs)
            {
                m_lhs = lhs;
                m_rhs = rhs;
            }

            protected override bool OnJudge()
            {
                return m_lhs.Judge() || m_rhs.Judge();
            }

            private readonly Precondition m_lhs = null;
            private readonly Precondition m_rhs = null;
        };

        // 异或
        class PreconditionXor : Precondition
        {
            public PreconditionXor(Precondition lhs, Precondition rhs)

            {
                m_lhs = lhs;
                m_rhs = rhs;
            }

            protected override bool OnJudge()
            {
                return m_lhs.Judge() ^ m_rhs.Judge();
            }

            private readonly Precondition m_lhs = null;
            private readonly Precondition m_rhs = null;
        };

        // 非
        class PreconditionNot : Precondition
        {
            public PreconditionNot(Precondition con)
            {
                m_con = con;
            }

            protected override bool OnJudge()
            {
                return !m_con.Judge();
            }

            private readonly Precondition m_con = null;
        }
    }
}