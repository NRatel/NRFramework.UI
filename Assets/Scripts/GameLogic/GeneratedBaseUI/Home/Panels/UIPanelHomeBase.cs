
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHomeBase : UIPanel
{
	protected RectTransform m_InputFieldTMP_RectTransform;
	protected RectTransform m_Placeholder_RectTransform;
	protected RectTransform m_TextArea_RectTransform;
	protected RectTransform m_Desc_RectTransform;
	protected RectTransform m_Button_RectTransform;
	protected Button m_Button_Button;
	protected RectTransform m_Text_RectTransform;
    protected override void OnBindCompsAndEvents() 
    {
		m_InputFieldTMP_RectTransform = (RectTransform)viewBehaviour.GetComponentByIndexs(0, 0);
		m_Placeholder_RectTransform = (RectTransform)viewBehaviour.GetComponentByIndexs(1, 0);
		m_TextArea_RectTransform = (RectTransform)viewBehaviour.GetComponentByIndexs(2, 0);
		m_Desc_RectTransform = (RectTransform)viewBehaviour.GetComponentByIndexs(3, 0);
		m_Button_RectTransform = (RectTransform)viewBehaviour.GetComponentByIndexs(4, 0);
		m_Button_Button = (Button)viewBehaviour.GetComponentByIndexs(4, 1);
		m_Text_RectTransform = (RectTransform)viewBehaviour.GetComponentByIndexs(5, 0);

		BindEvent(m_Button_Button);
	}

    protected override void OnUnbindCompsAndEvents() 
    {
		UnbindEvent(m_Button_Button);

		m_InputFieldTMP_RectTransform = null;
		m_Placeholder_RectTransform = null;
		m_TextArea_RectTransform = null;
		m_Desc_RectTransform = null;
		m_Button_RectTransform = null;
		m_Button_Button = null;
		m_Text_RectTransform = null;
	}
}