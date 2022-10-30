using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    /// <summary>
    /// 这只是一个根/组
    /// </summary>
    public class UILayer
    {
        public string layerId;
        public int startOrder;
        public int endOrder;

        private Dictionary<string, UIPanel> m_PanelDict;

        public UILayer()
        {
            m_PanelDict = new Dictionary<string, UIPanel>();
        }

        internal void AddPanel(UIPanel panel, int fixedOrder)
        {
            //默认起始
            int sortingOrder = startOrder; 

            if (fixedOrder >= 0)
            {
                //若有指定，则使用指定的SortintOrder
                sortingOrder = startOrder + fixedOrder; 
            }
            else
            {
                UIPanel topPanel = GetTopPanel();
                if (topPanel != null)
                {
                    //若存在topPanel，在其基础上自增
                    sortingOrder = topPanel.canvas.sortingOrder + topPanel.panelBehaviour.thickness + 1; 
                }
            }

            Debug.Assert(sortingOrder <= endOrder, "sortingOrder超出设定的endOrder");

            panel.SetSortingOrder(sortingOrder);
            m_PanelDict.Add(panel.panelId, panel);
        }

        internal void RemovePanel(string panelId)
        {
            m_PanelDict.Remove(panelId);
        }

        public Dictionary<string, UIPanel> GetPanelDict()
        {
            return m_PanelDict;
        }

        public UIPanel GetPanel(string panelId)
        {
            return m_PanelDict[panelId];
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