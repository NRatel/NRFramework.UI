using System;
using System.Collections.Generic;

namespace NRFramework
{
    public class Event
    {
        private string m_EventName { get; set; }
        private List<Action<object>> m_HandlerList { get; set; }
        public int handlerCount { get { return m_HandlerList.Count; } }

        public Event(string eventName)
        {
            m_EventName = eventName;
            m_HandlerList = new List<Action<object>>();
        }

        public void AddListener(Action<object> handler)
        {
            m_HandlerList.Add(handler);
        }

        public bool RemoveListener(Action<object> handler)
        {
            return m_HandlerList.Remove(handler);
        }

        public void RemoveAllListener()
        {
            m_HandlerList.Clear();
        }

        public void Invoke(object eventParameter)
        {
            foreach (Action<object> handler in m_HandlerList)
            {
                handler(eventParameter);
            }
        }

        public bool Has(Action<object> handler)
        {
            return m_HandlerList.Contains(handler);
        }
    }

}
