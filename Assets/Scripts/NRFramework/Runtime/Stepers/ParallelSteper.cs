using System;
using System.Collections.Generic;

namespace NRFramework
{
    sealed public class ParallelSteper : IMultiStep
    {
        private readonly List<IStep> m_Stepes;
        private readonly int m_Limit = -1;

        private int m_StartedCount;
        private int m_CompletedCount;

        private Action<int> m_OnProgress;
        private Action m_OnComplete;

        public ParallelSteper(List<IStep> stepes, int limit = -1)
        {
            m_Stepes = stepes;
            m_Limit = limit;
        }

        public void Enter()
        {
            Enter(null, null);
        }

        public void Enter(Action onComplete)
        {
            Enter(null, onComplete);
        }

        public void Enter(Action<int> onProgress, Action onComplete)
        {
            m_StartedCount = 0;
            m_CompletedCount = 0;
            m_OnProgress = onProgress;
            m_OnComplete = onComplete;

            if (m_Limit < 0 || m_Stepes.Count < m_Limit)
            {
                //开始全部
                foreach (IStep step in m_Stepes)
                {
                    step.Enter(FinshStep);
                    m_StartedCount += 1;
                }
            }
            else
            {
                //开始限制部分
                for (int i = 0; i < m_Limit; i++)
                {
                    IStep step = m_Stepes[i];
                    step.Enter(FinshStep);
                    m_StartedCount += 1;
                }
            }
        }

        private void FinshStep()
        {
            m_CompletedCount += 1;
            if (m_OnProgress != null)
            {
                m_OnProgress(m_CompletedCount);
            }

            if (m_CompletedCount == m_Stepes.Count)
            {
                m_OnComplete();
            }
            else
            {
                if (m_StartedCount < m_Stepes.Count)
                {
                    //未结束且未全部开始，开始新任务
                    IStep newStep = m_Stepes[m_StartedCount];
                    newStep.Enter(FinshStep);
                    m_StartedCount += 1;
                }
            }
        }
    }
}