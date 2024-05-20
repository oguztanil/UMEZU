using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTimeInfoPanel : MonoBehaviour
{
    [SerializeField] GameObject infoPanel;

    public void StopForPanel()
    {
        GameManager.instance.StopTime();
        infoPanel.SetActive(true);
    }
    public void ContinueGame()
    {
        GameManager.instance.ContinueTime();
        infoPanel.SetActive(false);
    }
}
