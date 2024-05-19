using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTimeForInfo : MonoBehaviour
{
    public void StopTime()
    {
        GameManager.instance.StopTime();
    }

    public void ContinueTime()
    {
        GameManager.instance.ContinueTime();
    }

}
