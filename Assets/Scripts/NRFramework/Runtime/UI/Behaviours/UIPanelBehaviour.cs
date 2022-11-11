using System;
using System.Collections;
using UnityEngine;

namespace NRFramework
{
    public enum UIPanelType
    {
        /// <summary>
        /// 场景（铺衬在底）。内容通常占满全屏甚至超出。
        /// 1、背景：自带透明背景，完全阻挡下方交互事件（即使内容含空白区域），点击背景无任何响应。
        /// 2、焦点：可独立获得焦点。
        /// </summary>
        Scene,

        /// <summary>
        /// 叠加。内容通常未占屏局部。非模态的，可同时与下层界面交互。
        /// 1、背景：无背景，空白部分无法阻挡下方交互事件。
        /// 2、焦点：可选获得焦点（若可获得，与其下方“可获得焦点的界面”共同获得焦点）。
        /// 如：主界面切换菜单（可获得焦点）、主界面浮动功能气泡（不可获得焦点）、toast（不可获得焦点）。
        /// </summary>
        Overlay,

        /// <summary>
        /// 窗体。内容通常未占屏局部。模态地，要求用户必须首先对该界面进行响应。
        /// 1、背景：自带黑色半透背景，完全阻挡下方交互事件（即使内容含空白区域），点击背景默认关闭自身。
        /// 2、焦点：可独立获得焦点。
        /// 如，部分二级功能界面、确认框等。
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
        private bool m_CanGetFocus;        //可获得焦点？（仅Overlay界面可选，默认true）

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
        public bool canGetFocus { get { return m_CanGetFocus; } }
        public int thickness { get { return m_Thickness; } }
        public bool inSafeArea { get { return m_InSafeArea; } }

        public UIPanelOpenAnimPlayMode openAnimPlayMode { get { return m_OpenAnimPlayMode; } }

        public UIPanelCloseAnimPlayMode closeAnimPlayMode { get { return m_CloseAnimPlayMode; } }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            m_PanelType = UIPanelType.Scene;
            m_CanGetFocus = true;
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

        static private Color sm_AlphaBgColor = new Color(0, 0, 0, 0);
        static private Color sm_BlackBgColor = new Color(0, 0, 0, 0.5f);
        internal Color GetBgColor()
        {
            switch (panelType)
            {
                case UIPanelType.Scene:
                    return sm_AlphaBgColor;
                case UIPanelType.Window:
                    return sm_BlackBgColor;
                default:
                    throw new Exception();
            }
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
