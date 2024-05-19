using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialObjectiveKeyboard : MonoBehaviour
{
    public UnityEvent onComplete;

    bool aPressed = false;
    bool wPressed = false;
    bool sPressed = false;
    bool dPressed = false;
    bool complete = false;

    [SerializeField] GameObject aWhite;
    [SerializeField] GameObject aGreen;

    [SerializeField] GameObject wWhite;
    [SerializeField] GameObject wGreen;

    [SerializeField] GameObject sWhite;
    [SerializeField] GameObject sGreen;

    [SerializeField] GameObject dWhite;
    [SerializeField] GameObject dGreen;

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            aPressed = true;
            aWhite.SetActive(false);
            aGreen.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            
            wPressed = true;
            wWhite.SetActive(false);
            wGreen.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            sPressed = true;
            sWhite.SetActive(false);
            sGreen.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            dPressed = true;
            dWhite.SetActive(false);
            dGreen.SetActive(true);
        }

        if (aPressed && wPressed && sPressed && dPressed && !complete)
        {
            onComplete.Invoke();
            complete = true;
        }

    }
}
