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
        public UIPanelLuaCommon CreatePanel(string panelId, string prefabPath, LuaTable luaPanel)
        {
            Debug.Assert(!panelDict.ContainsKey(panelId), "panel已存在");

            UIPanelLuaCommon panel = new UIPanelLuaCommon();
            panel.Create(panelId, this, prefabPath, luaPanel);
            int targetSortingOrder = GetTargetSortingOrder();
            panel.SetSortingOrder(targetSortingOrder);
            panel.rectTransform.SetSiblingIndex(GetTargetSiblingIndex(targetSortingOrder));
            UIManager.Instance.ChangeFocus();

            return panel;
        }

        public UIPanelLuaCommon CreatePanel(string panelId, UIPanelBehaviour panelBehaviour, LuaTable luaPanel)
        {
            Debug.Assert(!panelDict.ContainsKey(panelId), "panel已存在");

            UIPanelLuaCommon panel = new UIPanelLuaCommon();
            panel.Create(panelId, this, panelBehaviour, luaPanel);
            int targetSortingOrder = GetTargetSortingOrder();
            panel.SetSortingOrder(targetSortingOrder);
            panel.rectTransform.SetSiblingIndex(GetTargetSiblingIndex(targetSortingOrder));
            panelDict.Add(panel.panelId, panel);
            UIManager.Instance.ChangeFocus();

            return panel;
        }
    }
}
# endif

