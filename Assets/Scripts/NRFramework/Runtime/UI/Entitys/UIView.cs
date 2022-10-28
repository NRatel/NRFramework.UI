using System;
using System.Collections.Generic;
using UnityEngine;

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

        public Dictionary<string, UIWidget> widgetDict;

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

            OnInternalCreating();
            OnCreating();

            OnInternalCreated();
            OnCreated();
        }

        /* 关于 界面显示及状态相关问题 的思考：
        // 基本流程：
        //  1、界面创建后，播放打开动画（若有）。
        //  2、界面初始化时，注入或获取 Data 完成显示。
        //  3、界面刷新时，注入或获取 Data 完成显示。
        //  4、界面关闭时，播放关闭动画（若有）。

        // 注意：
        //  1、动画播放是异步的。动画一般都是创建时挂到预设上的，只操作初始主要节点，不依赖数据。
        //  2、获取Data可能是异步的（现请求）。
        //  3、某些组件的显示可能是异步的（如：为了优化脏标记异步更新）。

        // 外部需求：
        //  1、跳转连续打开多个界面时，不关心动画，但依赖数据（由数据决定是否可以依次打开，直至目标界面）。
        //  2、功能解锁、红点、引导等上层系统需要能随时获取界面当前状态（如引导，要等界面完全准备好后才能执行）。

        // ---------------------------------

        // 其他问题：
        // 1、异步请求数据，应放在Create前还是Init中？
        //      建议后者，后者可以利用自身界面阻挡操作。但注意，必须处理好“创建后~初始化完成前”的显示。

        // 2、界面显示和动画状态如何维护？ 
        //      ⑴、只维护自身状态，不考虑子Widget。
        //      ⑵、但在外部读取时可以考虑计入自身及所有子Widget的状态（结合实际需求）。
        //      ⑶、初始化/刷新方法 完全由用户自定义，可以根据实际需求进行标记。
        //      ⑷、在 OnInternalCreated 中调起打开动画
        //      ⑸、在 Close 中调起关闭动画。
        //      
        // 3、是否将界面状态暴露到 Inspector中，便于调试？
        //      不确定有没必要，待定。

        /// <summary>
        /// 同步初始化/刷新示例
        /// </summary>
        public void InitOrRefresh_Sync(object data)
        {
            ShowWithData(data);
            panelShowState = UIPanelShowState.Idle;
        }

        /// <summary>
        /// 异步初始化/刷新示例
        /// </summary>
        public void InitOrRefresh_Async(object data1, Action onInited)
        {
            GetData2((data2) =>      //异步获取数据
            {
                ShowWithDatas(data1, data2, () =>     //异步显示
                {
                    panelShowState = UIPanelShowState.Idle;
                    onInited();
                });
            });
        }
        */

        /// <summary>
        /// 关闭。
        /// 注意：
        /// 1、关闭需要先等待关闭所有子Widget后再关闭自身。
        /// 2、子类应重写 OnClosing 完成自身的清理工作。
        /// 3、清理过程是先清子类后清父类；关闭回调是先调父类后调子类。
        /// 4、关闭方法回调 onClosed 晚于生命周期 OnClosed 被调用。
        /// </summary>
        /// <param name="onClosed">关闭回调（晚于生命周期OnClosed）</param>
        public virtual void Close(Action onClosed = null)
        {
            if (widgetDict.Count <= 0)
            {
                CloseSelf(onClosed);
            }
            else
            {
                int wPocess = 0;
                foreach (UIWidget widget in widgetDict.Values)
                {
                    widget.Close(() =>
                    {
                        wPocess += 1;
                        if (wPocess == widgetDict.Count)
                        {
                            CloseSelf(onClosed);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 直接关闭（不播关闭动画）
        /// 有时候，即使有动画，也想直接关掉。通常在跳转时。
        /// </summary>
        public void CloseDirectly()
        {
            if (widgetDict.Count > 0)
            {
                foreach (UIWidget widget in widgetDict.Values)
                {
                    widget.CloseDirectly();
                }
            }
            CloseSelf(null);
        }

        private void CloseSelf(Action onClosed = null)
        {
            PlayCloseAnim(() =>
            {
                OnClosing();
                OnInternalClosing();

                OnInternalClosed();
                OnClosed();
                if (onClosed != null) { onClosed(); }
            });
        }

        protected internal virtual void OnInternalCreating()
        {
            this.rectTransform = viewBehaviour.gameObject.GetComponent<RectTransform>();
            this.gameObject = rectTransform.gameObject;

            //widgetDict = new Dictionary<string, UIWidget>(); //改为需要时创建
        }

        protected virtual void OnCreating() { }

        protected internal virtual void OnInternalCreated()
        {
            showState = UIShowState.Crated;
            animState = UIAnimState.Crated;

            PlayOpenAnim();
        }

        protected virtual void OnCreated() { }

        protected internal virtual void OnInternalClosing()
        {
            GameObject.Destroy(gameObject);
            gameObject = null;
            rectTransform = null;
            viewBehaviour = null;
            parentRectTransform = null;
            viewId = null;
        }

        protected virtual void OnClosing() { }

        protected internal virtual void OnInternalClosed()
        {
            animState = UIAnimState.Closed;
            showState = UIShowState.Closed;
        }

        protected virtual void OnClosed() { }

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

        internal void RemoveWidgetRef(string widgetId)
        {
            widgetDict.Remove(widgetId);
        }
        #endregion

        private void PlayOpenAnim()
        {
            Debug.Assert(animState == UIAnimState.Idle);

            animState = UIAnimState.Opening;
            viewBehaviour.PlayOpenAnim(() => { animState = UIAnimState.Idle; });
        }

        private void PlayCloseAnim(Action onFinish)
        {
            Debug.Assert(animState == UIAnimState.Idle);

            animState = UIAnimState.Closing;
            viewBehaviour.PlayOpenAnim(() => { animState = UIAnimState.Idle; onFinish(); });
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
    }
}
