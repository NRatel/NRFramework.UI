
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHomeBase : UIPanel
{
	protected TMP_Dropdown m_Dropdown_TMPDropdown;
	protected TextMeshProUGUI m_TextTMP_TextMeshProUGUI;
	protected TMP_InputField m_InputFieldTMP_TMPInputField;

    protected override void OnBindCompsAndEvents() 
    {
		m_Dropdown_TMPDropdown = (TMP_Dropdown)viewBehaviour.GetComponentByIndexs(0, 0);
		m_TextTMP_TextMeshProUGUI = (TextMeshProUGUI)viewBehaviour.GetComponentByIndexs(1, 0);
		m_InputFieldTMP_TMPInputField = (TMP_InputField)viewBehaviour.GetComponentByIndexs(2, 0);
	}

    protected override void OnUnbindCompsAndEvents() 
    {

		m_Dropdown_TMPDropdown = null;
		m_TextTMP_TextMeshProUGUI = null;
		m_InputFieldTMP_TMPInputField = null;
	}
}