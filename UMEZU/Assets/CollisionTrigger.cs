using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionTrigger : MonoBehaviour
{
    public UnityEvent onCollide;
    bool isComplete;

    private void OnTriggerEnter(Collider other)
    {
        if (!isComplete)
        {
            if (other.CompareTag("Player"))
            {
                onCollide.Invoke();
                isComplete = true;
            }
        }
    }

}
