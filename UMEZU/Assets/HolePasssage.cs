using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolePasssage : MonoBehaviour
{

    [SerializeField] float maxSizeToFit;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out ozController slimeController))
        {
            if (slimeController.currentSize < maxSizeToFit)
            {
                this.GetComponent<Collider>().enabled = false;
            }
            
        }
    }

   


}
