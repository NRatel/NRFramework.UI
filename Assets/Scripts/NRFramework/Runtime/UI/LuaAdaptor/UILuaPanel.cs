#if USE_LUA
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace NRFramework
{
    public class UILuaPanel : UIPanel
    {
        public LuaTable @this;

        private Action<LuaTable, LuaTable> m_LuaOnCreated;
        private Action<LuaTable> m_LuaOnEnable;
        private Action<LuaTable> m_LuaOnStart;
        private Action<LuaTable> m_LuaOnDisable;
        private Action<LuaTable> m_LuaOnDestroy;

        public UILuaPanel(string panelId, LuaTable luaTable) : base(panelId)
        {
            @this = luaTable;
        }

        protected override void OnCreating()
        {
            base.OnCreating();

            SetMember("csPanel", this);
            SetMember("panelId", panelId);
            SetMember("behaviour", panelBehaviour);
            SetMember("rectTransform", rectTransform);
            SetMember("gameObject", gameObject);
            SetMember("parentRectTransform", parentRectTransform);
            SetMember("context", ((UILuaContext)context).luaContext);
            SetMember("parentCanvas", parentCanvas);
            SetMember("canvas", canvas);
            SetMember("gaphicRaycaster", gaphicRaycaster);

            GetMember("OnCreated", out m_LuaOnCreated);
            GetMember("OnEnable", out m_LuaOnEnable);
            GetMember("OnStart", out m_LuaOnStart);
            GetMember("OnDisable", out m_LuaOnDisable);
            GetMember("OnDestroy", out m_LuaOnDestroy);
        }

        //设置成员 供Lua调C#
        private void SetMember<TKey, TValue>(TKey key, TValue value)
        {
            @this.Set(key, value);
        }

        //获取成员 供C#调Lua
        //注意释放？
        private void GetMember<TKey, TValue>(TKey key, out TValue value)
        {
            @this.Get(key, out value);
        }

#region UIView生命周期
        protected override void OnCreated(UIContext context)
        {
            base.OnCreated(context);
            m_LuaOnCreated?.Invoke(@this, ((UILuaContext)context).luaContext);
        }
#endregion

#region 来自UIBehaviour的生命周期
        protected override void OnEnable()
        {
            base.OnEnable();
            m_LuaOnEnable?.Invoke(@this);
        }
        
        protected override void OnStart()
        {
            base.OnStart();
            m_LuaOnStart?.Invoke(@this);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            m_LuaOnDisable?.Invoke(@this);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_LuaOnDestroy?.Invoke(@this);
            @this.Dispose();
            @this = null;
        }
#endregion
    }
}
#endif