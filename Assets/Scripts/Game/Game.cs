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
        bottomRoot = UIManager.Instance.CreateUIRoot("bottomRoot", 0, 999);
        normalRoot = UIManager.Instance.CreateUIRoot("normalRoot", 1000, 1999);
        topRoot = UIManager.Instance.CreateUIRoot("topRoot", 2000, 2999);
        guideRoot = UIManager.Instance.CreateUIRoot("guideRoot", 3000, 3999);
    }

    public void StartGame()
    {
        UIPanelHome uiPanelHome = normalRoot.CreatePanel<UIPanelHome>("Assets/GameRes/GUI/Prefabs/Home/Panels/UIPanelHome.prefab");
        uiPanelHome.Init();

        List<UIPanel> focusingPanels = UIManager.Instance.GetFocusingPanels();
        Debug.Log("focusingPanels.Count: " + focusingPanels.Count);
    }
}