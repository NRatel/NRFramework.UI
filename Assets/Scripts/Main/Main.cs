using NRFramework;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        Game.Instance.StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            List<UIPanel> focusingPanels = UIManager.Instance.GetFocusingPanels();
            string panelIdsStr = "";
            for (int i = 0; i < focusingPanels.Count; i++)
            {
                panelIdsStr = focusingPanels[i].panelId + (i < focusingPanels.Count - 1 ? "," : "");
            }
            Debug.Log(panelIdsStr);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            UIPanel topestPanel = UIManager.Instance.GetTopestPanel();
            Debug.Log(topestPanel);
        }
    }
}