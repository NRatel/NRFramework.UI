using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public enum UIPanelShowState { Initing, Idle, Refreshing, Destroyed }

    public enum UIPanelAnimState { Opening, Idle, Closing, Closed }

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

            PlayOpenAnim(null);
        }

        internal void Create(string panelId, UIRoot uiRoot, UIPanelBehaviour panelBehaviour)
        {
            this.parentUIRoot = uiRoot;
            base.Create(panelId, UIManager.Instance.uiCanvas.GetComponent<RectTransform>(), panelBehaviour);

            PlayOpenAnim(null);
        }

        internal void Close(Action onFinish = null)
        {
            PlayCloseAnim(() =>
            {
                base.Destroy();
                onFinish?.Invoke();
            });
        }

        internal new void Destroy()
        { 
            base.Destroy();
        }

        internal void SetSortingOrder(int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
        }

        internal void ChangeFocus(bool got)
        {
            OnInternalFocusChanged(got);
            OnFocusChanged(got);
        }

        #region 关闭自身接口

        protected void CloseSelf(Action onFinish = null)
        {
            parentUIRoot.ClosePanel(panelId, onFinish);
        }

        protected void DestroySelf()
        {
            parentUIRoot.DestroyPanel(panelId);
        }

        #endregion

        #region 打开关闭动画接口
        protected virtual void PlayOpenAnim(Action onFinish = null)
        {
            if (panelBehaviour.ExistValidAnimator() && panelBehaviour.openAnimPlayMode == UIPanelOpenAnimPlayMode.AutoPlay)
            {
                Debug.Assert(animState != UIPanelAnimState.Opening && animState != UIPanelAnimState.Closing);

                animState = UIPanelAnimState.Opening;
                panelBehaviour.PlayOpenAnim(() => { animState = UIPanelAnimState.Idle; onFinish?.Invoke(); });
            }
            else
            {
                animState = UIPanelAnimState.Idle;
                onFinish?.Invoke();
            }
        }

        protected virtual void PlayCloseAnim(Action onFinish = null)
        {
            if (panelBehaviour.ExistValidAnimator() && panelBehaviour.openAnimPlayMode == UIPanelOpenAnimPlayMode.AutoPlay)
            {
                Debug.Assert(animState != UIPanelAnimState.Opening && animState != UIPanelAnimState.Closing);

                animState = UIPanelAnimState.Closing;
                panelBehaviour.PlayOpenAnim(() => { animState = UIPanelAnimState.Closed; onFinish?.Invoke(); });
            }
            else
            {
                animState = UIPanelAnimState.Closed;
                onFinish?.Invoke();
            }
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

        protected internal override void OnInternalDestroying()
        {
            switch (panelBehaviour.panelType)
            {
                case (UIPanelType.Scene):
                    if (UIBlocker.Instance.transform.parent == rectTransform) { UIBlocker.Instance.Unbind(); }
                    break;
                case (UIPanelType.Overlay):
                    break;
                case (UIPanelType.Window):
                    if (UIBlocker.Instance.transform.parent == rectTransform) { UIBlocker.Instance.Unbind(); }
                    break;
            }

            //组件引用解除即可, 实例会随gameObject销毁
            gaphicRaycaster = null;
            canvas = null;
            parentUIRoot = null;

            base.OnInternalDestroying();
        }

        protected internal override void OnInternalDestroyed()
        {
            showState = UIPanelShowState.Destroyed;
        }

        protected internal void OnInternalFocusChanged(bool got)
        {
            switch (panelBehaviour.panelType)
            {
                case (UIPanelType.Scene):
                    if (got) { UIBlocker.Instance.Bind(rectTransform, panelBehaviour.GetBgColor(), null); }
                    else { if (UIBlocker.Instance.transform.parent == rectTransform) { UIBlocker.Instance.Unbind(); } }
                    break;
                case (UIPanelType.Overlay):
                    break;
                case (UIPanelType.Window):
                    if (got) { UIBlocker.Instance.Bind(rectTransform, panelBehaviour.GetBgColor(), OnWindowBgClicked); }
                    else { if (UIBlocker.Instance.transform.parent == rectTransform) { UIBlocker.Instance.Unbind(); } }
                    break;
            }
        }

        #region 子类生命周期
        protected virtual void OnFocusChanged(bool got) { }

        protected virtual void OnWindowBgClicked() { CloseSelf(null); }

        protected virtual void OnEscButtonClicked() { }

        #endregion
    }
}


