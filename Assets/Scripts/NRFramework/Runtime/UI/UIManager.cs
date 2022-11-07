using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    public class UIManager : Singleton<UIManager>
    {
        public Canvas uiCanvas { private set; get; }

        public Camera uiCamera { private set; get; }

        public Dictionary<string, UIRoot> rootDict { private set; get; }

        private UIManager()
        {
            uiCanvas = GameObject.Find(NRFrameworkSetting.kUICanvasPath).GetComponent<Canvas>();
            uiCamera = GameObject.Find(NRFrameworkSetting.kUICameraPath).GetComponent<Camera>();
            rootDict = new Dictionary<string, UIRoot>();
        }

        public void CreateUIRoot(string rootId, int startOrder, int endOrder)
        {
            Debug.Assert(!rootDict.ContainsKey(rootId), "uiRoot已存在");
            Debug.Assert(startOrder >= 0, "必须使startOrder >= 0");
            Debug.Assert(endOrder >= startOrder, "必须使endOrder >= startOrder");
            
            foreach (UIRoot uiRoot in rootDict.Values)
            {
                Debug.Assert(startOrder > uiRoot.endOrder || endOrder < uiRoot.startOrder, "sortingOrder范围不允许与其他uiRoot交叉");
            }

            rootDict.Add(rootId, new UIRoot()
            {
                rootId = rootId,
                startOrder = startOrder,
                endOrder = endOrder
            });
        }

        public bool ExistUIRoot(string rootId)
        {
            return rootDict.ContainsKey(rootId);
        }

        public UIPanel GetTopestPanel()
        {
            UIPanel topestPanel = null;
            foreach (UIRoot uiRoot in rootDict.Values)
            {
                UIPanel panel = uiRoot.GetTopestPanel();
                if (panel != null && (topestPanel == null || panel.canvas.sortingOrder > topestPanel.canvas.sortingOrder))
                {
                    topestPanel = panel;
                }
            }
            return topestPanel;
        }

        public void SortAllPanels()
        {
#if UNITY_EDITOR
            List<UIPanel> allPanels = new List<UIPanel>();
            foreach (UIRoot uiRoot in rootDict.Values)
            {
                foreach (UIPanel panel in uiRoot.panelDict.Values)
                {
                    allPanels.Add(panel);
                }
            }
            allPanels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });
            foreach (UIPanel panel in allPanels)
            {
                panel.rectTransform.SetAsFirstSibling();
            }
#endif
        }
    }
} 