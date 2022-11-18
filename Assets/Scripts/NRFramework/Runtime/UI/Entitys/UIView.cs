// https://github.com/NRatel/NRFramework.UI

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
        public Transform parentTransform;
        protected UIViewBehaviour viewBehaviour;
        public RectTransform rectTransform;
        public GameObject gameObject;

        public Dictionary<string, UIWidget> widgetDict { private set; get; }

        static public event Action<Button> onButtonClickedGlobalEvent;
        static public event Action<Toggle, bool> onToggleValueChangedGlobalEvent;
        static public event Action<Dropdown, int> onDropdownValueChangedGlobalEvent;

        #region 创建关闭接口
        protected void Create(string viewId, Transform parentTransform, string prefabPath)
        {
            GameObject prefab;
#if UNITY_EDITOR
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
#else
            prefab = null;  //todo，改用自封装的资源加载接口
#endif
            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            UIViewBehaviour viewBehaviour = go.GetComponent<UIViewBehaviour>();

            Debug.Assert(viewBehaviour != null, "UIViewBehaviour组件不存在");

            Create(viewId, parentTransform, viewBehaviour);
        }

        protected void Create(string viewId, Transform parentTransform, UIViewBehaviour viewBehaviour)
        {
            this.viewId = viewId;
            this.parentTransform = parentTransform;
            this.viewBehaviour = viewBehaviour;

            OnInternalCreating();
            OnBindCompsAndEvents();
            OnCreating();

            OnInternalCreated();
            OnCreated();
        }

        protected void Destroy()
        {
            if (widgetDict != null && widgetDict.Count > 0)
            {
                foreach (KeyValuePair<string, UIWidget> kvPair in widgetDict)
                {
                    kvPair.Value.Destroy();
                }
            }

            OnDestroying();
            OnUnbindCompsAndEvents();
            OnInternalDestroying();

            OnInternalDestroyed();
            OnDestroyed();
        }
        #endregion

        #region Widget操作相关接口
        public T CreateWidget<T>(string widgetId, RectTransform parentTransform, string prefabPath) where T : UIWidget
        {
            UIViewBehaviour parentViewBehaviour = parentTransform.GetComponentInParent<UIViewBehaviour>();
            Debug.Assert(viewBehaviour.Equals(parentViewBehaviour));    //必须以当前UIView的元素作为UIWidget的根节点

            T widget = Activator.CreateInstance(typeof(T)) as T;
            widget.Create(widgetId, this, parentTransform, prefabPath);

            if (widgetDict == null) { widgetDict = new Dictionary<string, UIWidget>(); }
            widgetDict.Add(widgetId, widget);
            return widget;
        }

        public T CreateWidget<T>(string widgetId, RectTransform parentTransform, UIWidgetBehaviour widgetBehaviour) where T : UIWidget
        {
            UIViewBehaviour parentViewBehaviour = parentTransform.GetComponentInParent<UIViewBehaviour>();
            Debug.Assert(viewBehaviour.Equals(parentViewBehaviour));    //必须以当前UIView的元素作为UIWidget的根节点

            T widget = Activator.CreateInstance(typeof(T)) as T;
            widget.Create(widgetId, this, parentTransform, widgetBehaviour);

            if (widgetDict == null) { widgetDict = new Dictionary<string, UIWidget>(); }
            widgetDict.Add(widgetId, widget);
            return widget;
        }

        public T CreateWidget<T>(RectTransform parentTransform, UIWidgetBehaviour widgetBehaviour) where T : UIWidget
        {
            return CreateWidget<T>(typeof(T).Name, parentTransform, widgetBehaviour);
        }

        public T CreateWidget<T>(RectTransform parentTransform, string prefabPath) where T : UIWidget
        {
            return CreateWidget<T>(typeof(T).Name, parentTransform, prefabPath);
        }

        public void DestroyWidget(string widgetId)
        {
            Debug.Assert(widgetDict != null); //widgetDict未创建
            Debug.Assert(widgetDict.ContainsKey(widgetId)); //widget不存在

            UIWidget widget = widgetDict[widgetId];
            widgetDict.Remove(widgetId);
            widget.Destroy();
        }

        public void DestroyWidget<T>()
        {
            DestroyWidget(typeof(T).Name);
        }

        public UIWidget GetWidget(string widgetId)
        {
            return widgetDict[widgetId];
        }

        public UIWidget GetWidget<T>() where T : UIWidget
        {
            return GetWidget(typeof(T).Name);
        }

        public bool ExistWidget(string widgetId)
        {
            return widgetDict.ContainsKey(widgetId);
        }

        #endregion

        #region 反射获取组件相关接口
        public int FindComponent<T>(string compDefine, out T comp) where T : Component
        {
            comp = null;
            if (string.IsNullOrEmpty(compDefine)) { return FindCompErrorCode.COMP_DEFINE_IS_NULL_OR_EMPTY; }

            FieldInfo fieldInfo = this.GetType().GetField(compDefine, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null) { return FindCompErrorCode.NOT_EXIST_THIS_COMPONENT; }

            T value = fieldInfo.GetValue(this) as T;
            if (value == null) { return FindCompErrorCode.ERROR_CAST_TYPE; }

            comp = value;
            return FindCompErrorCode.OK;
        }

        public int FindWidgetComponent<T>(string[] widgetIds, string compDefine, out T comp) where T : Component
        {
            comp = null;
            if (widgetIds == null || widgetIds.Length <= 0) { return FindCompErrorCode.WIDGETS_ID_IS_NULL_OR_EMPTY; }

            UIView view = this;
            for (int i = 0; i < widgetIds.Length; i++)
            {
                if (view.widgetDict == null) { return FindCompErrorCode.NOT_EXIST_ANY_CHILD_WIDGET; }
                string widgetId = widgetIds[i];
                if (!view.ExistWidget(widgetId)) { return FindCompErrorCode.NOT_EXIST_THIS_CHILD_WIDGET; }
                view = view.GetWidget(widgetId);
            }

            return view.FindComponent<T>(compDefine, out comp);
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

            viewBehaviour.transform.SetParent(this.parentTransform, false);

            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
            rectTransform.localScale = Vector3.one;
        }

        protected internal virtual void OnInternalCreated() { }

        protected internal virtual void OnInternalDestroying()
        {
            GameObject.Destroy(gameObject);

            gameObject = null;
            rectTransform = null;
            viewBehaviour = null;
            parentTransform = null;
            viewId = null;

            widgetDict = null;
        }

        protected internal virtual void OnInternalDestroyed() { }

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
        protected virtual void OnDestroying() { }

        /// <summary>
        /// 解除组件变量和事件（自动生成）
        /// </summary>
        protected virtual void OnUnbindCompsAndEvents() { }

        /// <summary>
        /// 关闭完成
        /// </summary>
        protected virtual void OnDestroyed() { }
        #endregion
    }
}
