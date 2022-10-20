using System;
using System.Collections.Generic;

namespace NRFramework
{
    /// <summary>
    /// 串行步进（单元: IStp）
    /// </summary>
    sealed public class SerialSteps : IMultiStep
    {
        private readonly List<IStep> m_Stepes;
        private Action<int> m_OnProgress;
        private Action m_OnComplete;
        private int m_Progress;

        public SerialSteps(List<IStep> stepes)
        {
            this.m_Stepes = stepes;
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
            m_Progress = 0;
            m_OnProgress = onProgress;
            m_OnComplete = onComplete;
            Excute();
        }

        private void Excute()
        {
            if (m_Progress >= m_Stepes.Count)
            {
                if (m_OnComplete != null) { m_OnComplete(); }
                return;
            }
            m_Stepes[m_Progress].Enter(Next);
        }

        private void Next()
        {
            m_Progress += 1;
            if (m_OnProgress != null) { m_OnProgress(m_Progress); }
            Excute();
        }
    }
}