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
            int targetSortingOrder = GetTargetSortingOrder();
            panel.SetSortingOrder(targetSortingOrder);
            panel.rectTransform.SetSiblingIndex(GetTargetSiblingIndex(targetSortingOrder));
            panelDict.Add(panel.panelId, panel);
            UIManager.Instance.ChangeFocus();

            return panel;
        }

        public T CreatePanel<T>(string panelId, UIPanelBehaviour panelBehaviour) where T : UIPanel
        {
            Debug.Assert(!panelDict.ContainsKey(panelId), "panel已存在");

            T panel = Activator.CreateInstance(typeof(T)) as T;
            panel.Create(panelId, this, panelBehaviour);
            int targetSortingOrder = GetTargetSortingOrder();
            panel.SetSortingOrder(targetSortingOrder);
            panel.rectTransform.SetSiblingIndex(GetTargetSiblingIndex(targetSortingOrder));
            panelDict.Add(panel.panelId, panel);
            UIManager.Instance.ChangeFocus();

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
            Debug.Assert(panelDict.ContainsKey(panelId), "panel不存在");

            UIPanel panel = panelDict[panelId];
            panelDict.Remove(panelId);
            panel.Close(onFinish);

            UIManager.Instance.ChangeFocus();
        }

        public void DestroyPanel(string panelId)
        {
            Debug.Assert(panelDict.ContainsKey(panelId), "panel不存在");

            UIPanel panel = panelDict[panelId];
            panelDict.Remove(panelId);
            panel.Destroy();

            UIManager.Instance.ChangeFocus();
        }

        public void ClosePanel<T>(Action onFinish = null)
        {
            ClosePanel(typeof(T).Name, onFinish);
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

        private int GetTargetSortingOrder()
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

            int targetOrder = topestPanel != null ? (topestPanel.canvas.sortingOrder + topestPanel.panelBehaviour.thickness + 1) : startOrder;

            Debug.Assert(targetOrder <= endOrder, "targetOrder 越界了");

            return targetOrder;
        }

        private int GetTargetSiblingIndex(int sortingOrder)
        {
            List<UIPanel> panels = UIManager.Instance.GetPanels((panel) => 
            {
                return sortingOrder > panel.canvas.sortingOrder;
            });
            return panels.Count;
        }
    }
}