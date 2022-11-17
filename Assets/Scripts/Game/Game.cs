using NRFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game: Singleton<Game>
{
    public UIRoot bottomRoot { get; private set; }
    public UIRoot normalRoot { get; private set; }
    public UIRoot topRoot { get; private set; }
    public UIRoot guideRoot { get; private set; }

    private Game()
    {
        bottomRoot = UIManager.Instance.CreateRoot("bottomRoot", 0, 999);
        normalRoot = UIManager.Instance.CreateRoot("normalRoot", 1000, 1999);
        topRoot = UIManager.Instance.CreateRoot("topRoot", 2000, 2999);
        guideRoot = UIManager.Instance.CreateRoot("guideRoot", 3000, 3999);
    }

    public void StartGame()
    {
        UIPanelHome panelHome = normalRoot.CreatePanel<UIPanelHome>("Assets/GameRes/GUI/Prefabs/Home/Panels/UIPanelHome.prefab");
        panelHome.Init();
    }
}