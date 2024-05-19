using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] Animator anim;

    public bool isOpen;

    private void Start()
    {
        if (isOpen)
        {
            anim.Play("DoorOpen");
        }
        else
        {
            anim.Play("DoorClosed");
        }

    }

    public void OpenDoor()
    {

        isOpen = true;
        anim.Play("DoorOpening");
        
    }
    


}
