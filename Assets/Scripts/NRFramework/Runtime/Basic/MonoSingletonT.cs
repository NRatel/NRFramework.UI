//1、强制检查单例的销毁，除应用退出外其他销毁操作均非法！
//2、应避免在应用退出后访问单例，尤其是在其他物体的OnDestroy中。 
//（应用退出时，单例会被Unity自动销毁，且Unity在销毁物体是随机进行的）

using System;
using UnityEngine;

namespace NRFramework
{
    [DisallowMultipleComponent]
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private const string k_RootName = "#MonoSingletons#";
        static private T sm_Instance;
        static private bool sm_AppQuitted;

        static internal Transform commonRoot { get; private set; }

        static MonoSingleton()
        {
            GameObject rootObj = GameObject.Find(k_RootName);
            if (rootObj == null) 
            {
                rootObj = new GameObject(k_RootName);
                GameObject.DontDestroyOnLoad(rootObj);
                commonRoot = rootObj.transform;
            }

            Type type = typeof(T);
            string name = type.Name;   //默认类名
            HideFlags hideFlags = HideFlags.None;
#if UNITY_EDITOR
            var attributes = type.GetCustomAttributes(true);
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is MonoSingletonSetting mss)
                {
                    hideFlags = mss.HideFlags;
                    name = !string.IsNullOrEmpty(mss.NameInHierarchy) ? mss.NameInHierarchy : name;
                    break;
                }
            }
#endif
            sm_Instance = new GameObject(name).AddComponent<T>();
            sm_Instance.transform.SetParent(commonRoot, false);
            sm_Instance.hideFlags = hideFlags;
        }

        static public T Instance
        {
            get
            {
#if UNITY_EDITOR
                Debug.Assert(!sm_AppQuitted, "单例已销毁，仍尝试访问");
#endif
                return sm_Instance;
            }
        }

        protected virtual void OnDestroy()
        {
//#if UNITY_EDITOR
//            Debug.Assert(sm_AppQuitted, "MonoSingleton 可能被意外销毁了"); //不准确，todo
//#endif
            sm_Instance = null;
        }

        protected virtual void OnApplicationQuit()
        {
            sm_AppQuitted = true;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonSetting : Attribute
    {
        private HideFlags m_HideFlags;
        private string m_NameInHierarchy;   //null或""时默认使用类名

        public MonoSingletonSetting(HideFlags hideFlags, string nameInHierarchy = null)
        {
            m_HideFlags = hideFlags;
            m_NameInHierarchy = nameInHierarchy;
        }

        public string NameInHierarchy
        {
            get { return m_NameInHierarchy; }
        }

        public HideFlags HideFlags
        {
            get { return m_HideFlags; }
        }
    }

}