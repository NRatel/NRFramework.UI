using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    public class Subscriber
    {
        internal string eventName { set; get; }
        internal Action<EventArgument> handler { set; get; }
    }

    public interface ISubscribable
    {
        Subscriber Subscribe(string eventName, Action<EventArgument> handler);
        void Unsubscribe(string eventName, Action<EventArgument> handler);
        void Unsubscribe(Subscriber subscribeId);
        //void UnsubscribeAll(string eventName);
    }

    public interface IPublishable 
    {
        void Publish(string eventName, EventArgument eventArgument);
    }

    public class EventDispatcher : ISubscribable, IPublishable
    {
        private Dictionary<string, Event> m_EventDict;

        public EventDispatcher()
        {
            m_EventDict = new Dictionary<string, Event>();
        }

        #region implement interface
        public Subscriber Subscribe(string eventName, Action<EventArgument> handler)
        {
            if (!m_EventDict.ContainsKey(eventName))
            {
                m_EventDict.Add(eventName, new Event(eventName));
            }

            Debug.Assert(!m_EventDict[eventName].Has(handler));
            m_EventDict[eventName].AddListener(handler);

            return new Subscriber() { eventName = eventName, handler = handler };
        }

        public void Unsubscribe(string eventName, Action<EventArgument> handler)
        {
            Debug.Assert(m_EventDict.ContainsKey(eventName));
            //if (!m_EventDict.ContainsKey(eventName)) { return; }

            bool result = m_EventDict[eventName].RemoveListener(handler);
            Debug.Assert(result);

            if (m_EventDict[eventName].handlerCount == 0)
            {
                m_EventDict.Remove(eventName);
            }
        }

        public void Unsubscribe(Subscriber subscriber)
        {
            Unsubscribe(subscriber.eventName, subscriber.handler);
        }

        public void Publish(string eventName, EventArgument eventArgument)
        {
            //Debug.Assert(m_EventDict.ContainsKey(eventName));
            if (!m_EventDict.ContainsKey(eventName)) { return; }
            m_EventDict[eventName].Invoke(eventArgument);
        }
        #endregion
    }
}
