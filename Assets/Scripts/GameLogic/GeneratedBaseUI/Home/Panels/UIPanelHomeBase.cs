
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHomeBase : UIPanel
{
	protected TextMeshProUGUI m_Desc_TextMeshProUGUI;
	protected Image m_Bg_Image;
	protected Button m_BtnAddIcon_Button;

    protected override void OnBindCompsAndEvents() 
    {
		m_Desc_TextMeshProUGUI = (TextMeshProUGUI)viewBehaviour.GetComponentByIndexs(0, 0);
		m_Bg_Image = (Image)viewBehaviour.GetComponentByIndexs(1, 0);
		m_BtnAddIcon_Button = (Button)viewBehaviour.GetComponentByIndexs(2, 0);

		BindEvent(m_BtnAddIcon_Button);
	}

    protected override void OnUnbindCompsAndEvents() 
    {
		UnbindEvent(m_BtnAddIcon_Button);

		m_Desc_TextMeshProUGUI = null;
		m_Bg_Image = null;
		m_BtnAddIcon_Button = null;
	}
}