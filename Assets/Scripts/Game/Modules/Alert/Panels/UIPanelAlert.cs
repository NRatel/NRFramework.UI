using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelAlert : UIPanelAlertBase
{
    private Action m_OnOK;
    private Action m_OnCancel;

    protected override void OnCreating()
    {
        Debug.Log("UIPanelAlert OnCreating");

        CreateWidget<CommonAlertTitle>(m_CommonAlertTitle_UIWidgetBehaviour);
    }

    protected override void OnCreated()
    {
        Debug.Log("UIPanelAlert OnCreated");
    }

    public void Init(string title, string content, Action onOK = null, Action onCancel = null)
    {
        GetWidget<CommonAlertTitle>().Init(title);
        m_Content_TMPText.text = content;
        m_OnOK = onOK;
        m_OnCancel = onCancel;
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_BtnOK_Button)
        {
            CloseSelf();
            m_OnOK?.Invoke();
        }

        else if (button == m_BtnCancel_Button)
        {
            CloseSelf();
            m_OnCancel?.Invoke();
        }
    }

    protected override void OnFocusChanged(bool got)
    {
        Debug.Log("UIPanelAlert OnFocusChanged：" + got);
    }

    protected override void OnDestroying()
    {
        Debug.Log("UIPanelAlert OnDestroying");
    }

    protected override void OnDestroyed()
    {
        Debug.Log("UIPanelAlert OnDestroyed");
    }
}