
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class CommonAlertTitleBase : UIWidget
{
	protected TextMeshProUGUI m_Title_TMPText;

    protected override void OnBindCompsAndEvents() 
    {
		m_Title_TMPText = (TextMeshProUGUI)viewBehaviour.GetComponentByIndexs(0, 0);

	}

    protected override void OnUnbindCompsAndEvents() 
    {
		m_Title_TMPText = null;
	}
}