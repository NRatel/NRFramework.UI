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

        protected void DestroySelf()
        {
            parentView.DestroyWidget(widgetId);
        }

        protected internal override void OnInternalDestroying()
        {
            parentView = null;

            base.OnInternalDestroying();
        }
    }
}
