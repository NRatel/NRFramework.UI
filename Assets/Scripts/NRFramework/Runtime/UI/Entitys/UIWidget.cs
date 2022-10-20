using UnityEngine;

namespace NRFramework
{
    public class UIWidget : UIView
    {
        protected UIView parentView;
        protected string widgetId { get { return viewId; } }
        public UIWidgetBehaviour widgetBehaviour { get { return (UIWidgetBehaviour)viewBehaviour; } }

        public UIWidget(string widgetId) : base(widgetId) { }

        public void Init(UIView parentView, RectTransform parentRectTransform, string prefabPath, UIContext context)
        {
            this.parentView = parentView;
            base.Init(parentRectTransform, prefabPath, context);
        }

        public void Init(UIView parentView, RectTransform parentRectTransform, UIWidgetBehaviour widgetBehaviour, UIContext context)
        {
            this.parentView = parentView;
            base.Init(parentRectTransform, widgetBehaviour, context);
        }
    }
}
