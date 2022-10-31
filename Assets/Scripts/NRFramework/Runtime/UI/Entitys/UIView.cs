using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public enum UIShowState { Crated, Initing, Refreshing, Idle, Closed }
    public enum UIAnimState { Crated, Opening, Closing, Idle, Closed }

    public abstract class UIView
    {
        protected string viewId;
        public RectTransform parentRectTransform;
        protected UIViewBehaviour viewBehaviour;
        public RectTransform rectTransform;
        public GameObject gameObject;

        public UIShowState showState { protected set; get; }
        public UIAnimState animState { protected set; get; }

        public Dictionary<string, UIWidget> widgetDict { private set; get; }

        static public event Action<Button> onButtonClickedGlobalEvent;
        static public event Action<Toggle, bool> onToggleValueChangedGlobalEvent;
        static public event Action<Dropdown, int> onDropdownValueChangedGlobalEvent;

        #region 创建关闭接口
        protected internal void Create(string viewId, RectTransform parentRectTransform, string prefabPath)
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

            viewBehaviour.transform.SetParent(this.parentRectTransform, false);

            Create(viewId, parentRectTransform, viewBehaviour);
        }

        protected internal void Create(string viewId, RectTransform parentRectTransform, UIViewBehaviour viewBehaviour)
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

        public virtual void Close()
        {
            if (widgetDict.Count > 0)
            {
                foreach (UIWidget widget in widgetDict.Values)
                {
                    widget.Close();
                }
            }

            OnClosing();
            OnUnbindCompsAndEvents();
            OnInternalClosing();

            OnInternalClosed();
            OnClosed();
        }
        #endregion

        #region 打开关闭动画接口
        public virtual void PlayOpenAnim(Action onFinish)
        {
            Debug.Assert(animState != UIAnimState.Opening && animState != UIAnimState.Closing);

            animState = UIAnimState.Opening;
            viewBehaviour.PlayOpenAnim(() => { animState = UIAnimState.Idle; onFinish(); });
        }

        public virtual void PlayCloseAnim(Action onFinish)
        {
            Debug.Assert(animState != UIAnimState.Opening && animState != UIAnimState.Closing);

            animState = UIAnimState.Closing;
            viewBehaviour.PlayOpenAnim(() => { animState = UIAnimState.Closed; onFinish(); });
        }

        #endregion

        #region Widget相关接口
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

        public UIWidget GetWidget(string widgetId)
        {
            return widgetDict[widgetId];
        }

        internal void RemoveWidgetRef(string widgetId)
        {
            widgetDict.Remove(widgetId);
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

        protected void BindEvent(InputField inputField)
        {
            inputField.onValueChanged.AddListener((value) => { OnValueChanged(inputField, value); });
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
        }

        protected internal virtual void OnInternalCreated()
        {
            showState = UIShowState.Crated;
            animState = UIAnimState.Crated;
        }

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

        protected internal virtual void OnInternalClosed()
        {
            showState = UIShowState.Closed;
        }

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
