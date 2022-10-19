using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    public class Subscriber     //订阅者
    {
        internal string eventName { set; get; }
        internal Action<object> handler { set; get; }
    }

    public interface ISubscribable  //可被订阅/被取消订阅的
    {
        Subscriber Subscribe(string eventName, Action<object> handler);
        void Unsubscribe(string eventName, Action<object> handler);
        void Unsubscribe(Subscriber subscribeId);
        //void UnsubscribeAll(string eventName);    //移除eventName对应的所有handler。存在订阅代理时不安全，故移除
    }

    public interface IDispatchable  //可派发的
    {
        void Dispatch(string eventName, object eventParameter);
    }

    public class EventDispatcher : ISubscribable, IDispatchable //事件调度器
    {
        private Dictionary<string, Event> m_EventDict { get; set; }

        public EventDispatcher()
        {
            m_EventDict = new Dictionary<string, Event>();
        }

        #region implement interface
        public Subscriber Subscribe(string eventName, Action<object> handler)
        {
            if (!m_EventDict.ContainsKey(eventName))
            {
                m_EventDict.Add(eventName, new Event(eventName));
            }

            Debug.Assert(!m_EventDict[eventName].Has(handler), "Subscribe 失败！同一事件不应存在重复的处理器，EventName: " + eventName);
            if (!m_EventDict[eventName].Has(handler))
            {
                m_EventDict[eventName].AddListener(handler);
            }
            return new Subscriber() { eventName = eventName, handler = handler };
        }

        public void Unsubscribe(string eventName, Action<object> handler)
        {
            Debug.Assert(m_EventDict.ContainsKey(eventName), "Unsubscribe失败！EventName不存在: " + eventName);
            if (m_EventDict.ContainsKey(eventName))
            {
                bool result = m_EventDict[eventName].RemoveListener(handler);
                Debug.Assert(result, "Unsubscribe失败！handler不存在");

                if (m_EventDict[eventName].handlerCount == 0)
                {
                    m_EventDict.Remove(eventName);
                }
            }
        }

        public void Unsubscribe(Subscriber subscriber)
        {
            Unsubscribe(subscriber.eventName, subscriber.handler);
        }

        public void Dispatch(string eventName, object eventParameter)
        {
            //Debug.Assert(m_EventDict.ContainsKey(eventName), "Dispatch失败！EventName不存在: " + eventName);
            if (m_EventDict.ContainsKey(eventName))
            {
                m_EventDict[eventName].Invoke(eventParameter);
            }
        }
        #endregion
    }
}
