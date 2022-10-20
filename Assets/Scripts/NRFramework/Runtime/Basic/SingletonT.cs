using System;
using System.Reflection;

namespace NRFramework
{
    /// <summary>
    /// 泛型单例抽象类（双重检查锁方式）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        static private T sm_Instance;
        static private object sm_Lock = new object();

        static public T Instance
        {
            get
            {
                if (sm_Instance == null)
                {
                    lock (sm_Lock)
                    {
                        if (sm_Instance == null)
                        {
                            //sm_Instance = new T();   //不可行。需要“公有的无参构造函数”
                            //sm_Instance = (T)Activator.CreateInstance(typeof(T));  //不可行，这种反射构造方式需要“公有的构造函数”
                            //sm_Instance = (T)Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).ToString());//不可行。这种反射构造方式需要“公有的构造函数”

                            //利用反射在类外部调用类的私有无参构造函数进行构造！但有开销
                            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
                            if (ctor == null) { throw new Exception("\"" + typeof(T).ToString() + "\"类中不存在私有无参构造函数"); }
                            sm_Instance = ctor.Invoke(null) as T;
                        }
                    }
                }
                return sm_Instance;
            }
        }

        //为了能被子类继承，父类的构造必须不能私有
        //private Singleton() { }
        protected Singleton() { }

        public virtual void Dispose()
        {
            sm_Instance = null;
        }
    }

    /// <summary>
    /// 泛型单例抽象类（静态初始化式方式）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SimpleSingleton<T> where T : SimpleSingleton<T>, new()
    {
        static private T sm_Instance;

        static public T Instance
        {
            get
            {
                return sm_Instance;
            }
        }

        static SimpleSingleton()
        {
            sm_Instance = new T();   //使用new，但构造无法设为 protected
        }

        public virtual void Dispose()
        {
            sm_Instance = null;
        }
    }
}