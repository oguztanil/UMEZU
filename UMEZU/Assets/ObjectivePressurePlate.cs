using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectivePressurePlate : MonoBehaviour
{

    public UnityEvent onComplete;

    [SerializeField] GameObject[] objectiveGameObjects;
    bool completed = false;

    private void OnTriggerEnter(Collider other)
    {
       if (completed == false)
       {
            CheckIfCorrectObject(other.gameObject);
       }
    }

    void CheckIfCorrectObject(GameObject collidedObject)
    {
        foreach (var go in objectiveGameObjects)
        {
            if (go == collidedObject)
            {
                completed = true;
                onComplete.Invoke();
            }
        }
    }

}
