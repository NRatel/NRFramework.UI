using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    [System.Serializable]
    public class UIOpElement
    {
        [SerializeField]
        private GameObject m_Target;                //目标物体，可能为null（未设置 或 引用的丢失但未刷新）。

        [SerializeField]
        private List<Component> m_ComponentList;    //组件列表，可能为null（引用的丢失但未刷新）。

        public GameObject target { set { m_Target = value; } get { return m_Target; } }

        public List<Component> componentList { get { return m_ComponentList; } }

        public UIOpElement()
        {
            m_Target = null;
            m_ComponentList = new List<Component>();
        }

        public Component GetComponentByIndex(int index)
        {
            return m_ComponentList[index];
        }
    }

    [DisallowMultipleComponent]
    public abstract class UIViewBehaviour : MonoBehaviour
    {
        [SerializeField]
        protected List<UIOpElement> m_OpElementList;

        public event Action onEnable;
        public event Action onStart;
        public event Action onDisable;
        public event Action onDestroy;

        public List<UIOpElement> opElementList { get { return m_OpElementList; } }

        public bool HasSavedGameObject(GameObject go)
        {
            for (int i = 0; i < opElementList.Count; i++)
            {
                UIOpElement opElement = opElementList[i];
                if (go.Equals(opElement.target))  //go必定不为null, element.target可能为null
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasSavedComponent(GameObject go, Component comp)
        {
            for (int i = 0; i < opElementList.Count; i++)
            {
                UIOpElement element = opElementList[i];
                if (!go.Equals(element.target))  //go必定不为null, element.target可能为null
                {
                    continue;    //target不同时，无需继续对组件列表进行遍历
                }

                for (int j = 0; j < element.componentList.Count; j++)
                {
                    Component savedComp = element.componentList[j];
                    if (comp.Equals(savedComp))  //comp必定不为null, savedComp可能为null
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public GameObject GetGameObjectByIndex(int index)
        {
            return m_OpElementList[index].target;
        }

        public Component GetComponentByIndexs(int index, int index2)
        {
            return m_OpElementList[index].GetComponentByIndex(index2);
        }

#if UNITY_EDITOR
        // todo 验证 添加的Go是否是本UIViewBehaviour所在Go的子物体
        //protected virtual void OnValidate()
        //{
        //    Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++" + m_OpElementList.Count);
        //    for (int i = 0; i < m_OpElementList.Count; i++)
        //    {
        //        GameObject target = m_OpElementList[i].target;
        //        //if (target != null && !this.Equals(target.GetComponentInParent<UIViewBehaviour>()))
        //        //{
        //        //    m_OpElementList[i].target == null;
        //        //}

        //        if (target != null)
        //        {
        //            Debug.Log("000000000, " + target);
        //            Debug.Log("111111111, " + this);
        //            UIViewBehaviour behaviour = target.GetComponentInParent<UIViewBehaviour>();
        //            Debug.Log("222222222, " + behaviour);

        //            Debug.Log(this.Equals(target.GetComponentInParent<UIViewBehaviour>()));
        //        }
        //    }
        //}

        protected virtual void Reset()
        {
            m_OpElementList = new List<UIOpElement>();
        }
#endif
        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;
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

            StartCoroutine(DoAfter(length, onFinish));
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

            StartCoroutine(DoAfter(length, onFinish));
        }

        private IEnumerator DoAfter(float duration, Action onFinish)
        {
            yield return new WaitForSeconds(duration);
            onFinish();
        }

        private void OnEnable() { onEnable?.Invoke(); }   //View创建时，不会被调用

        private void Start() { onStart?.Invoke(); }       //View创建时，后期绑定的UIViewBehaviour，不会被调用

        private void OnDisable() { onDisable?.Invoke(); }

        private void OnDestroy() { onDestroy?.Invoke(); }
    }
}
