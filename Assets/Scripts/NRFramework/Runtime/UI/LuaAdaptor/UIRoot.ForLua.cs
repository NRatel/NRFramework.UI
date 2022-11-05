# if USE_LUA
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace NRFramework
{
    public partial class UIRoot
    {
        public UIPanelLuaCommon CreatePanel(string panelId, string prefabPath, LuaTable luaPanel, int fixedOrder = -1)
        {
            Debug.Assert(!panelDict.ContainsKey(panelId), "panel已存在");

            UIPanelLuaCommon panel = new UIPanelLuaCommon();
            panel.Create(panelId, this, prefabPath, luaPanel);

            SetPanelSortingOrder(panel, fixedOrder);
            panelDict.Add(panel.panelId, panel);
            UIManager.Instance.SortAllPanelSiblings();

            return panel;
        }

        public UIPanelLuaCommon CreatePanel(string panelId, UIPanelBehaviour panelBehaviour, LuaTable luaPanel, int fixedOrder = -1)
        {
            Debug.Assert(!panelDict.ContainsKey(panelId), "panel已存在");

            UIPanelLuaCommon panel = new UIPanelLuaCommon();
            panel.Create(panelId, this, panelBehaviour, luaPanel);

            SetPanelSortingOrder(panel, fixedOrder);
            panelDict.Add(panel.panelId, panel);
            UIManager.Instance.SortAllPanelSiblings();

            return panel;
        }
    }
}
# endif

