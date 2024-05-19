using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlimeTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Color slimeColor;
    [SerializeField] Color almostDyingColor;

    bool lowHealth = false;


    #region Singleton

    public static SlimeTimer instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
    }

    #endregion

    public void SetTimer(float time)
    {
        int timeInt = (int)time;
        timerText.text = timeInt.ToString();

        if (timeInt <= 10)
        {
            timerText.color = almostDyingColor;
        }
        else
        {
            timerText.color = slimeColor;
        }
    }

}
