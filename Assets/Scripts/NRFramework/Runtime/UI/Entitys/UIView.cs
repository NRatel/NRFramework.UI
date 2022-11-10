using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public abstract partial class UIView
    {
        protected string viewId;
        public RectTransform parentRectTransform;
        protected UIViewBehaviour viewBehaviour;
        public RectTransform rectTransform;
        public GameObject gameObject;

        public Dictionary<string, UIWidget> widgetDict { private set; get; }

        static public event Action<Button> onButtonClickedGlobalEvent;
        static public event Action<Toggle, bool> onToggleValueChangedGlobalEvent;
        static public event Action<Dropdown, int> onDropdownValueChangedGlobalEvent;

        #region 创建关闭接口
        protected void Create(string viewId, RectTransform parentRectTransform, string prefabPath)
        {
            GameObject prefab;
#if UNITY_EDITOR
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
#else
            prefab = null;  //todo，改用资源管理接口加载
#endif
            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            UIViewBehaviour viewBehaviour = go.GetComponent<UIViewBehaviour>();

            Debug.Assert(viewBehaviour != null, "UIViewBehaviour组件不存在");

            Create(viewId, parentRectTransform, viewBehaviour);
        }

        protected void Create(string viewId, RectTransform parentRectTransform, UIViewBehaviour viewBehaviour)
        {
            this.viewId = viewId;
            this.parentRectTransform = parentRectTransform;
            this.viewBehaviour = viewBehaviour;

            OnInternalCreating();
            OnBindCompsAndEvents();
            OnCreating();

            OnInternalCreated();
            OnCreated();
        }

        protected void Close()
        {
            if (widgetDict != null && widgetDict.Count > 0)
            {
                foreach (KeyValuePair<string, UIWidget> kvPair in widgetDict)
                {
                    kvPair.Value.Close();
                }
            }

            OnClosing();
            OnUnbindCompsAndEvents();
            OnInternalClosing();

            OnInternalClosed();
            OnClosed();
        }
        #endregion

        #region Widget操作相关接口
        public T CreateWidget<T>(string widgetId, RectTransform parentRectTransform, string prefabPath) where T : UIWidget
        {
            UIViewBehaviour parentViewBehaviour = parentRectTransform.GetComponentInParent<UIViewBehaviour>();
            Debug.Assert(viewBehaviour.Equals(parentViewBehaviour), "必须以当前UIView的元素作为UIWidget的根节点");

            T widget = Activator.CreateInstance(typeof(T)) as T;
            widget.Create(widgetId, this, parentRectTransform, prefabPath);

            if (widgetDict == null) { widgetDict = new Dictionary<string, UIWidget>(); }
            widgetDict.Add(widgetId, widget);
            return widget;
        }

        public T CreateWidget<T>(string widgetId, RectTransform parentRectTransform, UIWidgetBehaviour widgetBehaviour) where T : UIWidget
        {
            UIViewBehaviour parentViewBehaviour = parentRectTransform.GetComponentInParent<UIViewBehaviour>();
            Debug.Assert(viewBehaviour.Equals(parentViewBehaviour), "必须以当前UIView的元素作为UIWidget的根节点");

            T widget = Activator.CreateInstance(typeof(T)) as T;
            widget.Create(widgetId, this, parentRectTransform, widgetBehaviour);

            if (widgetDict == null) { widgetDict = new Dictionary<string, UIWidget>(); }
            widgetDict.Add(widgetId, widget);
            return widget;
        }

        public T CreateWidget<T>(RectTransform parentRectTransform, UIWidgetBehaviour widgetBehaviour) where T : UIWidget
        {
            return CreateWidget<T>(typeof(T).Name, parentRectTransform, widgetBehaviour);
        }

        public T CreateWidget<T>(RectTransform parentRectTransform, string prefabPath) where T : UIWidget
        {
            return CreateWidget<T>(typeof(T).Name, parentRectTransform, prefabPath);
        }

        public void CloseWidget(string widgetId)
        {
            Debug.Assert(widgetDict.ContainsKey(widgetId), "widget不存在");

            UIWidget widget = widgetDict[widgetId];
            widgetDict.Remove(widgetId);
            widget.Close();
        }

        public void CloseWidget<T>()
        {
            CloseWidget(typeof(T).Name);
        }

        public UIWidget GetWidget(string widgetId)
        {
            return widgetDict[widgetId];
        }

        public UIWidget GetWidget<T>() where T : UIWidget
        {
            return GetWidget(typeof(T).Name);
        }
        #endregion

        #region 反射获取组件相关接口
        public T FindComponent<T>(string compDefine) where T : Component
        {
            FieldInfo fieldInfo = this.GetType().GetField(compDefine);
            return fieldInfo.GetValue(null) as T;
        }

        public T FindWidgetComponent<T>(string[] widgetIds, string compDefine) where T : Component
        {
            UIView view = this;
            for (int i = 0; i < widgetIds.Length; i++)
            {
                view = view.GetWidget(widgetIds[i]);
            }
            return view.FindComponent<T>(compDefine);
        }

        public T FindWidgetComponent<T>(string widgetPath, string compDefine) where T : Component
        {
            string[] widgetIds = widgetPath.Split("/");
            return FindWidgetComponent<T>(widgetIds, compDefine);
        }
        #endregion

        #region 组件事件绑定
        protected void BindEvent(Button button)
        {
            button.onClick.AddListener(() =>
            {
                onButtonClickedGlobalEvent?.Invoke(button);
                OnClicked(button);
            });
        }

        protected void BindEvent(Toggle toggle)
        {
            toggle.onValueChanged.AddListener((value) =>
            {
                onToggleValueChangedGlobalEvent?.Invoke(toggle, value);
                OnValueChanged(toggle, value);
            });
        }

        protected void BindEvent(Dropdown dropdown)
        {
            dropdown.onValueChanged.AddListener((value) =>
            {
                onDropdownValueChangedGlobalEvent?.Invoke(dropdown, value);
                OnValueChanged(dropdown, value);
            });
        }

        protected void BindEvent(Slider slider)
        {
            slider.onValueChanged.AddListener((value) => { OnValueChanged(slider, value); });
        }

        protected void BindEvent(Scrollbar scrollbar)
        {
            scrollbar.onValueChanged.AddListener((value) => { OnValueChanged(scrollbar, value); });
        }

        protected void BindEvent(ScrollRect scrollRect)
        {
            scrollRect.onValueChanged.AddListener((value) => { OnValueChanged(scrollRect, value); });
        }

        protected void UnbindEvent(Button button)
        {
            button.onClick.RemoveAllListeners();
        }

        protected void UnbindEvent(Toggle toggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }

        protected void UnbindEvent(Dropdown dropdown)
        {
            dropdown.onValueChanged.RemoveAllListeners();
        }

        protected void UnbindEvent(InputField inputField)
        {
            inputField.onValueChanged.RemoveAllListeners();
        }

        protected void UnbindEvent(Slider slider)
        {
            slider.onValueChanged.RemoveAllListeners();
        }

        protected void UnbindEvent(Scrollbar scrollbar)
        {
            scrollbar.onValueChanged.RemoveAllListeners();
        }

        protected void UnbindEvent(ScrollRect scrollRect)
        {
            scrollRect.onValueChanged.RemoveAllListeners();
        }
        #endregion

        #region 内部生命周期
        protected internal virtual void OnInternalCreating()
        {
            this.rectTransform = viewBehaviour.gameObject.GetComponent<RectTransform>();
            this.gameObject = rectTransform.gameObject;

            viewBehaviour.transform.SetParent(this.parentRectTransform, false);

            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
            rectTransform.localScale = Vector3.one;
        }

        protected internal virtual void OnInternalCreated() { }

        protected internal virtual void OnInternalClosing()
        {
            GameObject.Destroy(gameObject);

            gameObject = null;
            rectTransform = null;
            viewBehaviour = null;
            parentRectTransform = null;
            viewId = null;

            widgetDict = null;
        }

        protected internal virtual void OnInternalClosed() { }

        #endregion

        #region 子类生命周期
        /// <summary>
        /// 子类在此完成自身特有创建内容
        /// </summary>
        protected virtual void OnCreating() { }

        /// <summary>
        /// 绑定组件变量和事件（自动生成）
        /// </summary>
        protected virtual void OnBindCompsAndEvents() { }

        /// <summary>
        /// 创建完成
        /// </summary>
        protected virtual void OnCreated() { }

        protected virtual void OnClicked(Button button) { }

        protected virtual void OnValueChanged(Toggle toggle, bool value) { }

        protected virtual void OnValueChanged(Dropdown dropdown, int value) { }

        protected virtual void OnValueChanged(InputField inputField, string value) { }

        protected virtual void OnValueChanged(Slider slider, float value) { }

        protected virtual void OnValueChanged(Scrollbar scrollbar, float value) { }

        protected virtual void OnValueChanged(ScrollRect scrollRect, Vector2 value) { }

        /// <summary>
        /// 子类在此完成自身特有关闭（清理）内容
        /// </summary>
        protected virtual void OnClosing() { }

        /// <summary>
        /// 解除组件变量和事件（自动生成）
        /// </summary>
        protected virtual void OnUnbindCompsAndEvents() { }

        /// <summary>
        /// 关闭完成
        /// </summary>
        protected virtual void OnClosed() { }
        #endregion
    }
}
