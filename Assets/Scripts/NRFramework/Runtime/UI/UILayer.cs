using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    /// <summary>
    /// 这只是一个虚拟的层，实质是一个order范围
    /// </summary>
    public class UILayer
    {
        public string layerId;
        public int startOrder;
        public int endOrder;

        public Dictionary<string, UIPanel> m_PanelDict;

        public UILayer()
        {
            m_PanelDict = new Dictionary<string, UIPanel>();
        }

        public void AddPanel(UIPanel panel, int fixedOrder)
        {
            int sortingOrder = startOrder; //默认起始
            if (fixedOrder >= 0)
            {
                sortingOrder = startOrder + fixedOrder; //若有指定，则使用指定的SortintOrder
            }
            else
            {
                UIPanel topPanel = GetTopPanel();
                if (topPanel != null)
                {
                    sortingOrder = topPanel.canvas.sortingOrder + topPanel.panelBehaviour.thickness + 1; //若存在topPanel，在其基础上自增
                }
            }

            Debug.Assert(sortingOrder <= endOrder, "sortingOrder超出设定的endOrder");

            panel.SetSortingOrder(sortingOrder);
            m_PanelDict.Add(panel.panelId, panel);
        }

        public void RemovePanel(string panelId)
        {
            m_PanelDict.Remove(panelId);
        }

        public UIPanel GetTopPanel()
        {
            UIPanel topPanel = null;
            foreach (KeyValuePair<string, UIPanel> kvPair in m_PanelDict)
            {
                UIPanel panel = kvPair.Value;
                if (topPanel == null || panel.canvas.sortingOrder > topPanel.canvas.sortingOrder)
                {
                    topPanel = panel;
                }
            }
            return topPanel;
        }
    }
}