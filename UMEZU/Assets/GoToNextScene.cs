using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNextScene : MonoBehaviour
{
    [SerializeField] string nextSceneName;
    bool isComplete = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerenter");
        Debug.Log(other.name);
        if (other.CompareTag("Player") && !isComplete)
        {
            FinishLevel();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            FinishLevel();
        }
    }

    private void FinishLevel()
    {
        Debug.Log("isplayer");
        GameManager.instance.LoadNextLevel(nextSceneName);
        isComplete = true;
    }
}
