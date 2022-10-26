using System;
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

        // 界面初始化应该怎么设计？

        // 基础原则：希望尽量同步，避免异步。但都要支持。

        // 含两个过程：
        //  1、传入Data 或 从内部获取外部Data（可能异步（现请求））并显示，回调界面显示OK。
        //  2、播放界面打开动画（异步）（也可能无动画），回调动画播放OK。

        // 需要考虑的问题：
        //  1、异步请求数据，应放在Init中，还是Create前？
        //  2、异步获取是否要考虑计入子Widget动画？
        //  3、动画播放是否在显示完成后才能开始？（依赖数据？）

        // 外部需求：
        //  1、跳转时连续打开多个界面，不关心动画，但依赖数据（数据决定能不能打开目标内容）。
        //  2、红点、引导等上层系统应能随时知晓界面当前状态（尤其是引导，要等界面完全准备好后才能执行）。

        // 可能的情况：
        //  1、无需任何数据、无打开动画。
        //  2、无需任何数据、有打开动画。
        //  3、可同步获得数据、无打开动画。
        //  4、可同步获得数据、有打开动画。
        //  5、异步获得数据、无打开动画。
        //  6、异步获得数据、有打开动画。（最终准备好的时长，取最大值）

        // 用户应自定义初始化方法，并自行在 界面显示完成、界面动画播放完成、界面完全准备好时设置自身状态并回调。
        // 可直接为情况1、2，提供默认的初始化方法。

        public virtual void Init(Action onReady = null)
        {
            panelBehaviour.PlayOpenAnim(onReady);
        }

        #region 生命周期
        protected override void OnCreated() { }

        protected virtual void OnGotFoucus(bool got) { }
        #endregion
    }
}


