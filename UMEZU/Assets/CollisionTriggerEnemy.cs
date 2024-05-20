using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionTriggerEnemy : MonoBehaviour
{
    public UnityEvent onCollide;
    bool isComplete;

    private void OnTriggerEnter(Collider other)
    {
        if (!isComplete)
        {
           
            onCollide.Invoke();
            isComplete = true;
            
        }
    }
}
