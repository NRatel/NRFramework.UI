
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHomeBase : UIPanel
{
	protected TextMeshProUGUI m_desc_TextMeshProUGUI;
	protected Image m_bg_Image;
	protected Button m_btnAddIcon_Button;
	protected Image m_btnAddIcon_Image;
	protected RectTransform m_iconRoot_RectTransform;

    protected override void OnBindCompsAndEvents() 
    {
		m_desc_TextMeshProUGUI = (TextMeshProUGUI)viewBehaviour.GetComponentByIndexs(0, 0);
		m_bg_Image = (Image)viewBehaviour.GetComponentByIndexs(1, 0);
		m_btnAddIcon_Button = (Button)viewBehaviour.GetComponentByIndexs(2, 0);
		m_btnAddIcon_Image = (Image)viewBehaviour.GetComponentByIndexs(2, 1);
		m_iconRoot_RectTransform = (RectTransform)viewBehaviour.GetComponentByIndexs(3, 0);

		BindEvent(m_btnAddIcon_Button);
	}

    protected override void OnUnbindCompsAndEvents() 
    {
		UnbindEvent(m_btnAddIcon_Button);

		m_desc_TextMeshProUGUI = null;
		m_bg_Image = null;
		m_btnAddIcon_Button = null;
		m_btnAddIcon_Image = null;
		m_iconRoot_RectTransform = null;
	}
}