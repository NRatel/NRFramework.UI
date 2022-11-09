using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class UIPanelHome : UIPanelHomeBase
{
    private int m_IconCount;

    protected override void OnCreating() 
    {
        Debug.Log("UIPanelHome OnCreating");
    }

    protected override void OnCreated() 
    {
        Debug.Log("UIPanelHome OnCreated");
    }

    public void Init()
    {
        Debug.Log("UIPanelHome Custom Init");

        m_desc_TextMeshProUGUI.text = "Hello NRatel!";
        m_IconCount = 0;
    }

    protected override void OnClicked(Button button) 
    {
        Debug.Log("UIPanelHome OnClicked" + button);

        string widgetId = "MyIcon" + m_IconCount;
        UIPanelHome_MyIcon myIcon = CreateWidget<UIPanelHome_MyIcon>(widgetId, m_iconRoot_RectTransform, "Assets/GameRes/GUI/Prefabs/Home/Widgets/UIPanelHome_MyIcon.prefab");
        myIcon.Init(m_IconCount);
        myIcon.rectTransform.anchoredPosition = new Vector2(UnityEngine.Random.Range(0, 200), UnityEngine.Random.Range(0, 200));
        m_IconCount += 1;
    }
    
    protected override void OnFocusChanged(bool got) 
    {
        Debug.Log("UIPanelHome OnFocusChanged" + got);
    }
    //protected override void OnWindowBgClicked() { }
    
    //protected override void OnEscButtonClicked() { }

    protected override void OnClosing() 
    {
        Debug.Log("UIPanelHome OnClosing");
    }

    protected override void OnClosed()
    {
        Debug.Log("UIPanelHome OnClosed");
    }
}