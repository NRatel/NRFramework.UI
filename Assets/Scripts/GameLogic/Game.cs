using NRFramework;
using System.Collections;
using UnityEngine;

public class Game
{
    public UIRoot bottomUIRoot { get; private set; }
    public UIRoot normalUIRoot { get; private set; }
    public UIRoot topUIRoot { get; private set; }
    public UIRoot guideUIRoot { get; private set; }

    public Game()
    {
        bottomUIRoot = UIManager.Instance.CreateUIRoot("bottom", 0, 999);
        normalUIRoot = UIManager.Instance.CreateUIRoot("normal", 1000, 1999);
        topUIRoot = UIManager.Instance.CreateUIRoot("top", 2000, 2999);
        guideUIRoot = UIManager.Instance.CreateUIRoot("guide", 3000, 3999);
    }

    public void StartGame()
    {
        UIPanelHome uiPanelHome = normalUIRoot.CreatePanel<UIPanelHome>("Assets/GameRes/GUI/Prefabs/Home/Panels/UIPanelHome.prefab");
        uiPanelHome.Init();
    }
}