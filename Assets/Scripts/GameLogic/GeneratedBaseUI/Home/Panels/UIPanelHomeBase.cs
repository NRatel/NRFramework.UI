// 导出测试
// savePath: F:\Github\NRFramework\Assets\Scripts\GameLogic\GeneratedBaseUI\Home\Panels\UIPanelHomeBase.cs

using UnityEngine;
using UnityEngine.UI;
using NRFramework;

public class UIPanelHomeBase : UIPanel
{
    public GameObject m_XXX_Go;
    public Button m_XXX_Btn;

    protected override void OnBindCompsAndEvents() 
    {
        m_XXX_Go = panelBehaviour.GetGameObjectByIndex(0);
        m_XXX_Btn = (Button)panelBehaviour.GetComponentByIndexs(1, 2);
    }

    protected override void OnUnbindCompsAndEvents() 
    {
        m_XXX_Btn.onClick.RemoveAllListeners();

        m_XXX_Go = null;
        m_XXX_Btn = null;
    }
}
