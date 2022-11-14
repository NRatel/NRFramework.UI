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

        public T CreatePanel<T>(string panelId, UIPanelBehaviour panelBehaviour, int sortingOrder) where T : UIPanel
        {
            Debug.Assert(!panelDict.ContainsKey(panelId));
            Debug.Assert(sortingOrder >= startOrder && sortingOrder <= endOrder);

            T panel = Activator.CreateInstance(typeof(T)) as T;
            panel.Create(panelId, this, panelBehaviour);
            panel.SetSortingOrder(sortingOrder);
            int siblingIndex = GetCurrentSiblingIndex(sortingOrder);
            panel.SetSiblingIndex(siblingIndex);
            panelDict.Add(panel.panelId, panel);
            UIManager.Instance.SetBackgroundAndFocus();

            return panel;
        }

        public T CreatePanel<T>(string prefabPath, int sortingOrder) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath, sortingOrder);
        }

        public T CreatePanel<T>(UIPanelBehaviour panelBehaviour, int sortingOrder) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, panelBehaviour, sortingOrder);
        }

        public T CreatePanel<T>(string panelId, string prefabPath) where T : UIPanel
        {
            return CreatePanel<T>(panelId, prefabPath, GetIncrementedSortingOrder());
        }

        public T CreatePanel<T>(string panelId, UIPanelBehaviour panelBehaviour) where T : UIPanel
        {
            return CreatePanel<T>(panelId, panelBehaviour, GetIncrementedSortingOrder());

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
            Debug.Assert(panelDict.ContainsKey(panelId));

            UIPanel panel = panelDict[panelId];
            panelDict.Remove(panelId);
            panel.Close(onFinish);

            UIManager.Instance.SetBackgroundAndFocus();
        }

        public void ClosePanel<T>(Action onFinish = null)
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

        public void DestroyPanel<T>()
        {
            DestroyPanel(typeof(T).Name);
        }

        public UIPanel GetPanel(string panelId)
        {
            return panelDict[panelId];
        }

        public UIPanel GetPanel<T>() where T : UIPanel
        {
            return GetPanel(typeof(T).Name);
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
            {
                return sortingOrder > panel.canvas.sortingOrder;
            });
            return panels.Count;
        }
    }
}