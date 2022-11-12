using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    public class SubscribeProxy : ISubscribable
    {
        private ISubscribable m_Subject;
        private Dictionary<string, Subscriber> m_SubscriberDict;

        public SubscribeProxy(ISubscribable subject)
        {
            m_Subject = subject;
            m_SubscriberDict = new Dictionary<string, Subscriber>();
        }

        #region implement interface
        public Subscriber Subscribe(string eventName, Action<EventArgument> handler)
        {
            Debug.Assert(!m_SubscriberDict.ContainsKey(eventName));
            Subscriber subscriber = m_Subject.Subscribe(eventName, handler);
            m_SubscriberDict.Add(eventName, subscriber);
            return subscriber;
        }

        public void Unsubscribe(string eventName, Action<EventArgument> handler)
        {
            Debug.Assert(m_SubscriberDict.ContainsKey(eventName));
            m_Subject.Unsubscribe(eventName, handler);
            m_SubscriberDict.Remove(eventName);
        }

        public void Unsubscribe(Subscriber subscriber)
        {
            Debug.Assert(m_SubscriberDict.ContainsKey(subscriber.eventName));
            m_Subject.Unsubscribe(subscriber);
            m_SubscriberDict.Remove(subscriber.eventName);
        }
        #endregion

        public void Unsubscribe(string eventName)
        {
            Debug.Assert(m_SubscriberDict.ContainsKey(eventName));
            m_Subject.Unsubscribe(m_SubscriberDict[eventName]);
            m_SubscriberDict.Remove(eventName);
        }

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
