using System;
using System.Collections.Generic;

namespace NRFramework
{
    public class EventArgument { }

    public class Event
    {
        private string m_EventName;

        private List<Action<EventArgument>> m_HandlerList;

        public int handlerCount { get { return m_HandlerList.Count; } }

        public Event(string eventName)
        {
            m_EventName = eventName;
            m_HandlerList = new List<Action<EventArgument>>();
        }

        public void AddListener(Action<EventArgument> handler)
        {
            m_HandlerList.Add(handler);
        }

        public bool RemoveListener(Action<EventArgument> handler)
        {
            return m_HandlerList.Remove(handler);
        }

        public void RemoveAllListener()
        {
            m_HandlerList.Clear();
        }

        public void Invoke(EventArgument eventArgument)
        {
            foreach (Action<EventArgument> handler in m_HandlerList)
            {
                handler(eventArgument);
            }
        }

        public bool Has(Action<EventArgument> handler)
        {
            return m_HandlerList.Contains(handler);
        }
    }

}
