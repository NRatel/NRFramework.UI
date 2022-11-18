// https://github.com/NRatel/NRFramework.UI

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
            Debug.Assert(!panelDict.ContainsKey(panelId));  //panel已存在

            UIPanelLuaCommon panel = new UIPanelLuaCommon();
            panel.Create(panelId, this, prefabPath, luaPanel);
            int targetSortingOrder = GetIncrementedSortingOrder();
            panel.SetSortingOrder(targetSortingOrder);
            int targetSiblingIndex = GetCurrentSiblingIndex(targetSortingOrder);
            panel.SetSiblingIndex(targetSiblingIndex);
            UIManager.Instance.SetBackgroundAndFocus();

            return panel;
        }
    }
}
# endif

