#if USE_LUA
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace NRFramework
{
    public class UIPanelLuaCommon : UIPanel
    {
        public LuaTable @this;

        private Action<LuaTable> m_LuaOnCreated;

        public void Create(string panelId, Canvas parentCanvas, string prefabPath, LuaTable luaTable)
        {
            @this = luaTable;
            base.Create(panelId, parentCanvas.GetComponent<RectTransform>(), prefabPath);
        }

        public void Create(string panelId, Canvas parentCanvas, UIPanelBehaviour panelBehaviour, LuaTable luaTable)
        {
            @this = luaTable;
            base.Create(panelId, parentCanvas.GetComponent<RectTransform>(), panelBehaviour);
        }

        protected override void OnInternalCreating()
        {
            base.OnInternalCreating();

            SetMember("csPanel", this);
            SetMember("panelId", panelId);
            SetMember("behaviour", panelBehaviour);
            SetMember("rectTransform", rectTransform);
            SetMember("gameObject", gameObject);
            SetMember("parentRectTransform", parentRectTransform);
            SetMember("parentUIRoot", parentUIRoot);
            SetMember("canvas", canvas);
            SetMember("gaphicRaycaster", gaphicRaycaster);

            GetMember("OnCreated", out m_LuaOnCreated);
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
        protected override void OnCreated()
        {
            base.OnCreated();
            m_LuaOnCreated?.Invoke(@this);
        }

        //...
#endregion
    }
}
#endif