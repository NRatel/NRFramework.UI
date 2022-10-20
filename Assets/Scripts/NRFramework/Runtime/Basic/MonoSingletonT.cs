//1、设计上仅允许主动调用Dispose() 或 应用退出时销毁单例，其他销毁操作均非法！
//2、应避免在应用退出后访问单例,尤其是在其他物体的OnDestroy中。
//   原因：应用退出时，单例会被Unity自动销毁，且Unity在销毁物体是随机进行的。

using System;
using UnityEngine;

namespace NRFramework
{
    [DisallowMultipleComponent]
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        static private T sm_Instance;
        static private bool sm_MarkAsDestroy = false;
        static private bool sm_AppQuitted;
        static private readonly object sm_Lock = new object();

        public static T Instance
        {
            get
            {
                Debug.Assert(!sm_MarkAsDestroy && !sm_AppQuitted, "单例已销毁，仍然尝试访问");
                //if (sm_MarkAsDestroy || sm_AppQuitted) { return null; }

                if (sm_Instance == null)
                {
                    lock (sm_Lock)
                    {
                        if (sm_Instance == null)
                        {
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
                            string parentName = "#MonoSingletons#";
                            GameObject parentObj = GameObject.Find(parentName);
                            Transform parent = parentObj != null ? parentObj.transform : new GameObject(parentName).transform;
                            GameObject.DontDestroyOnLoad(parent.gameObject);
                            sm_Instance = new GameObject(name).AddComponent<T>();
                            sm_Instance.transform.SetParent(parent, false);
                            sm_Instance.hideFlags = hideFlags;
                            sm_MarkAsDestroy = false;
                        }
                    }  
                }
                return sm_Instance;
            }
        }

        public virtual void Dispose()
        {
            sm_MarkAsDestroy = true;
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            Debug.Assert(sm_MarkAsDestroy || sm_AppQuitted, "MonoSingleton 被意外销毁！");
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