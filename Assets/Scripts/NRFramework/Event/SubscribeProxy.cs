using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    //代理订阅接口、对一组订阅者进行管理
    public class SubscribeProxy : ISubscribable
    {
        private ISubscribable m_Subject;    //被代理主题（传入EventDispatcher对象）
        private Dictionary<string, Subscriber> m_SubscriberDict { get; set; }

        public SubscribeProxy(ISubscribable subject)
        {
            m_Subject = subject;
            m_SubscriberDict = new Dictionary<string, Subscriber>();
        }

        #region implement interface
        public Subscriber Subscribe(string eventName, Action<object> handler)
        {
            Debug.Assert(!m_SubscriberDict.ContainsKey(eventName), "Subscribe失败！同一代理内禁止存在多处对同一事件的订阅，EventName: " + eventName);
            Subscriber subscriber = m_Subject.Subscribe(eventName, handler);
            m_SubscriberDict.Add(eventName, subscriber);
            return subscriber;
        }

        public void Unsubscribe(string eventName, Action<object> handler)
        {
            Debug.Assert(m_SubscriberDict.ContainsKey(eventName), "Unsubscribe失败！事件不存在: " + eventName);
            m_Subject.Unsubscribe(eventName, handler);
            m_SubscriberDict.Remove(eventName);
        }

        public void Unsubscribe(Subscriber subscriber)
        {
            Debug.Assert(m_SubscriberDict.ContainsKey(subscriber.eventName), "Unsubscribe失败！事件不存在: " + subscriber.eventName);
            m_Subject.Unsubscribe(subscriber);
            m_SubscriberDict.Remove(subscriber.eventName);
        }
        #endregion

        //同一代理内不存在多处对同一事件的订阅，所以可以直接使用事件名移除
        public void Unsubscribe(string eventName)
        {
            Debug.Assert(m_SubscriberDict.ContainsKey(eventName), "Unsubscribe失败！事件不存在: " + eventName);
            m_Subject.Unsubscribe(m_SubscriberDict[eventName]);
            m_SubscriberDict.Remove(eventName);
        }

        //清空代理容器（注意不要在遍历时进行移除操作）
        public void UnsubscribeAll()
        {
            foreach (var pair in m_SubscriberDict)
            {
                m_Subject.Unsubscribe(pair.Value);
            }
            m_SubscriberDict.Clear();
        }
    }
}
