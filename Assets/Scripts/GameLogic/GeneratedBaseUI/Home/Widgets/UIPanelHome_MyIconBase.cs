
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHome_MyIconBase : UIWidget
{
	protected Button m_BtnCover_Button;
	protected Image m_Icon_Image;

    protected override void OnBindCompsAndEvents() 
    {
		m_BtnCover_Button = (Button)viewBehaviour.GetComponentByIndexs(0, 0);
		m_Icon_Image = (Image)viewBehaviour.GetComponentByIndexs(1, 0);

		BindEvent(m_BtnCover_Button);
	}

    protected override void OnUnbindCompsAndEvents() 
    {
		UnbindEvent(m_BtnCover_Button);

		m_BtnCover_Button = null;
		m_Icon_Image = null;
	}
}