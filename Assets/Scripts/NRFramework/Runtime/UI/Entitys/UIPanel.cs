using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public enum UIPanelShowState { Crated, Initing, Refreshing, Idle }
    public enum UIPanelAnimState { Crated, Opening, Closing, Idle }

    public class UIPanel : UIView
    {
        public string panelId { get { return viewId; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }
        public Canvas parentCanvas;
        public Canvas canvas;
        public GraphicRaycaster gaphicRaycaster;

        public UIPanelShowState panelShowState { protected set; get; }
        public UIPanelAnimState panelAnimState { protected set; get; }

        protected internal void Create(string panelId, Canvas parentCanvas, string prefabPath)
        {
            this.parentCanvas = parentCanvas;
            base.Create(panelId, parentCanvas.GetComponent<RectTransform>(), prefabPath);
        }

        protected internal void Create(string panelId, Canvas parentCanvas, UIPanelBehaviour panelBehaviour)
        {
            this.parentCanvas = parentCanvas;
            base.Create(panelId, parentCanvas.GetComponent<RectTransform>(), panelBehaviour);
        }

        internal void SetSortingOrder(int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
        }

        protected internal override void OnInternalCreating()
        {
            base.OnInternalCreating();
            canvas = panelBehaviour.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            gaphicRaycaster = panelBehaviour.gameObject.AddComponent<GraphicRaycaster>();

            panelShowState = UIPanelShowState.Crated;
            panelAnimState = UIPanelAnimState.Crated;

            PlayOpenAnim();
        }

        protected internal override void OnInternalCreated()
        {
            base.OnInternalCreated();
            panelShowState = UIPanelShowState.Crated;
            panelAnimState = UIPanelAnimState.Crated;

            PlayOpenAnim();
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
        // 2、界面显示状态是否需要考虑子Widget？ 
        //      初始化/刷新方法 完全由用户自定义，可以根据实际显示逻辑考虑。注意正确标记状态。
        // 3、是否将界面状态暴露到 Inspector中，便于调试？
        //      不确定有没必要，待定。

        /// <summary>
        /// 同步初始化示例（刷新同理）
        /// </summary>
        public void Init_Sync(object data)
        {
            ShowWithData(data);
            panelShowState = UIPanelShowState.Idle;
        }

        /// <summary>
        /// 异步初始化示例（刷新同理）
        /// </summary>
        public void Init_Async(object data1, Action onInited)
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

        public void Close()
        {
            
        }

        public void PlayOpenAnim()
        {
            Debug.Assert(panelAnimState == UIPanelAnimState.Idle);

            panelAnimState = UIPanelAnimState.Opening;
            panelBehaviour.PlayOpenAnim(() => { panelAnimState = UIPanelAnimState.Idle; });
        }

        public virtual void PlayCloseAnim()
        {
            Debug.Assert(panelAnimState == UIPanelAnimState.Idle);

            panelAnimState = UIPanelAnimState.Closing;
            panelBehaviour.PlayOpenAnim(() => { panelAnimState = UIPanelAnimState.Idle; });
        }

        #region 生命周期
        protected override void OnCreated() { }

        protected virtual void OnGotFoucus(bool got) { }

        #endregion
    }
}


