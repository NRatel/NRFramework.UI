// https://github.com/NRatel/NRFramework.UI

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

        public T CreatePanel<T>(string panelId, string prefabPath, int sortingOrder) where T : UIPanel
        {
            Debug.Assert(!panelDict.ContainsKey(panelId));
            Debug.Assert(sortingOrder >= startOrder && sortingOrder <= endOrder);

            T panel = Activator.CreateInstance(typeof(T)) as T;
            panel.Create(panelId, this, prefabPath);
            panel.SetSortingOrder(sortingOrder);
            int siblingIndex = GetCurrentSiblingIndex(sortingOrder);
            panel.SetSiblingIndex(siblingIndex);
            panelDict.Add(panel.panelId, panel);
            UIManager.Instance.SetBackgroundAndFocus();

            return panel;
        }

        public T CreatePanel<T>(string panelId, string prefabPath) where T : UIPanel
        {
            return CreatePanel<T>(panelId, prefabPath, GetIncrementedSortingOrder());
        }

        public T CreatePanel<T>(string prefabPath, int sortingOrder) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath, sortingOrder);
        }

        public T CreatePanel<T>(string prefabPath) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath);
        }

        public void ClosePanel(string panelId, Action onFinish = null)
        {
            Debug.Assert(panelDict.ContainsKey(panelId));

            UIPanel panel = panelDict[panelId];
            panelDict.Remove(panelId);
            panel.Close(onFinish);

            UIManager.Instance.SetBackgroundAndFocus();
        }

        public void ClosePanel<T>(Action onFinish = null) where T : UIPanel
        {
            ClosePanel(typeof(T).Name, onFinish);
        }

        public void DestroyPanel(string panelId)
        {
            Debug.Assert(panelDict.ContainsKey(panelId));

            UIPanel panel = panelDict[panelId];
            panelDict.Remove(panelId);
            panel.Destroy();

            UIManager.Instance.SetBackgroundAndFocus();
        }

        public void DestroyPanel<T>() where T : UIPanel
        {
            DestroyPanel(typeof(T).Name);
        }

        public void SetPanelVisible(string panelId, bool visible)
        {
            Debug.Assert(panelDict.ContainsKey(panelId));
            UIPanel panel = panelDict[panelId];
            panel.SetVisible(visible);

            UIManager.Instance.SetBackgroundAndFocus();
        }

        public void SetPanelVisible<T>(bool visible) where T : UIPanel
        {
            SetPanelVisible(typeof(T).Name, visible);
        }

        public UIPanel GetPanel(string panelId)
        {
            return panelDict[panelId];
        }

        public UIPanel GetPanel<T>() where T : UIPanel
        {
            return GetPanel(typeof(T).Name);
        }

        public bool ExistPanel(string panelId)
        {
            return panelDict.ContainsKey(panelId);
        }

        public bool ExistPanel<T>()
        {
            return ExistPanel(typeof(T).Name);
        }

        public int FindPanelComponent<T>(string panelId, string compDefine, out T comp) where T : Component
        {
            comp = null;
            if (string.IsNullOrEmpty(panelId)) { return FindCompErrorCode.PANEL_ID_IS_NULL_OR_EMPTY; }
            UIPanel panel = GetPanel(panelId);
            return panel.FindComponent<T>(compDefine, out comp);
        }

        public int FindWidgetComponent<T>(string panelId, string[] widgetIds, string compDefine, out T comp) where T : Component
        {
            comp = null;
            if (string.IsNullOrEmpty(panelId)) { return FindCompErrorCode.PANEL_ID_IS_NULL_OR_EMPTY; }
            if (!ExistPanel(panelId)) { return FindCompErrorCode.NOT_EXIST_THIS_PANEL; }
            UIPanel panel = GetPanel(panelId);
            return panel.FindWidgetComponent<T>(widgetIds, compDefine, out comp);
        }

        private int GetIncrementedSortingOrder()
        {
            UIPanel topestPanel = null;
            foreach (KeyValuePair<string, UIPanel> kvPair in panelDict)
            {
                UIPanel panel = kvPair.Value;
                if (topestPanel == null || panel.canvas.sortingOrder > topestPanel.canvas.sortingOrder)
                {
                    topestPanel = panel;
                }
            }

            return topestPanel != null ? (topestPanel.canvas.sortingOrder + topestPanel.panelBehaviour.thickness + 1) : startOrder;
        }

        private int GetCurrentSiblingIndex(int sortingOrder)
        {
            List<UIPanel> panels = UIManager.Instance.FilterPanels((panel) =>
            { return sortingOrder > panel.canvas.sortingOrder; });

            return panels.Count;
        }
    }
}