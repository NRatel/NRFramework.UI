// https://github.com/NRatel/NRFramework.UI

using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public enum UIPanelShowState { Initing, Refreshing, Idle, Hidden, /* Destroyed */ }

    public enum UIPanelAnimState { Opening, Idle, Closing, Closed }

    public abstract class UIPanel : UIView
    {
        public string panelId { get { return viewId; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }

        public UIRoot parentUIRoot { private set; get; }
        public Canvas canvas { private set; get; }
        public GraphicRaycaster graphicRaycaster { private set; get; }
        public CanvasGroup canvasGroup { private set; get; }

        public UIPanelShowState showState { protected set; get; }

        public UIPanelAnimState animState { protected set; get; }

        internal void Create(string panelId, UIRoot uiRoot, string prefabPath)
        {
            this.parentUIRoot = uiRoot;
            base.Create(panelId, UIManager.Instance.uiCanvas.transform, prefabPath);

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

        internal void SetSiblingIndex(int siblingIndex)
        {
            rectTransform.SetSiblingIndex(siblingIndex);
        }

        internal void SetVisible(bool visible)
        {
            if (showState != UIPanelShowState.Hidden && visible) { return; }
            if (showState == UIPanelShowState.Hidden && !visible) { return; }

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;

            showState = visible ? UIPanelShowState.Idle : UIPanelShowState.Hidden;

            OnVisibleChanged(visible);
        }

        internal void SetBackground()
        {
            switch (panelBehaviour.bgClickEventType)
            {
                case UIPanelBgClickEventType.PassThrough:
                    UIBlocker.Instance.Bind(rectTransform, panelBehaviour.bgTexture, panelBehaviour.bgColor, true, null);
                    break;
                case UIPanelBgClickEventType.DontRespone:
                    UIBlocker.Instance.Bind(rectTransform, panelBehaviour.bgTexture, panelBehaviour.bgColor, false, null);
                    break;
                case UIPanelBgClickEventType.CloseSelf:
                    UIBlocker.Instance.Bind(rectTransform, panelBehaviour.bgTexture, panelBehaviour.bgColor, false, () => 
                    {
                        if (showState != UIPanelShowState.Idle) { return; }
                        CloseSelf(null); 
                    });
                    break;
                case UIPanelBgClickEventType.DestorySelf:
                    UIBlocker.Instance.Bind(rectTransform, panelBehaviour.bgTexture, panelBehaviour.bgColor, false, () => 
                    {
                        if (showState != UIPanelShowState.Idle) { return; }
                        DestroySelf(); 
                    });
                    break;
                case UIPanelBgClickEventType.Custom:
                    UIBlocker.Instance.Bind(rectTransform, panelBehaviour.bgTexture, panelBehaviour.bgColor, false, () =>
                    {
                        if (showState != UIPanelShowState.Idle) { return; }
                        OnBackgroundClicked();
                    });
                    break;
            }
        }

        internal void SetFocus(bool got)
        {
            OnFocusChanged(got);
        }

        internal void DoEscPress()
        {
            Debug.Assert(panelBehaviour.escPressEventType != UIPanelEscPressEventType.DontCheck);

            switch (panelBehaviour.escPressEventType)
            {
                case UIPanelEscPressEventType.DontRespone:
                    //do nothing
                    break;
                case UIPanelEscPressEventType.CloseSelf:
                    CloseSelf(null);
                    break;
                case UIPanelEscPressEventType.DestorySelf:
                    DestroySelf();
                    break;
                case UIPanelEscPressEventType.Custom:
                    OnEscButtonPressed();
                    break;
            }
        }

        #region 操作自身接口

        protected void CloseSelf(Action onFinish = null)
        {
            parentUIRoot.ClosePanel(panelId, onFinish);
        }

        protected void DestroySelf()
        {
            parentUIRoot.DestroyPanel(panelId);
        }

        protected void SetSelfVisible(bool visible)
        {
            parentUIRoot.SetPanelVisible(panelId, visible);
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

            canvas = panelBehaviour.gameObject.GetOrAddComponent<Canvas>();
            graphicRaycaster = gameObject.GetOrAddComponent<GraphicRaycaster>();
            canvas.overrideSorting = true;
        }

        protected internal override void OnInternalCreated()
        {
            showState = UIPanelShowState.Idle;
            animState = UIPanelAnimState.Idle;
        }

        protected internal override void OnInternalDestroying()
        {
            UIBlocker.Instance.Unbind();

            //组件引用解除即可, 实例会随gameObject销毁
            canvasGroup = null;
            graphicRaycaster = null;
            canvas = null;
            parentUIRoot = null;

            base.OnInternalDestroying();
        }

        protected internal override void OnInternalDestroyed()
        {
            //showState = UIPanelShowState.Destroyed;
        }

        #region 子类生命周期
        protected virtual void OnVisibleChanged(bool visible) { }

        protected virtual void OnFocusChanged(bool got) { }

        protected virtual void OnBackgroundClicked() { }

        protected virtual void OnEscButtonPressed() { }
        #endregion
    }
}