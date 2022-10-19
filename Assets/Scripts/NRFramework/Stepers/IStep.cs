using System;

namespace NRFramework
{
    public interface IStep
    {
        void Enter();
        void Enter(Action onFinish);
    }
}
