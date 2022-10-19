using System;
using System.Reflection;

public abstract class Singleton<T> where T : Singleton<T>
{
    static private T instance;

    static public T GetInstance()
    {
        return instance;
    }

    static Singleton()
    {
        //使用反射创建实例，安全但有开销
        var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
        var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
        if (ctor == null)
        {
            throw new Exception("\"" + typeof(T).ToString() + "\"类中不存在私有无参构造函数");
        }
        instance = (T)ctor.Invoke(null);
    }

    protected Singleton() { }

    static public void Dispose()
    {
        instance = null;
    }
}

public abstract class SimpleSingleton<T> where T : Singleton<T>, new()
{
    static private T instance;

    static public T GetInstance()
    {
        return instance;
    }

    static SimpleSingleton()
    {
        instance = new T();
    }

    static public void Dispose()
    {
        instance = null;
    }
}