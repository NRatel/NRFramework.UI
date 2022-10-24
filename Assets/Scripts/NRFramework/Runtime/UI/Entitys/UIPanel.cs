using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public class UIPanel : UIView
    {
        public string panelId { get { return viewId; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }
        public Canvas parentCanvas;
        public Canvas canvas;
        public GraphicRaycaster gaphicRaycaster;

        public void Init(string panelId, Canvas parentCanvas, string prefabPath, UIContext context)
        {
            this.parentCanvas = parentCanvas;
            base.Init(panelId, parentCanvas.GetComponent<RectTransform>(), prefabPath, context);
        }

        public void SetSortingOrder(int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
        }

        protected override void OnCreating()
        {
            base.OnCreating();
            canvas = panelBehaviour.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            gaphicRaycaster = panelBehaviour.gameObject.AddComponent<GraphicRaycaster>();
        }

        #region UIPanel生命周期
        protected virtual void OnGotFoucus() { }

        protected virtual void OnLostFoucus() { }
        #endregion
    }
}


