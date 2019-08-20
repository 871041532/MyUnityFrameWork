using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

    //Job基类
    public class Job
    {
        protected Action<Job> m_SuccessCall;//成功回调
        protected Action<Job> m_ErrorCall;//失败回调
        protected Action<Job> m_ProgressChangeCall;//进行中回调
        
        protected int m_Id = -1;
        public float Id { get { return m_Id; } }
        protected float m_Progress = 0;
        public float Progress { get { return m_Progress; } }
        protected bool m_IsRunning = false;
        public bool IsRunning { get { return m_IsRunning; } }
        // 替换OnRun的策略方法，用于组合模式减少代码量
        protected Action<Job> m_OnRun;

        public Job(Action<Job> runProcess = null)
        {
            m_OnRun = runProcess;
        }

        #region 外部接口
        //开跑
        public void Run(Action<Job> successCall = null, Action<Job> errorCall = null, Action<Job> processChangeCall = null, int _runId = 0)
        {
            if (m_IsRunning)
            {
                Debug.LogError("Job is on running, do not repeat.");
                return;
            }
            m_SuccessCall = successCall;
            m_ErrorCall = errorCall;
            m_ProgressChangeCall = processChangeCall;
            m_Id = _runId;
            m_IsRunning = true;
            ProgressChange(0);
            if (m_OnRun is null)
            {
                this.OnRun();
            }
            else
            {
                m_OnRun(this);
            }
        }

        public void Success()
        {
            if (!m_IsRunning)
            {
                return;
            }
            ProgressChange(1);
            m_SuccessCall?.Invoke(this);
            Reset();
        }
        
        public void Fail()
        {
            if (!m_IsRunning)
            {
                return;
            }
            m_ErrorCall?.Invoke(this);
            Reset();
        }
        
        protected void ProgressChange(float progress)
        {
            if (Math.Abs(progress - m_Progress) >= 0.001)
            {
                m_Progress = progress;
                m_ProgressChangeCall?.Invoke(this);
            }
        }
        #endregion
        

        public void Reset()
        {
            if (!m_IsRunning)
            {
                return;
            }
            m_IsRunning = false;
            m_SuccessCall = null;
            m_ProgressChangeCall = null;
            m_ErrorCall = null;
            OnReset();
        }

        #region 模板方法
        protected virtual void OnRun()
        {

        }
        protected virtual void OnReset()
        {
            
        }
        #endregion
    }

    public class SequenceJob : Job
    {
        private List<Job> m_Children = new List<Job>();
        private int m_CurIdx = 0;
        private readonly Action<Job> m_ChildSuccessHandler;
        private readonly Action<Job> m_ChildFailHandler;
        private readonly Action<Job> m_ChildProgressChangeHandler;

        public SequenceJob()
        {
            m_ChildSuccessHandler = ChildSuccessHandler;
            m_ChildFailHandler = ChildFailHandler;
            m_ChildProgressChangeHandler = ChildProgressChangeHandler;
        }
        
        public void AddChild(Job job)
        {
            if (m_IsRunning)
            {
                Debug.LogError("ParallelJob 已经启动，无法添加Child.");
            }
            else
            {
                m_Children.Add(job);
            }
        }
        
        private void ChildSuccessHandler(Job child)
        {
            m_CurIdx += 1;
            nextJob();
        }

        private void ChildFailHandler(Job child)
        {
            Fail();
        }

        private void ChildProgressChangeHandler(Job child)
        {
            float progress = (m_CurIdx + child.Progress >= 0? child.Progress:0) / m_Children.Count;
            ProgressChange(progress);
        }

        private void nextJob()
        {
            if (!m_IsRunning)
            {
                return;
            }
            if (m_Children.Count > m_CurIdx)
            {
                m_Children[m_CurIdx].Run(m_ChildSuccessHandler, m_ChildFailHandler, m_ChildProgressChangeHandler, m_CurIdx);
            }
            else
            {
                Success();
            }
        }
        
        protected override void OnRun()
        {
            m_CurIdx = 0;
            nextJob();
        }
        
        protected override void OnReset()
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].Reset();
            }
            m_CurIdx = 0;
        }
    }

    public class ParallelJob : Job
    {
        public List<Job> m_Children = new List<Job>();
        public int m_SuccessCount = 0;
        public int m_ErrorCount = 0;
        public List<Job> m_ErrorChildren = new List<Job>();
        private readonly Action<Job> m_ChildSuccessHandler;
        private readonly Action<Job> m_ChildFailHandler;
        private readonly Action<Job> m_ChildProgressChangeHandler;

        public ParallelJob()
        {
            m_ChildSuccessHandler = ChildSuccessHandler;
            m_ChildFailHandler = ChildFailHandler;
            m_ChildProgressChangeHandler = ChildProgressChangeHandler;
        }
        
        public void AddChild(Job job)
        {
            if (m_IsRunning)
            {
                Debug.LogError("ParallelJob 已经启动，无法添加Child.");
            }
            else
            {
                m_Children.Add(job);
            }
        }
        
        protected override void OnRun()
        {
            if (m_Children.Count <= 0)
            {
                TryEnd();
            }
            else
            {
                for (int i = 0; i < m_Children.Count; i++)
                {
                    m_Children[i].Run(m_ChildSuccessHandler, m_ChildFailHandler, m_ChildProgressChangeHandler, i);
                }
            }
        }
        
        protected override void OnReset()
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].Reset();
            }
            m_SuccessCount = 0;
            m_ErrorCount = 0;
            m_ErrorChildren.Clear();
        }
        
        private void TryEnd()
        {
            // 等待所有子Job执行完再出结果
            if (m_SuccessCount + m_ErrorCount == m_Children.Count)
            {
                if (m_ErrorCount > 0)
                {
                    Fail();
                }
                else
                {
                    Success();
                }
            }
        }
        
        private void ChildSuccessHandler(Job child)
        {
            if (!m_IsRunning)
            {
                return;
            }
            m_SuccessCount += 1;
            TryEnd();
        }
        
        private void ChildFailHandler(Job child)
        {
            if (!m_IsRunning)
            {
                return;
            }
            m_ErrorCount += 1;
            m_ErrorChildren.Add(child);
            TryEnd();
        }
        
        private void ChildProgressChangeHandler(Job child)
        {
            if (!m_IsRunning)
            {
                return;
            }
            float childrenProgress = 0;
            for (int i = 0; i < m_Children.Count; i++)
            {
                childrenProgress += m_Children[i].Progress >= 0? m_Children[i].Progress:0;
            }
            childrenProgress /= m_Children.Count;
            ProgressChange(childrenProgress);
        }
    }