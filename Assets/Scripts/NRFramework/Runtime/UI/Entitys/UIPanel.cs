using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public abstract class UIPanel : UIView
    {
        public string panelId { get { return viewId; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }
        public Canvas parentCanvas;
        public Canvas canvas;
        public GraphicRaycaster gaphicRaycaster;

        internal void Create(string panelId, Canvas parentCanvas, string prefabPath)
        {
            this.parentCanvas = parentCanvas;
            base.Create(panelId, parentCanvas.GetComponent<RectTransform>(), prefabPath);
        }

        internal void Create(string panelId, Canvas parentCanvas, UIPanelBehaviour panelBehaviour)
        {
            this.parentCanvas = parentCanvas;
            base.Create(panelId, parentCanvas.GetComponent<RectTransform>(), panelBehaviour);
        }

        internal void SetSortingOrder(int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
        }

        protected internal override void OnInternalCreating()
        {
            base.OnInternalCreating();

            canvas = panelBehaviour.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            gaphicRaycaster = panelBehaviour.gameObject.AddComponent<GraphicRaycaster>();
        }

        protected internal override void OnInternalClosing()
        {
            UIManager.Instance.RemovePanelRef(panelId);

            //组件引用解除即可, 实例会随gameObject销毁
            gaphicRaycaster = null;
            canvas = null;
            parentCanvas = null;

            base.OnInternalClosing();
        }

        #region 子类生命周期

        protected virtual void OnFoucus(bool got) { }

        protected virtual void OnEscButtonClicked() { }

        protected virtual void OnWindowBgClicked() { }

        #endregion
    }
}


