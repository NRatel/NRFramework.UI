﻿using System;
using System.Collections;
using UnityEngine;

namespace NRFramework
{
    public enum UIPanelType
    {
        /// <summary>
        /// 场景，内容占满全屏甚至超出（如，2D场景）。
        /// 特性：
        /// 1、有背景？：是（固定）（即使内容含空白区域，也可完全阻挡下方交互事件）。
        ///    ①、固定纯色透明；
        ///    ②、点击背景关闭界面？：否（固定）。
        /// 2、可获得焦点？是（固定）。
        /// （“获得焦点”：某时刻，可交互的最上层界面。主要用于打开上层界面或关闭上层界面时，下层界面对自身的处理）
        /// </summary>
        Scene,

        /// <summary>
        /// 覆盖（浮动在上），内容未占满全屏（如：底部切换菜单、主界面菜单（活动图标+状态栏）、非模态功能界面、聊天气泡、toast等）。
        /// 注意，它是一个单独界面，并非其他界面的一部分，否则应该使用UIWidget。
        /// 特性：
        /// 1、有背景？：否（固定）（空白部分不阻挡下方交互事件）。
        /// 2、可获得焦点？：是/否（可选）。
        /// （若为是，逐层向下检查，与其下方“可获得焦点的界面”共同获得焦点）。
        /// </summary>
        Overlap,

        /// <summary>
        /// 窗口（模态），内容未占满全屏（如，部分二级功能界面、确认框等）。
        /// 特性：
        /// 可控制变量：
        /// 1、有背景？：是（固定）（即使内容含空白区域，也可完全阻挡下方交互事件）。
        ///     ①、可设置样式。todo
        ///     ②、点击背景关闭界面？：是/否（可选，但建议项目中一致）。
        /// 2、可获得焦点？是（固定）。
        /// </summary>
        Window,
    }

    public enum UIPanelOpenAnimPlayMode { AutoPlay, ControlBySelf }

    public enum UIPanelCloseAnimPlayMode { AutoPlay, ControlBySelf }

    public class UIPanelBehaviour: UIViewBehaviour
    {
        [SerializeField]
        private UIPanelType m_PanelType;
        [SerializeField]
        private bool m_CanGetFoucus;        //可获得焦点？（仅Overlap界面可选）
        [SerializeField]
        private bool m_ColseWhenClickBg;    //点击背景关闭界面？（建议项目中一致）。（仅Window界面可选）

        //层级相关
        [SerializeField]
        private int m_Thickness;            //厚度

        //适配相关
        [SerializeField]
        private bool m_InSafeArea;          //是否在安全区域内打开

        [SerializeField]
        private UIPanelOpenAnimPlayMode m_OpenAnimPlayMode;   //界面打开动画

        [SerializeField]
        private UIPanelCloseAnimPlayMode m_CloseAnimPlayMode; //界面关闭动画

        public UIPanelType panelType { get { return m_PanelType; } }
        public bool colseWhenClickBg { get { return m_ColseWhenClickBg; } }
        public bool canGetFoucus { get { return m_CanGetFoucus; } }
        public int thickness { get { return m_Thickness; } }
        public bool inSafeArea { get { return m_InSafeArea; } }

        public UIPanelOpenAnimPlayMode openAnimPlayMode { get { return m_OpenAnimPlayMode; } }

        public UIPanelCloseAnimPlayMode closeAnimPlayMode { get { return m_CloseAnimPlayMode; } }

        public bool hasBg { get { return m_PanelType != UIPanelType.Overlap; } }  //是否有背景？（完全由PanelType决定）

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            m_PanelType = UIPanelType.Scene;
            m_CanGetFoucus = false;
            m_ColseWhenClickBg = true;
            m_Thickness = NRFrameworkSetting.kDefaultPanelThickness;
            m_InSafeArea = true;
            m_OpenAnimPlayMode = UIPanelOpenAnimPlayMode.AutoPlay;
            m_CloseAnimPlayMode = UIPanelCloseAnimPlayMode.AutoPlay;
        }
#endif

        public bool ExistValidAnimator()
        {
            Animator animator;
            bool exsistAimator = TryGetComponent<Animator>(out animator);

            //todo
            //Animator组件存在、启用、有Controller资源、且内容符合要求(有open、close动画，有跳转条件...)
            //return isAimatorExsist && animator.enabled && animator.runtimeAnimatorController != null && ...;

            //暂时简单检查
            return exsistAimator && animator.enabled;
        }

        internal void PlayOpenAnim(Action onFinish)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null) { onFinish(); return; }

            animator.SetTrigger("open");

            float length = 0;
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "open") { length = clip.length; break; }
            }

            StartCoroutine(DoAfterSeconds(length, onFinish));
        }

        internal void PlayCloseAnim(Action onFinish)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null) { onFinish(); return; }

            animator.SetTrigger("close");

            float length = 0;
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "close") { length = clip.length; break; }
            }

            StartCoroutine(DoAfterSeconds(length, onFinish));
        }

        private IEnumerator DoAfterSeconds(float duration, Action onFinish)
        {
            yield return new WaitForSeconds(duration);
            onFinish();
        }
    }
}
