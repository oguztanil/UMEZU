using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    
    

    

    #region Singleton

    public static VfxManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
    }

    #endregion

    public void InstantiateVFX(GameObject prefab,Vector3 pos, Quaternion rotation)
    {
        
        Instantiate(prefab, pos, rotation);
        
    }
    public void InstantiateVFX(GameObject prefab, Vector3 pos)
    {
        Quaternion rotation = Quaternion.identity;

        Debug.Log("Instantiated FX");
         Instantiate(prefab, pos, rotation);
        
    }


}
