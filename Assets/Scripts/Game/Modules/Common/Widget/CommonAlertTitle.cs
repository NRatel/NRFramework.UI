using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class CommonAlertTitle : CommonAlertTitleBase
{
    public void Init(string title) 
    {
        m_Title_TMPText.text = title;
    }
}