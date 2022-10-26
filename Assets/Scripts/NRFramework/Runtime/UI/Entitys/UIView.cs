using System;
using UnityEngine;

namespace NRFramework
{
    public partial class UIView
    {
        protected string viewId;
        protected UIViewBehaviour viewBehaviour;
        public RectTransform rectTransform;
        public GameObject gameObject;
        public RectTransform parentRectTransform;

        protected internal void Create(string viewId, RectTransform parentRectTransform, string prefabPath)
        {
            this.viewId = viewId;
            this.parentRectTransform = parentRectTransform;
            this.viewBehaviour = GetUIViewBehaviour(prefabPath);
            OnInternalCreating();
            OnCreated();
        }
        
        protected internal void Create(string viewId, RectTransform parentRectTransform, UIViewBehaviour viewBehaviour)
        {
            this.viewId = viewId;
            this.parentRectTransform = parentRectTransform;
            this.viewBehaviour = viewBehaviour;
            OnInternalCreating();
            OnCreated();
        }

        private UIViewBehaviour GetUIViewBehaviour(string prefabPath)
        {
            GameObject prefab;
#if UNITY_EDITOR
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
#else
            prefab = null;  //todo，改用资源管理接口加载
#endif

            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            UIViewBehaviour viewBehaviour = go.GetComponent<UIViewBehaviour>(); //这里直接取基类组件是可以的。

            Debug.Assert(viewBehaviour != null, "UIViewBehaviour组件不存在");

            viewBehaviour.transform.SetParent(this.parentRectTransform, false);
            return viewBehaviour;
        }

        protected internal virtual void OnInternalCreating()
        {
            this.rectTransform = viewBehaviour.gameObject.GetComponent<RectTransform>();
            this.gameObject = rectTransform.gameObject;
        }

        #region 打开Widget接口
        public T CreateWidget<T>(string widgetId, RectTransform parentRectTransform, string prefabPath) where T : UIWidget
        {
            UIViewBehaviour parentViewBehaviour = parentRectTransform.GetComponentInParent<UIViewBehaviour>();
            Debug.Assert(viewBehaviour.Equals(parentViewBehaviour), "必须以当前UIView的元素作为UIWidget的根节点");

            T widget = Activator.CreateInstance(typeof(T)) as T;
            widget.Create(widgetId, this, parentRectTransform, prefabPath);
            
            //todo 是否要在View中持有widget待定？
            return widget;
        }

        public T CreateWidget<T>(string widgetId, RectTransform parentRectTransform, UIWidgetBehaviour widgetBehaviour) where T : UIWidget
        {
            UIViewBehaviour parentViewBehaviour = parentRectTransform.GetComponentInParent<UIViewBehaviour>();
            Debug.Assert(viewBehaviour.Equals(parentViewBehaviour), "必须以当前UIView的元素作为UIWidget的根节点");

            T widget = Activator.CreateInstance(typeof(T)) as T;
            widget.Create(widgetId, this, parentRectTransform, widgetBehaviour);
            
            //todo 是否在View中持有widget？
            return widget;
        }
        #endregion

        protected virtual void OnCreated() { }
    }
}
