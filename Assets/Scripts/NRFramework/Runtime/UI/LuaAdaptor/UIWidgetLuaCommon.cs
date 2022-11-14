﻿// https://github.com/NRatel/NRFramework.UI

#if USE_LUA
using System;
using UnityEngine;
using XLua;

namespace NRFramework
{
    public class UIWidgetLuaCommon : UIWidget
    {
        public LuaTable @this;

        private Action<LuaTable> m_LuaOnCreating;
        private Action<LuaTable> m_LuaOnCreated;
        private Action<LuaTable> m_LuaOnDestroying;
        private Action<LuaTable> m_LuaOnDestroyed;

        public void Create(string widgetId, UIView parentView, RectTransform parentRectTransform, string prefabPath, LuaTable luaTable)
        {
            @this = luaTable;
            base.Create(widgetId, parentView, parentRectTransform.GetComponent<RectTransform>(), prefabPath);
        }

        public void Create(string panelId, UIView parentView, RectTransform parentRectTransform, UIWidgetBehaviour widgetBehaviour, LuaTable luaTable)
        {
            @this = luaTable;
            base.Create(panelId, parentView, parentRectTransform.GetComponent<RectTransform>(), widgetBehaviour);
        }

        protected override void OnCreating()
        {
            base.OnCreating();

            SetMember("csWidget", this);
            SetMember("widgetId", widgetId);
            SetMember("behaviour", widgetBehaviour);
            SetMember("rectTransform", rectTransform);
            SetMember("gameObject", gameObject);
            SetMember("parentRectTransform", parentRectTransform);
            SetMember("parentView", parentView);

            GetMember("OnCreating", out m_LuaOnCreating);
            GetMember("OnCreated", out m_LuaOnCreated);
            GetMember("OnDestroying", out m_LuaOnDestroying);
            GetMember("OnDestroyed", out m_LuaOnDestroyed);

            m_LuaOnCreating?.Invoke(@this);

        }

        protected override void OnCreated()
        {
            base.OnCreated();
            m_LuaOnCreated?.Invoke(@this);
        }

        protected override void OnDestroying()
        {
            m_LuaOnDestroying?.Invoke(@this);

            m_LuaOnCreating = null;
            m_LuaOnCreated = null;
            m_LuaOnDestroying = null;

            base.OnDestroying();
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            m_LuaOnDestroyed?.Invoke(@this);

            m_LuaOnDestroyed = null;
        }

        //设置成员 供Lua调C#
        private void SetMember<TKey, TValue>(TKey key, TValue value)
        {
            @this.Set(key, value);
        }

        //获取成员 供C#调Lua
        private void GetMember<TKey, TValue>(TKey key, out TValue value)
        {
            @this.Get(key, out value);
        }
    }
}
#endif