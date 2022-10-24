using UnityEngine;

namespace NRFramework
{
    public class UIWidget : UIView
    {
        protected UIView parentView;
        protected string widgetId { get { return viewId; } }
        public UIWidgetBehaviour widgetBehaviour { get { return (UIWidgetBehaviour)viewBehaviour; } }

        public void Init(string widgetId, UIView parentView, RectTransform parentRectTransform, string prefabPath, UIContext context)
        {
            this.parentView = parentView;
            base.Init(widgetId, parentRectTransform, prefabPath, context);
        }

        public void Init(string widgetId, UIView parentView, RectTransform parentRectTransform, UIWidgetBehaviour widgetBehaviour, UIContext context)
        {
            this.parentView = parentView;
            base.Init(widgetId, parentRectTransform, widgetBehaviour, context);
        }
    }
}
