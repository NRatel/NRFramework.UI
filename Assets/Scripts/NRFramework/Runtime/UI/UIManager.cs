// https://github.com/NRatel/NRFramework.UI

using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "# UIManager #")]
    public class UIManager : MonoSingleton<UIManager>
    {
        public Canvas uiCanvas { private set; get; }

        public Camera uiCamera { private set; get; }

        public Dictionary<string, UIRoot> rootDict { private set; get; }

        private List<UIPanel> m_FocusingPanels;
        private List<UIPanel> m_TempNewFocusingPanels;

        private void Awake()
        {
            uiCanvas = GameObject.Find(NRFrameworkSetting.kUICanvasPath).GetComponent<Canvas>();
            uiCamera = GameObject.Find(NRFrameworkSetting.kUICameraPath).GetComponent<Camera>();
            rootDict = new Dictionary<string, UIRoot>();

            m_FocusingPanels = new List<UIPanel>();
            m_TempNewFocusingPanels = new List<UIPanel>();
        }

        public UIRoot CreateUIRoot(string rootId, int startOrder, int endOrder)
        {
            Debug.Assert(!rootDict.ContainsKey(rootId));    //uiRoot已存在
            Debug.Assert(startOrder >= 0);                  //必须使startOrder >= 0
            Debug.Assert(endOrder >= startOrder);           //必须使endOrder >= startOrder

            UIRoot uiRoot = new UIRoot() { rootId = rootId, startOrder = startOrder, endOrder = endOrder };
            rootDict.Add(rootId, uiRoot);

            return uiRoot;
        }

        public UIRoot GetUIRoot(string rootId)
        {
            return rootDict[rootId];
        }

        public List<UIPanel> FilterPanels(Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = new List<UIPanel>();

            foreach (KeyValuePair<string, UIRoot> kvPair in rootDict)
            {
                foreach (KeyValuePair<string, UIPanel> kvPair2 in kvPair.Value.panelDict)
                {
                    if (filterFunc == null || filterFunc(kvPair2.Value))
                    {
                        panels.Add(kvPair2.Value);
                    }
                }
            }
            return panels;
        }

        public UIPanel FilterTopestPanel(Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(filterFunc);

            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            return panels.Count > 0 ? panels[panels.Count - 1] : null;
        }

        public UIPanel GetTopestPanel()
        {
            return FilterTopestPanel((panel)=> { return panel.showState != UIPanelShowState.Hidden; });
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

        public T FindPanelComponent<T>(string rootId, string panelId, string compDefine) where T : Component
        {
            UIRoot root = GetUIRoot(rootId);
            return root.FindPanelComponent<T>(panelId, compDefine);
        }

        public T FindPanelComponent<T>(string path, string compDefine) where T : Component
        {
            string[] strs = path.Split("/");
            string rootId = strs[0];
            string panelId = strs[1];

            return FindPanelComponent<T>(rootId, panelId, compDefine);
        }

        public T FindWidgetComponent<T>(string rootId, string panelId, string[] widgetIds, string compDefine) where T : Component
        {
            UIRoot root = GetUIRoot(rootId);
            return root.FindWidgetComponent<T>(panelId, widgetIds, compDefine);
        }

        public T FindWidgetComponent<T>(string path, string compDefine) where T : Component
        {
            string[] strs = path.Split("/");
            string rootId = strs[0];
            string panelId = strs[1];
            string[] widgetIds = new string[strs.Length - 2];
            for (int i = 0; i < strs.Length - 2; i++)
            { widgetIds[i] = strs[i + 2]; }

            return FindWidgetComponent<T>(rootId, panelId, widgetIds, compDefine);
        }

        internal void SetBackgroundAndFocus()
        {
            List<UIPanel> panels = FilterPanels((panel) =>
            { return panel.showState != UIPanelShowState.Hidden; });
            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            UIPanel needBgPanel = null;
            bool collectFocusCanBreak = false;

            for (int i = panels.Count - 1; i >= 0; i--)
            {
                UIPanel panel = panels[i];

                if (needBgPanel == null && panel.panelBehaviour.hasBg)
                {
                    needBgPanel = panel;
                }

                if (panel.panelBehaviour.getFocusType == UIPanelGetFocusType.GetWithOthers)
                {
                    m_TempNewFocusingPanels.Add(panel);
                }
                else if (panel.panelBehaviour.getFocusType == UIPanelGetFocusType.Get)
                {
                    m_TempNewFocusingPanels.Add(panel);
                    collectFocusCanBreak = true;
                }

                if (needBgPanel != null && collectFocusCanBreak) { break; }
            }

            //设置/移除背景
            if (needBgPanel != null) { needBgPanel.SetBackground(); }
            else { UIBlocker.Instance.Unbind(); }

            //丢失焦点时，由顶至下
            for (int i = m_FocusingPanels.Count - 1; i >= 0; i--)
            {
                UIPanel panel = m_FocusingPanels[i];
                if (panel.panelId != null && !m_TempNewFocusingPanels.Contains(panel))
                {
                    panel.SetFocus(false);
                }
            }

            //获得焦点时，由底至上
            for (int i = 0; i < m_TempNewFocusingPanels.Count; i++)
            {
                UIPanel panel = m_TempNewFocusingPanels[i];
                if (!m_FocusingPanels.Contains(panel))
                {
                    panel.SetFocus(true);
                }
            }

            List<UIPanel> t = m_FocusingPanels;
            m_FocusingPanels = m_TempNewFocusingPanels;
            m_TempNewFocusingPanels = t;
            m_TempNewFocusingPanels.Clear();
            t = null;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                UIPanel topestPanel = FilterTopestPanel((panel) => 
                { return panel.showState != UIPanelShowState.Hidden && panel.panelBehaviour.escPressEventType != UIPanelEscPressEventType.DontCheck; });

                if (topestPanel == null) { return; }
                if (topestPanel.showState != UIPanelShowState.Idle) { return; }

                topestPanel.DoEscPress();
            }
        }
    }
}