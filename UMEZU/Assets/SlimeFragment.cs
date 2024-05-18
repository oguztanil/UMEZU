using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlimeFragment : MonoBehaviour
{
    public int healAmount;
    bool used = false;

    [SerializeField] GameObject eatenParticle;
    
    public void StartConsuming(ozController controller)
    {
        used = true;
        transform.parent.GetComponent<Collider>().enabled = false;
        transform.parent.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void FinishConsuming(ozController controller)
    {
        
        

        if (eatenParticle != null)
        {
            Quaternion oppositeRotation = Quaternion.Inverse(controller.transform.rotation);

            VfxManager.instance.InstantiateVFX(eatenParticle, transform.position, oppositeRotation);
        }
        transform.parent.gameObject.SetActive(false);
    }

    public bool NotUsed()
    {
        return !used;
    }

}
