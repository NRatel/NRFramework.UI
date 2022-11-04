using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public enum UIPanelShowState { Initing, Refreshing, Idle, Closed }

    public enum UIPanelAnimState { Opening, Closing, Idle, Closed }

    public abstract class UIPanel : UIView
    {
        public string panelId { get { return viewId; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }

        public UIRoot parentUIRoot;
        public Canvas canvas;
        public GraphicRaycaster gaphicRaycaster;

        public UIPanelShowState showState { protected set; get; }

        public UIPanelAnimState animState { protected set; get; }

        internal void Create(string panelId, UIRoot uiRoot, string prefabPath)
        {
            this.parentUIRoot = uiRoot;
            base.Create(panelId, UIManager.Instance.uiCanvas.GetComponent<RectTransform>(), prefabPath);

            if (panelBehaviour.openAnimPlayMode == UIPanelOpenAnimPlayMode.AutoPlay) { PlayOpenAnim(null); }
        }

        internal void Create(string panelId, UIRoot uiRoot, UIPanelBehaviour panelBehaviour)
        {
            this.parentUIRoot = uiRoot;
            base.Create(panelId, UIManager.Instance.uiCanvas.GetComponent<RectTransform>(), panelBehaviour);

            if (panelBehaviour.openAnimPlayMode == UIPanelOpenAnimPlayMode.AutoPlay) { PlayOpenAnim(null); }
        }

        public virtual void Close(Action onFinish = null)
        {
            if (panelBehaviour.openAnimPlayMode == UIPanelOpenAnimPlayMode.AutoPlay)
            {
                PlayCloseAnim(() => { base.Close(); onFinish?.Invoke(); });
            }
            else
            {
                base.Close();
                onFinish?.Invoke();
            }
        }

        internal void SetSortingOrder(int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
        }

        #region 打开关闭动画接口
        protected virtual void PlayOpenAnim(Action onFinish)
        {
            Debug.Assert(animState != UIPanelAnimState.Opening && animState != UIPanelAnimState.Closing);

            animState = UIPanelAnimState.Opening;
            panelBehaviour.PlayOpenAnim(() => { animState = UIPanelAnimState.Idle; onFinish(); });
        }

        protected virtual void PlayCloseAnim(Action onFinish)
        {
            Debug.Assert(animState != UIPanelAnimState.Opening && animState != UIPanelAnimState.Closing);

            animState = UIPanelAnimState.Closing;
            viewBehaviour.PlayOpenAnim(() => { animState = UIPanelAnimState.Closed; onFinish(); });
        }
        #endregion

        protected internal override void OnInternalCreating()
        {
            base.OnInternalCreating();

            canvas = panelBehaviour.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            gaphicRaycaster = panelBehaviour.gameObject.AddComponent<GraphicRaycaster>();
        }

        protected internal override void OnInternalCreated() 
        {
            showState = UIPanelShowState.Idle;
            animState = UIPanelAnimState.Idle;
        }

        protected internal override void OnInternalClosing()
        {
            parentUIRoot.RemovePanelRef(panelId);

            //组件引用解除即可, 实例会随gameObject销毁
            gaphicRaycaster = null;
            canvas = null;
            parentUIRoot = null;

            base.OnInternalClosing();
        }

        protected internal override void OnInternalClosed() 
        {
            showState = UIPanelShowState.Closed;
        }

        #region 子类生命周期
        protected virtual void OnFoucus(bool got) { }

        protected virtual void OnEscButtonClicked() { }

        protected virtual void OnWindowBgClicked() { }

        #endregion
    }
}


