// https://github.com/NRatel/NRFramework.UI

using System;
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
        //public event Action onUpdate;
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
        protected virtual void OnValidate()
        {
            //// todo
            //// 添加的 GameObject 必须是本 UIViewBehaviour 所在 GameObject 的子物体
            //for (int i = 0; i < m_OpElementList.Count; i++)
            //{
            //    GameObject target = m_OpElementList[i].target;
            //    if (target != null)
            //    {
            //        UIViewBehaviour parentBehaviour = target.GetComponentInParent<UIViewBehaviour>();

            //        //Debug.Log("xxx0: " + target);
            //        //Debug.Log("xxx1: " + parentBehaviour);

            //        //Debug.Log("sss2: " + (parentBehaviour == null));
            //        //Debug.Log("sss3: " + (parentBehaviour is UIViewBehaviour));
            //        //Debug.Log("sss4: " + parentBehaviour.GetInstanceID());

            //        // 似乎有bug，parentBehaviour 有时为 null。
            //        Debug.Assert(this.Equals(parentBehaviour));
            //    }
            //}
        }

        protected virtual void Reset()
        {
            m_OpElementList = new List<UIOpElement>();
        }
#endif
        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;
        }

        private void OnEnable() { onEnable?.Invoke(); }   //View创建时，不会被调用

        private void Start() { onStart?.Invoke(); }       //View创建时，后期绑定的UIViewBehaviour，不会被调用

        //private void Update() { onUpdate?.Invoke(); }

        private void OnDisable() { onDisable?.Invoke(); }

        private void OnDestroy() { onDestroy?.Invoke(); }
    }
}
