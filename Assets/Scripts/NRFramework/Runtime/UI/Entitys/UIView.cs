using System;
using UnityEngine;

namespace NRFramework
{
    public abstract class UIView
    {
        protected string viewId;
        public RectTransform parentRectTransform;
        protected UIViewBehaviour viewBehaviour;
        public RectTransform rectTransform;
        public GameObject gameObject;

        protected internal void Create(string viewId, RectTransform parentRectTransform, string prefabPath)
        {
            UIViewBehaviour viewBehaviour = GetUIViewBehaviour(prefabPath);
            Create(viewId, parentRectTransform, viewBehaviour);
        }

        protected internal void Create(string viewId, RectTransform parentRectTransform, UIViewBehaviour viewBehaviour)
        {
            this.viewId = viewId;
            this.parentRectTransform = parentRectTransform;
            this.viewBehaviour = viewBehaviour;
            this.rectTransform = viewBehaviour.gameObject.GetComponent<RectTransform>();
            this.gameObject = rectTransform.gameObject;

            OnInternalCreating();
            OnCreating();

            OnInternalCreated();
            OnCreated();
        }

        /// <summary>
        /// 关闭。（注意：创建是先父后子，关闭（清理）是先子后父）
        /// </summary>
        public virtual void Close(Action onClosed)
        {
            OnInternalClosing();
            OnClosing();

            GameObject.Destroy(gameObject);
            gameObject = null;
            rectTransform = null;
            viewBehaviour = null;
            parentRectTransform = null;
            viewId = null;

            OnInternalClosed();
            OnClosed();
            onClosed(); //注意：方法回调晚于生命周期
        }

        protected internal virtual void OnInternalCreating() { }    //仅供UIPanel和UIWidget内部使用

        protected virtual void OnCreating() { }

        protected internal virtual void OnInternalCreated() { }     //仅供UIPanel和UIWidget内部使用    

        protected virtual void OnCreated() { }

        protected internal virtual void OnInternalClosing() { }     //仅供UIPanel和UIWidget内部使用

        protected virtual void OnClosing() { }

        protected internal virtual void OnInternalClosed() { }      //仅供UIPanel和UIWidget内部使用

        protected virtual void OnClosed() { }

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
    }
}
