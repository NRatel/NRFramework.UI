using UnityEngine;

namespace NRFramework
{
    public abstract class UIWidget : UIView
    {
        protected UIView parentView;
        protected string widgetId { get { return viewId; } }
        public UIWidgetBehaviour widgetBehaviour { get { return (UIWidgetBehaviour)viewBehaviour; } }

        protected internal void Create(string widgetId, UIView parentView, RectTransform parentRectTransform, string prefabPath)
        {
            this.parentView = parentView;
            base.Create(widgetId, parentRectTransform, prefabPath);
        }

        protected internal void Create(string widgetId, UIView parentView, RectTransform parentRectTransform, UIWidgetBehaviour widgetBehaviour)
        {
            this.parentView = parentView;
            base.Create(widgetId, parentRectTransform, widgetBehaviour);
        }

        #region 子类生命周期
        /// <summary>
        /// 子类在此补充创建内容
        /// </summary>
        protected override void OnCreating() { }

        /// <summary>
        /// 创建完成
        /// </summary>
        protected override void OnCreated() { }

        /// <summary>
        /// 子类在此补充关闭（清理）内容
        /// </summary>
        protected override void OnClosing() { }

        /// <summary>
        /// 关闭完成
        /// </summary>
        protected override void OnClosed() { }
        #endregion
    }
}
