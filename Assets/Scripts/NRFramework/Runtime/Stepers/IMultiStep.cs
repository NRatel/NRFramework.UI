using System;

namespace NRFramework
{
    public interface IMultiStep : IStep
    {
        void Enter(Action<int> onProgress, Action onFinish); //onProgres 范围（1~总步数）
    }
}