using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    public partial class UIManager : Singleton<UIManager>
    {
        private class UILayer
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

        private Canvas m_UICanvas;
        private Camera m_UICamera;
        private Dictionary<string, UILayer> m_LayerDict;
        public Dictionary<string, UIPanel> m_AllPanelDict;
        private Dictionary<string, string> m_PanelToLayerMap; //panel与其所在Layer的映射。

        private UIManager()
        {
            m_UICanvas = GameObject.Find(NRFrameworkSetting.kUICanvasPath).GetComponent<Canvas>();
            m_UICamera = GameObject.Find(NRFrameworkSetting.kUICameraPath).GetComponent<Camera>();
            m_LayerDict = new Dictionary<string, UILayer>();
            m_AllPanelDict = new Dictionary<string, UIPanel>();
            m_PanelToLayerMap = new Dictionary<string, string>();
        }

        public void CreateLayer(string layerId, int startOrder, int endOrder)
        {
            Debug.Assert(!m_LayerDict.ContainsKey(layerId), "layer已存在");
            Debug.Assert(startOrder >= 0, "必须使startOrder >= 0");
            Debug.Assert(endOrder >= startOrder, "必须使endOrder >= startOrder");
            
            foreach (UILayer layer in m_LayerDict.Values)
            {
                Debug.Assert(startOrder > layer.endOrder || endOrder < layer.startOrder, "各layer的sortingOrder范围不允许交叉");
            }

            m_LayerDict.Add(layerId, new UILayer()
            {
                layerId = layerId,
                startOrder = startOrder,
                endOrder = endOrder
            });
        }

        public T OpenPanel<T>(string panelId, string prefabPath, UIPanelOpenSetting setting, UIContext context = null) where T : UIPanel
        {
            T panel = Activator.CreateInstance(typeof(T), panelId) as T;
            panel.Init(m_UICanvas, prefabPath, context);
            SetAndCache(panel, setting);
            SortAllPanels();
            return panel;
        }

        public void ClosePanel<T>(string panelId)
        {

        }

        public UIPanel GetTopPanel()
        {
            UIPanel topPanel = null;
            foreach (UILayer layer in m_LayerDict.Values)
            {
                UIPanel topPanelInLayer = layer.GetTopPanel();
                if (topPanelInLayer != null && (topPanel == null || topPanelInLayer.canvas.sortingOrder > topPanel.canvas.sortingOrder))
                {
                    topPanel = topPanelInLayer;
                }
            }
            return topPanel;
        }

        private void SetAndCache(UIPanel panel, UIPanelOpenSetting setting)
        {
            //Debug.Log("setting.layerId: " + setting.layerId);
            Debug.Assert(m_LayerDict.ContainsKey(setting.layerId), "layer不存在");

            UILayer layer = m_LayerDict[setting.layerId];

            layer.AddPanel(panel, setting.fixedOrder);
            m_AllPanelDict.Add(panel.panelId, panel);
            m_PanelToLayerMap.Add(panel.panelId, setting.layerId);
        }

        //调整所有Panel的 sblingIndex。仅编辑器下？
        private void SortAllPanels()
        {
#if UNITY_EDITOR
            List<UIPanel> panels = new List<UIPanel>();
            foreach (UIPanel panel in m_AllPanelDict.Values)
            {
                panels.Add(panel);
            }
            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });
            foreach (UIPanel panel in panels)
            {
                panel.rectTransform.SetAsFirstSibling();
            }
#endif
        }
    }

    public partial class UIPanelOpenSetting
    {
        public string layerId;          //所在层
        public int fixedOrder;          //指定sortingOrder，正指定；负自增。
    }
} 