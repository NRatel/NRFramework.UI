
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHomeBase : UIPanel
{
	protected TextMeshProUGUI m_Desc_TMPText;
	protected Image m_Bg_Image;
	protected Button m_BtnAddIcon_Button;
	protected Image m_BtnAddIcon_Image;
	protected RectTransform m_IconRoot_RectTransform;
	protected Button m_BtnTestFindComp_Button;

    protected override void OnBindCompsAndEvents() 
    {
		m_Desc_TMPText = (TextMeshProUGUI)viewBehaviour.GetComponentByIndexs(0, 0);
		m_Bg_Image = (Image)viewBehaviour.GetComponentByIndexs(1, 0);
		m_BtnAddIcon_Button = (Button)viewBehaviour.GetComponentByIndexs(2, 0);
		m_BtnAddIcon_Image = (Image)viewBehaviour.GetComponentByIndexs(2, 1);
		m_IconRoot_RectTransform = (RectTransform)viewBehaviour.GetComponentByIndexs(3, 0);
		m_BtnTestFindComp_Button = (Button)viewBehaviour.GetComponentByIndexs(4, 0);

		BindEvent(m_BtnAddIcon_Button);
		BindEvent(m_BtnTestFindComp_Button);
	}

    protected override void OnUnbindCompsAndEvents() 
    {
		UnbindEvent(m_BtnAddIcon_Button);
		UnbindEvent(m_BtnTestFindComp_Button);

		m_Desc_TMPText = null;
		m_Bg_Image = null;
		m_BtnAddIcon_Button = null;
		m_BtnAddIcon_Image = null;
		m_IconRoot_RectTransform = null;
		m_BtnTestFindComp_Button = null;
	}
}