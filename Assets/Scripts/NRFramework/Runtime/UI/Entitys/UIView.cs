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
        public UIContext context;

        public UIView(string viewId)
        {
            this.viewId = viewId;
        }

        protected void Init(RectTransform parentRectTransform, string prefabPath, UIContext context)
        {
            this.parentRectTransform = parentRectTransform;
            this.viewBehaviour = GetUIViewBehaviour(prefabPath);
            this.context = context;
            this.OnCreating();
            this.OnCreated(context);
        }
        
        protected void Init(RectTransform parentRectTransform, UIViewBehaviour viewBehaviour, UIContext context)
        {
            this.parentRectTransform = parentRectTransform;
            this.viewBehaviour = viewBehaviour;
            this.context = context;
            this.OnCreating();
            this.OnCreated(context);
        }

        private UIViewBehaviour GetUIViewBehaviour(string prefabPath)
        {
            GameObject prefab;
#if UNITY_EDITOR
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);  //todo，改用资源管理接口加载
#else
            prefab = null;  //todo
#endif

            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            UIViewBehaviour viewBehaviour = go.GetComponent<UIViewBehaviour>(); //这里直接取基类组件是可以的。

            Debug.Assert(viewBehaviour != null, "UIViewBehaviour组件不存在");

            viewBehaviour.transform.SetParent(this.parentRectTransform, false);
            return viewBehaviour;
        }

        protected virtual void OnCreating()
        {
            this.rectTransform = viewBehaviour.gameObject.GetComponent<RectTransform>();
            this.gameObject = this.rectTransform.gameObject;

            this.viewBehaviour.onEnable = OnEnable;
            this.viewBehaviour.onStart = OnStart;
            this.viewBehaviour.onDisable = OnDisable;
            this.viewBehaviour.onDestroy = OnDestroy;
        }

        #region 打开Widget接口
        public T OpenWidget<T>(string widgetId, RectTransform parentRectTransform, string prefabPath, UIContext context) where T : UIWidget
        {
            UIViewBehaviour parentViewBehaviour = parentRectTransform.GetComponentInParent<UIViewBehaviour>();
            Debug.Assert(viewBehaviour.Equals(parentViewBehaviour), "必须以当前UIView的元素作为UIWidget的根节点");

            T widget = Activator.CreateInstance(typeof(T), widgetId) as T;
            widget.Init(this, parentRectTransform, prefabPath, context);
            
            //todo 是否在View中持有widget？
            return widget;
        }

        public T OpenWidget<T>(string widgetId, RectTransform parentRectTransform, UIWidgetBehaviour widgetBehaviour, UIContext context) where T : UIWidget
        {
            UIViewBehaviour parentViewBehaviour = parentRectTransform.GetComponentInParent<UIViewBehaviour>();
            Debug.Assert(viewBehaviour.Equals(parentViewBehaviour), "必须以当前UIView的元素作为UIWidget的根节点");

            T widget = Activator.CreateInstance(typeof(T), widgetId) as T;
            widget.Init(this, parentRectTransform, widgetBehaviour, context);
            
            //todo 是否在View中持有widget？
            return widget;
        }
        #endregion

        #region UIView生命周期
        protected virtual void OnCreated(UIContext context) { Debug.Log("UIView OnCreated"); }
        #endregion

        #region 来自UIBehaviour的生命周期
        protected virtual void OnEnable() { Debug.Log("UIView OnEnable"); }
        
        protected virtual void OnStart() { Debug.Log("UIView OnStart"); }
        
        protected virtual void OnDisable() { Debug.Log("UIView OnDisable"); }
        
        protected virtual void OnDestroy() { Debug.Log("UIView OnDestroy"); }
        #endregion
    }
}
