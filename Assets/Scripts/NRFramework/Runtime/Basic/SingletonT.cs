using System;
using System.Reflection;

namespace NRFramework
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        static private T sm_Instance;

        static Singleton()
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
            if (ctor == null) { throw new Exception("\"" + typeof(T).ToString() + "\"类中不存在私有无参构造函数"); }
            sm_Instance = ctor.Invoke(null) as T;
        }

        static public T Instance { get { return sm_Instance; } }

        protected Singleton() { }
    }
}