
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHomeBase : UIPanel
{	protected TMP_Dropdown m_Dropdown_TMPDropdown;	protected TMP_InputField m_InputFieldTMP_TMPInputField;	protected TextMeshProUGUI m_Desc_TextMeshProUGUI;	protected Image m_Bg_Image;	protected Button m_BtnAddIcon_Button;
    protected override void OnBindCompsAndEvents() 
    {		m_Dropdown_TMPDropdown = (TMP_Dropdown)viewBehaviour.GetComponentByIndexs(0, 0);		m_InputFieldTMP_TMPInputField = (TMP_InputField)viewBehaviour.GetComponentByIndexs(1, 0);		m_Desc_TextMeshProUGUI = (TextMeshProUGUI)viewBehaviour.GetComponentByIndexs(2, 0);		m_Bg_Image = (Image)viewBehaviour.GetComponentByIndexs(3, 0);		m_BtnAddIcon_Button = (Button)viewBehaviour.GetComponentByIndexs(4, 0);		BindEvent(m_Dropdown_TMPDropdown);		BindEvent(m_InputFieldTMP_TMPInputField);		BindEvent(m_BtnAddIcon_Button);	}

    protected override void OnUnbindCompsAndEvents() 
    {		UnbindEvent(m_Dropdown_TMPDropdown);		UnbindEvent(m_InputFieldTMP_TMPInputField);		UnbindEvent(m_BtnAddIcon_Button);		m_Dropdown_TMPDropdown = null;		m_InputFieldTMP_TMPInputField = null;		m_Desc_TextMeshProUGUI = null;		m_Bg_Image = null;		m_BtnAddIcon_Button = null;	}
}