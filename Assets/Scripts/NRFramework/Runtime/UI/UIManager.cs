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

        private List<UIPanel> m_FocusingPanels;     //关闭界面时无需清理，由ChangeFocus覆盖。
        private List<UIPanel> m_TempNewFocusingPanels;

        private UIManager()
        {
            uiCanvas = GameObject.Find(NRFrameworkSetting.kUICanvasPath).GetComponent<Canvas>();
            uiCamera = GameObject.Find(NRFrameworkSetting.kUICameraPath).GetComponent<Camera>();
            rootDict = new Dictionary<string, UIRoot>();
            m_FocusingPanels = new List<UIPanel>();
            m_TempNewFocusingPanels = new List<UIPanel>();
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

        public List<UIPanel> GetPanels(Func<UIPanel, bool> filterFunc)
        {
            List<UIPanel> panels = new List<UIPanel>();

            foreach (KeyValuePair<string, UIRoot> kvPair in rootDict)
            {
                foreach (KeyValuePair<string, UIPanel> kvPair2 in kvPair.Value.panelDict)
                {
                    if (filterFunc(kvPair2.Value))
                    {
                        panels.Add(kvPair2.Value);
                    }
                }
            }
            return panels;
        }

        public List<UIPanel> GetFocusingPanels()
        {
            List<UIPanel> vaildPanels = new List<UIPanel>();
            foreach (UIPanel panel in m_FocusingPanels)
            {
                if (panel != null) { vaildPanels.Add(panel); }
            }
            return vaildPanels;
        }

        internal void ChangeFocus()
        {
            List<UIPanel> panels = GetPanels((panel) => { return true; });
            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            m_TempNewFocusingPanels.Clear();

            for (int i = panels.Count - 1; i >= 0; i--)
            {
                UIPanel panel = panels[i];
                if (panel.panelBehaviour.panelType == UIPanelType.Overlap)
                {
                    m_TempNewFocusingPanels.Add(panel); continue;
                }
                else
                {
                    m_TempNewFocusingPanels.Add(panel); break;
                }
            }

            //丢失焦点时，由顶至下
            for (int i = m_FocusingPanels.Count - 1; i >= 0; i--)
            {
                UIPanel panel = m_FocusingPanels[i];
                if (panel.panelId != null && !m_TempNewFocusingPanels.Contains(panel))
                {
                    panel.ChangeFocus(false);
                }
            }

            //获得焦点时，由底至上
            for (int i = 0; i < m_TempNewFocusingPanels.Count; i++)
            {
                UIPanel panel = m_TempNewFocusingPanels[i];
                if (m_FocusingPanels.Contains(panel))
                {
                    panel.ChangeFocus(true);
                }
            }

            List<UIPanel> t = m_FocusingPanels;
            m_FocusingPanels = m_TempNewFocusingPanels;
            m_TempNewFocusingPanels = t;
            m_FocusingPanels.Clear();
            t = null;
        }
    }
} 