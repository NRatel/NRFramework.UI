using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRFramework
{
    public partial class UIManager : Singleton<UIManager>
    {
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

        /// <summary>
        /// 创建一个UI层
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="startOrder"></param>
        /// <param name="endOrder"></param>
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

        /// <summary>
        /// 创建一个UI面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelId">panelId</param>
        /// <param name="prefabPath">预设路径</param>
        /// <param name="layerId">在哪个层打开</param>
        /// <param name="fixedOrder">指定sortingOrder，正为指定（浮动）；负为自动自增。</param>
        /// <returns></returns>
        public T CreatePanel<T>(string panelId, string prefabPath, string layerId, int fixedOrder) where T : UIPanel
        {
            T panel = Activator.CreateInstance(typeof(T)) as T;
            panel.Create(panelId, m_UICanvas, prefabPath);

            Debug.Assert(m_LayerDict.ContainsKey(layerId), "layer不存在");

            UILayer layer = m_LayerDict[layerId];
            layer.AddPanel(panel, fixedOrder);
            m_AllPanelDict.Add(panel.panelId, panel);
            m_PanelToLayerMap.Add(panel.panelId, layerId);

            SortAllPanels();
            return panel;
        }

        /// <summary>
        /// 创建一个UI面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelId">panelId</param>
        /// <param name="panelBehaviour"></param>
        /// <param name="layerId">在哪个层打开</param>
        /// <param name="fixedOrder">指定sortingOrder，正为指定（浮动）；负为自动自增。</param>
        /// <returns></returns>
        public T CreatePanel<T>(string panelId, UIPanelBehaviour panelBehaviour, string layerId, int fixedOrder) where T : UIPanel
        {
            T panel = Activator.CreateInstance(typeof(T)) as T;
            panel.Create(panelId, m_UICanvas, panelBehaviour);

            Debug.Assert(m_LayerDict.ContainsKey(layerId), "layer不存在");

            UILayer layer = m_LayerDict[layerId];
            layer.AddPanel(panel, fixedOrder);
            m_AllPanelDict.Add(panel.panelId, panel);
            m_PanelToLayerMap.Add(panel.panelId, layerId);

            SortAllPanels();
            return panel;
        }

        public void DestoryPanel<T>(string panelId)
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