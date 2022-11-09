using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "#UIBlocker#")]
    internal class UIBlocker : MonoSingleton<UIBlocker>
    {
        private RectTransform m_RectTrransform;
        private RawImage m_RawImage;
        private Button m_Btn;

        private void Awake()
        {
            m_RectTrransform = (RectTransform)transform;
            m_RawImage = gameObject.AddComponent<RawImage>();
            m_Btn = gameObject.AddComponent<Button>();

            m_RawImage.color = new Color(0, 0, 0, 0);
            m_Btn.transition = Selectable.Transition.None;

            gameObject.SetActive(false);
        }

        public void Bind(RectTransform parent, Color color, Action onClick = null)
        {
            gameObject.SetActive(true);

            m_RectTrransform.SetParent(parent);
            m_RectTrransform.SetAsFirstSibling();

            m_RectTrransform.localPosition = Vector3.zero;
            m_RectTrransform.localRotation = Quaternion.Euler(Vector3.zero);
            m_RectTrransform.localScale = Vector3.one;

            m_RectTrransform.anchorMin = Vector2.zero;
            m_RectTrransform.anchorMax = Vector2.one;
            m_RectTrransform.sizeDelta = Vector2.zero;

            m_RawImage.color = color;

            m_Btn.onClick.RemoveAllListeners();
            if (onClick != null) { m_Btn.onClick.AddListener(() => { onClick(); }); }
        }

        public void Unbind()
        {
            m_Btn.onClick.RemoveAllListeners();
            m_RectTrransform.SetParent(UIBlocker.commonRoot);
            gameObject.SetActive(false);
        }
    }
}