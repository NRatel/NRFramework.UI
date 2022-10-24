# if USE_LUA
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace NRFramework
{
    public partial class UIManager
    {
        public UILuaPanel OpenPanel(string panelId, string prefabPath, LuaTable luaPanel, LuaTable luaSetting, LuaTable luaContext = null)
        {
            UILuaPanel panel = new UILuaPanel();
            panel.Init(panelId, luaPanel, m_UICanvas, prefabPath, luaContext);
            SetAndCache(panel, luaSetting);
            SortAllPanels();
            return panel;
        }
    }

    public partial class UIPanelOpenSetting
    {
        public static implicit operator UIPanelOpenSetting(LuaTable luaSetting)
        {
            string layerId;
            luaSetting.Get("layerId", out layerId);
            Debug.Assert(!string.IsNullOrEmpty(layerId));

            int fixedOrder = -1;
            if (luaSetting.ContainsKey("fixedOrder"))
            {
                luaSetting.Get("fixedOrder", out fixedOrder);
            }

            return new UIPanelOpenSetting() { layerId = layerId, fixedOrder = fixedOrder };
        }
    }

    public class UILuaContext : UIContext
    {
        public LuaTable luaContext;
        public UILuaContext(LuaTable luaContext)
        {
            this.luaContext = luaContext;
        }
    }

    public partial class UIContext
    {
        public static implicit operator UIContext(LuaTable luaContext)
        {
            return new UILuaContext(luaContext);
        }
    }
}
# endif

