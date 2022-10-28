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
        /// <summary>
        /// 执行创建（子类在此补充创建内容）
        /// </summary>
        protected override void OnCreating() { }

        /// <summary>
        /// 创建完成（状态已置为“已创建”）（子类可在此做上层逻辑，如：处理外部回调等）
        /// </summary>
        protected override void OnCreated() { }

        protected virtual void OnFoucus(bool got) { }

        protected virtual void OnClickBackBtn() { }

        protected virtual void OnClickWindowBg() { }

        /// <summary>
        /// 执行关闭（子类在此补充关闭（清理）内容）
        /// </summary>
        protected override void OnClosing() { }

        /// <summary>
        /// 关闭完成（状态已置为“已关闭”）（子类可在此做上层逻辑，如：处理外部回调等）
        /// </summary>
        protected override void OnClosed() { }

        #endregion
    }
}


