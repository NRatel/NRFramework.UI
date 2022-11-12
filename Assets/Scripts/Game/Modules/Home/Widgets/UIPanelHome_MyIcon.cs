using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHome_MyIcon : UIPanelHome_MyIconBase
{
    protected override void OnCreating() { }

    protected override void OnCreated() { }

    public void Init(int count)
    {
        Debug.Log("UIPanelHome_MyIcon Custom Init " + count);

        m_Index_TextMeshProUGUI.text = "Index: " + count;
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_BtnCover_Button) 
        {
            CloseSelf();
        }
    }

    protected override void OnValueChanged(Toggle toggle, bool value) { }

    protected override void OnValueChanged(Dropdown dropdown, int value) { }

    protected override void OnValueChanged(TMP_Dropdown tmpDropdown, int value) { }

    protected override void OnValueChanged(InputField inputField, string value) { }

    protected override void OnValueChanged(TMP_InputField tmpInputField, string value) { }

    protected override void OnValueChanged(Slider slider, float value) { }

    protected override void OnValueChanged(Scrollbar scrollbar, float value) { }

    protected override void OnValueChanged(ScrollRect scrollRect, Vector2 value) { }
    
    protected override void OnClosing() { }

    protected override void OnClosed() { }
}