﻿using NRFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Game
{
    public UIRoot bottomRoot { get; private set; }
    public UIRoot normalRoot { get; private set; }
    public UIRoot topRoot { get; private set; }
    public UIRoot guideRoot { get; private set; }

    public Game()
    {
        bottomRoot = UIManager.Instance.CreateUIRoot("bottomRoot", 0, 999);
        normalRoot = UIManager.Instance.CreateUIRoot("normalRoot", 1000, 1999);
        topRoot = UIManager.Instance.CreateUIRoot("topRoot", 2000, 2999);
        guideRoot = UIManager.Instance.CreateUIRoot("guideRoot", 3000, 3999);
    }

    public void StartGame()
    {
        UIPanelHome uiPanelHome = normalRoot.CreatePanel<UIPanelHome>("Assets/GameRes/GUI/Prefabs/Home/Panels/UIPanelHome.prefab");
        uiPanelHome.Init();
    }
}