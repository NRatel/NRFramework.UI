using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
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

        public T CreatePanel<T>(string prefabPath) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath);
        }

        public T CreatePanel<T>(UIPanelBehaviour panelBehaviour) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, panelBehaviour);
        }

        public void ClosePanel(string panelId, Action onFinish = null)
        {
            UIPanel panel = panelDict[panelId];
            panelDict.Remove(panelId);
            panel.Close(onFinish);

            UIManager.Instance.SortAllPanels();
        }

        public void ClosePanelWithoutAnim(string panelId)
        {
            UIPanel panel = panelDict[panelId];
            panelDict.Remove(panelId);
            panel.CloseWithoutAnim();

            UIManager.Instance.SortAllPanels();
        }

        public void ClosePanel<T>(Action onFinish = null)
        {
            ClosePanel(typeof(T).Name, onFinish);
        }

        public void ClosePanelWithoutAnim<T>()
        {
            ClosePanelWithoutAnim(typeof(T).Name);
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