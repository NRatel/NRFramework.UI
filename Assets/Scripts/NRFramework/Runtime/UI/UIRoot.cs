using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    /// <summary>
    /// 这只是一个根/组
    /// </summary>
    public partial class UIRoot
    {
        public string rootId;
        public int startOrder;
        public int endOrder;

        public Dictionary<string, UIPanel> panelDict { private set; get; }

        public UIRoot()
        {
            panelDict = new Dictionary<string, UIPanel>();
        }

        /// <summary>
        /// 创建一个UI面板
        /// </summary>
        /// <typeparam name="T">自定义panel类</typeparam>
        /// <param name="panelId">panelId</param>
        /// <param name="prefabPath">预设路径</param>
        /// <returns></returns>
        public T CreatePanel<T>(string panelId, string prefabPath) where T : UIPanel
        {
            Debug.Assert(!panelDict.ContainsKey(panelId), "panel已存在");

            T panel = Activator.CreateInstance(typeof(T)) as T;
            panel.Create(panelId, this, prefabPath);
            panel.SetSortingOrder(GetCurSortingOrder());
            panelDict.Add(panel.panelId, panel);
            UIManager.Instance.SortAllPanels();

            return panel;
        }

        /// <summary>
        /// 创建一个UI面板
        /// </summary>
        /// <typeparam name="T">自定义panel类</typeparam>
        /// <param name="panelId">panelId</param>
        /// <param name="panelBehaviour">待绑定的panel行为组件</param>
        /// <param name="fixedOrder">指定sortingOrder，正为指定（浮动）；负为自动自增。</param>
        /// <returns></returns>
        public T CreatePanel<T>(string panelId, UIPanelBehaviour panelBehaviour) where T : UIPanel
        {
            Debug.Assert(!panelDict.ContainsKey(panelId), "panel已存在");

            T panel = Activator.CreateInstance(typeof(T)) as T;
            panel.Create(panelId, this, panelBehaviour);
            panel.SetSortingOrder(GetCurSortingOrder());
            panelDict.Add(panel.panelId, panel);
            UIManager.Instance.SortAllPanels();

            return panel;
        }

        /// <summary>
        /// 创建一个UI面板（以类型名作为panelId，方便使用）
        /// </summary>
        /// <typeparam name="T">自定义panel类</typeparam>
        /// <param name="prefabPath">预设路径</param>
        /// <param name="fixedOrder">指定sortingOrder，正为指定（浮动）；负为自动自增。</param>
        /// <returns></returns>
        public T CreatePanel<T>(string prefabPath) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath);
        }

        /// <summary>
        /// 创建一个UI面板（以类型名作为panelId，方便使用）
        /// </summary>
        /// <typeparam name="T">自定义panel类</typeparam>
        /// <param name="panelBehaviour">待绑定的panel行为组件</param>
        /// <param name="fixedOrder">指定sortingOrder，正为指定（浮动）；负为自动自增。</param>
        /// <returns></returns>
        public T CreatePanel<T>(UIPanelBehaviour panelBehaviour) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, panelBehaviour);
        }

        internal void RemovePanelRef(string panelId)
        {
            panelDict.Remove(panelId);
        }

        public UIPanel GetPanel(string panelId)
        {
            return panelDict[panelId];
        }

        public UIPanel GetPanel<T>() where T : UIPanel
        {
            return GetPanel(typeof(T).Name);
        }

        public UIPanel GetTopestPanel()
        {
            UIPanel topPanel = null;
            foreach (KeyValuePair<string, UIPanel> kvPair in panelDict)
            {
                UIPanel panel = kvPair.Value;
                if (topPanel == null || panel.canvas.sortingOrder > topPanel.canvas.sortingOrder)
                {
                    topPanel = panel;
                }
            }
            return topPanel;
        }

        private int GetCurSortingOrder()
        {
            UIPanel topestPanel = GetTopestPanel();
            if (topestPanel == null) { return startOrder; }

            return topestPanel.canvas.sortingOrder + topestPanel.panelBehaviour.thickness + 1;
        }

    }
}