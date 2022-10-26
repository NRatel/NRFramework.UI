using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public enum UIPanelState { 
        Created, 
        ShowReady, 
        PlayOpenAnim, 
        Idle
    }

    public class UIPanel : UIView
    {
        public string panelId { get { return viewId; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }
        public Canvas parentCanvas;
        public Canvas canvas;
        public GraphicRaycaster gaphicRaycaster;

        public UIPanelState panelState;

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
        }

        /* 界面初始化应该怎么设计？
        // 界面打开动画应该是 创建后自动开始的，还是初始化时开始的？
        // 

        // 基础原则：希望尽量同步，避免异步。但都要支持。

        // 含两个过程：
        //  1、传入Data 或 从内部获取外部Data（可能异步（现请求））并显示，回调界面显示OK。
        //  2、播放界面打开动画（异步）（也可能无动画），回调动画播放OK。

        // 需要考虑的问题：
        //  1、异步请求数据，应放在Create前还是Init中？
        //      且后者更优。
        //      后者应考虑
        //      在UIPanel中不用考虑前者，
        //  2、异步获取是否要考虑计入子Widget动画？
        //      默认不考虑，有需求自己处理。
        //  3、动画播放是否在显示完成后才能开始？（依赖数据？）
        //      只等同步不等异步，否则需要自行组织。

        // 外部需求：
        //  1、跳转时连续打开多个界面，不关心动画，但依赖数据（数据决定能不能打开目标内容）。
        //  2、红点、引导等上层系统应能随时知晓界面当前状态（尤其是引导，要等界面完全准备好后才能执行）。

        // 可能的情况：
        //  1、无需任何数据、无打开动画。
        //  2、无需任何数据、有打开动画。
        //  3、可同步获得数据并显示、无打开动画。
        //  4、可同步获得数据并显示、有打开动画。
        //  5、异步获得数据并显示、无打开动画。
        //  6、异步获得数据并显示、有打开动画。

        // 用户应自定义初始化方法，并自行在 界面显示完成、界面动画播放完成、最终完成时设置自身状态并回调。
        // 情况1、2，可直接为其提供默认的初始化方法 DefaultInit1 和 DefaultInit2。
        // 情况3，5可在完成自身显示逻辑后调用 DefaultInit1，完成状态设定和回调处理。
        // 情况4，可在完成自身显示逻辑后调用 DefaultInit2，完成状态设定和回调处理。
        // 情况6，需要自行组织（同时调起异步显示逻辑、动画播放逻辑，在各自和最终完成时回调）。
        // ---------------------------------
        // 默认认为 动画播放 在 界面显示逻辑之后进行。但只等同步不等异步，否则需要自行组织。
        // 默认不计入子Widget动画，否则需要自行组织。

        // 待考虑问题：
        // 焦点变化时重播打开/离开动画对状态的影响。
        // 界面状态是否应支持直观观察？比如暴露到 Inspector中。
        */

        public void DefaultInit1(Action<UIPanelState> onInitState) 
        {
            panelState = UIPanelState.ShowReady;
            onInitState(panelState);

            panelState = UIPanelState.PlayOpenAnim;
            onInitState(panelState);

            panelState = UIPanelState.Idle;
            onInitState(panelState);
        }

        public void DefaultInit2(Action<UIPanelState> onInitState)
        {
            panelState = UIPanelState.ShowReady;
            onInitState(panelState);


            panelBehaviour.PlayOpenAnim(()=> {
                panelState = UIPanelState.PlayOpenAnim;
                onInitState(panelState);

                panelState = UIPanelState.Idle;
                onInitState(panelState);
            });
        }

        private void RePlayOpenAnim()
        {
            
        }

        #region 生命周期
        protected override void OnCreated() { }

        protected virtual void OnGotFoucus(bool got) { }

        #endregion
    }
}


