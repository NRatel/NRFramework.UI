
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelAlertBase : UIPanel
{	protected Image m_Panel_Image;	protected UIWidgetBehaviour m_CommonAlertTitle_UIWidgetBehaviour;	protected TextMeshProUGUI m_Content_TMPText;	protected Button m_BtnOK_Button;	protected Button m_BtnCancel_Button;
    protected override void OnBindCompsAndEvents() 
    {		m_Panel_Image = (Image)viewBehaviour.GetComponentByIndexs(0, 0);		m_CommonAlertTitle_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComponentByIndexs(1, 0);		m_Content_TMPText = (TextMeshProUGUI)viewBehaviour.GetComponentByIndexs(2, 0);		m_BtnOK_Button = (Button)viewBehaviour.GetComponentByIndexs(3, 0);		m_BtnCancel_Button = (Button)viewBehaviour.GetComponentByIndexs(4, 0);		BindEvent(m_BtnOK_Button);		BindEvent(m_BtnCancel_Button);	}

    protected override void OnUnbindCompsAndEvents() 
    {		UnbindEvent(m_BtnOK_Button);		UnbindEvent(m_BtnCancel_Button);		m_Panel_Image = null;		m_CommonAlertTitle_UIWidgetBehaviour = null;		m_Content_TMPText = null;		m_BtnOK_Button = null;		m_BtnCancel_Button = null;	}
}