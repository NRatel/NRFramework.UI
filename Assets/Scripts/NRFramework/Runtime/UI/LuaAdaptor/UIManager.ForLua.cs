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
        public UILuaPanel CreatePanel(string panelId, string prefabPath, LuaTable luaPanel, string layerId, int fixedOrder = -1)
        {
            UILuaPanel panel = new UILuaPanel();
            panel.Create(panelId, luaPanel, m_UICanvas, prefabPath, luaPanel);

            Debug.Assert(m_LayerDict.ContainsKey(layerId), "layer不存在");

            m_LayerDict[layerId].AddPanel(panel, fixedOrder);
            m_PanelToLayerMap.Add(panel.panelId, layerId);

            SortAllSibling();
            return panel;
        }

        public UILuaPanel CreatePanel(string panelId, UIPanelBehaviour panelBehaviour, LuaTable luaPanel, string layerId, int fixedOrder = -1)
        {
            UILuaPanel panel = new UILuaPanel();
            panel.Create(panelId, m_UICanvas, panelBehaviour, luaPanel);

            Debug.Assert(m_LayerDict.ContainsKey(layerId), "layer不存在");

            m_LayerDict[layerId].AddPanel(panel, fixedOrder);
            m_PanelToLayerMap.Add(panel.panelId, layerId);

            SortAllSibling();
            return panel;
        }
    }
}
# endif

