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
        public UILuaPanel CreatePanel(string panelId, string prefabPath, string layerId, int fixedOrder, LuaTable luaPanel)
        {
            UILuaPanel panel = new UILuaPanel();
            panel.Create(panelId, luaPanel, m_UICanvas, prefabPath, luaPanel);

            Debug.Assert(m_LayerDict.ContainsKey(layerId), "layer不存在");

            UILayer layer = m_LayerDict[layerId];
            layer.AddPanel(panel, fixedOrder);
            m_AllPanelDict.Add(panel.panelId, panel);
            m_PanelToLayerMap.Add(panel.panelId, layerId);

            SortAllPanels();
            return panel;
        }

        public UILuaPanel CreatePanel(string panelId, UIPanelBehaviour panelBehaviour, string layerId, int fixedOrder, LuaTable luaPanel)
        {
            UILuaPanel panel = new UILuaPanel();
            panel.Create(panelId, m_UICanvas, panelBehaviour, luaPanel);

            Debug.Assert(m_LayerDict.ContainsKey(layerId), "layer不存在");

            UILayer layer = m_LayerDict[layerId];
            layer.AddPanel(panel, fixedOrder);
            m_AllPanelDict.Add(panel.panelId, panel);
            m_PanelToLayerMap.Add(panel.panelId, layerId);

            SortAllPanels();
            return panel;
        }
    }
}
# endif

