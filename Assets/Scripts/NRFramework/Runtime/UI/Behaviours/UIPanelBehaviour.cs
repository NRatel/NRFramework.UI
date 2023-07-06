// https://github.com/NRatel/NRFramework.UI

using System;
using System.Collections;
using UnityEngine;

namespace NRFramework
{
    public enum UIPanelType { Underlay, Overlay, Window, ModalWindow, Float, System, Custom }

    public enum UIPanelBgShowType { Alpha, HalfAlphaBlack, CustomColor, /* CustomTexture, BlurryScreenshot */ }

    public enum UIPanelBgClickEventType { PassThrough, DontRespone, CloseSelf, DestorySelf, Custom, }

    public enum UIPanelGetFocusType { DontGet, Get, GetWithOthers, }

    public enum UIPanelEscPressEventType { DontCheck, DontRespone, CloseSelf, DestorySelf, Custom, }

    public enum UIPanelOpenAnimPlayMode { AutoPlay, ControlBySelf }

    public enum UIPanelCloseAnimPlayMode { AutoPlay, ControlBySelf }

    public class UIPanelBehaviour : UIViewBehaviour
    {
#pragma warning disable 414
        [SerializeField]
        private UIPanelType m_PanelType;
#pragma warning restore 414

        [SerializeField]
        private bool m_HasBg;

        [SerializeField]
        private UIPanelBgShowType m_BgShowType;

        [SerializeField]
        private Color m_CustomBgColor;

        [SerializeField]
        private UIPanelBgClickEventType m_BgClickEventType;

        [SerializeField]
        private UIPanelGetFocusType m_GetFocusType;

        [SerializeField]
        UIPanelEscPressEventType m_EscPressEventType;

        //层级相关
        [SerializeField]
        private int m_Thickness;

        [SerializeField]
        private UIPanelOpenAnimPlayMode m_OpenAnimPlayMode;

        [SerializeField]
        private UIPanelCloseAnimPlayMode m_CloseAnimPlayMode;

        public bool hasBg { get { return m_HasBg; } }

        static private Color sm_BgColor_Alpha = new Color(0, 0, 0, 0);
        static private Color sm_BgColor_HalfAlphaBlack = new Color(0, 0, 0, 0.7f);

        public Texture bgTexture
        {
            get { return null; }
        }

        public Color bgColor
        {
            get
            {
                Debug.Assert(hasBg);
                return m_BgShowType switch
                {
                    UIPanelBgShowType.Alpha => sm_BgColor_Alpha,
                    UIPanelBgShowType.HalfAlphaBlack => sm_BgColor_HalfAlphaBlack,
                    UIPanelBgShowType.CustomColor => m_CustomBgColor,
                    _ => Color.white,
                };
            }
        }

        public UIPanelBgClickEventType bgClickEventType { get { return m_BgClickEventType; } }

        public UIPanelGetFocusType getFocusType { get { return m_GetFocusType; } }

        public UIPanelEscPressEventType escPressEventType { get { return m_EscPressEventType; } }

        public int thickness { get { return m_Thickness; } }

        public UIPanelOpenAnimPlayMode openAnimPlayMode { get { return m_OpenAnimPlayMode; } }

        public UIPanelCloseAnimPlayMode closeAnimPlayMode { get { return m_CloseAnimPlayMode; } }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            //默认显示为 Underlay
            m_PanelType = UIPanelType.Underlay;

            //默认显示为 Underlay 的子项
            m_HasBg = true;
            m_BgShowType = UIPanelBgShowType.Alpha;
            m_CustomBgColor = Color.white;
            m_BgClickEventType = UIPanelBgClickEventType.DontRespone;
            m_GetFocusType = UIPanelGetFocusType.Get;
            m_EscPressEventType = UIPanelEscPressEventType.DontRespone;

            m_Thickness = Config.kDefaultPanelThickness;
            m_OpenAnimPlayMode = UIPanelOpenAnimPlayMode.AutoPlay;
            m_CloseAnimPlayMode = UIPanelCloseAnimPlayMode.AutoPlay;
        }
#endif

        public bool ExistValidAnimator()
        {
            Animator animator;
            bool exsistAimator = TryGetComponent<Animator>(out animator);

            //todo 可以添加更复杂的检查
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
