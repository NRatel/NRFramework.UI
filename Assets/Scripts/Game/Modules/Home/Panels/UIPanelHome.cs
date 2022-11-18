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
        m_Desc_TMPText.text = "Hello NRatel!";
        m_IconCount = 0;
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_BtnAddIcon_Button)
        {
            string widgetId = "MyIcon" + m_IconCount;
            UIPanelHome_MyIcon myIcon = CreateWidget<UIPanelHome_MyIcon>(widgetId, m_IconRoot_RectTransform, "Assets/GameRes/GUI/Prefabs/Home/Widgets/UIPanelHome_MyIcon.prefab");
            myIcon.Init(m_IconCount);
            myIcon.rectTransform.anchoredPosition = new Vector2(UnityEngine.Random.Range(0, 200), UnityEngine.Random.Range(0, 200));
            m_IconCount += 1;
        }

        else if (button == m_BtnTestFindComp_Button)
        {
            UIPanelAlert panelAlert = Game.Instance.topRoot.CreatePanel<UIPanelAlert>("Assets/GameRes/GUI/Prefabs/Alert/Panels/UIPanelAlert.prefab");

            string title = "Alert";
            string content = "please confirm the \"m_Index_TMPText\" exist in \"normalRoot/UIPanelHome/MyIcon0\".";
            panelAlert.Init(title, content, () =>
            {
                int retCode = UIManager.Instance.FindComponentByPath("normalRoot/UIPanelHome/MyIcon0", "m_Index_TMPText", out TextMeshProUGUI indexText);
                Debug.Log(string.Format("retCode, result：{0}, {1}", retCode, indexText));
            }, null);
        }
    }

    protected override void OnFocusChanged(bool got)
    {
        Debug.Log("UIPanelHome OnFocusChanged：" + got);
    }

    protected override void OnDestroying()
    {
        Debug.Log("UIPanelHome OnDestroying");
    }

    protected override void OnDestroyed()
    {
        Debug.Log("UIPanelHome OnDestroyed");
    }
}