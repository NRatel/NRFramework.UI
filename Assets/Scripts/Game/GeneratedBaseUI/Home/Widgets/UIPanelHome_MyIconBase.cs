
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHome_MyIconBase : UIWidget
{	protected Image m_Icon_Image;	protected Button m_BtnCover_Button;	protected TextMeshProUGUI m_Index_TMPText;
    protected override void OnBindCompsAndEvents() 
    {		m_Icon_Image = (Image)viewBehaviour.GetComponentByIndexs(0, 0);		m_BtnCover_Button = (Button)viewBehaviour.GetComponentByIndexs(1, 0);		m_Index_TMPText = (TextMeshProUGUI)viewBehaviour.GetComponentByIndexs(2, 0);		BindEvent(m_BtnCover_Button);	}

    protected override void OnUnbindCompsAndEvents() 
    {		UnbindEvent(m_BtnCover_Button);		m_Icon_Image = null;		m_BtnCover_Button = null;		m_Index_TMPText = null;	}
}