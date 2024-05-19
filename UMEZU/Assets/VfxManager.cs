using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using DG.Tweening;

public class VfxManager : MonoBehaviour
{

    [SerializeField] Volume globalVolume;

    public bool debugMode = false;
    

    #region Singleton

    public static VfxManager instance;
    private void Awake()
    {
       
        instance = this;
        if (debugMode) return;
        DontDestroyOnLoad(this.gameObject);
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

    public void LoadSceneEffect()
    {
        globalVolume.profile.TryGet(out LensDistortion lensDistortion);

        DOVirtual.Float(0, .5f, 0.7f, value =>
        {
            lensDistortion.intensity.value = value;
        });
        DOVirtual.Float(0, -1, 1, value =>
        {
            lensDistortion.intensity.value = value;
        });
        globalVolume.profile.TryGet(out Vignette vignette);

        //vignette.color.value = Color.white;
        //vignette.intensity.value = .5f;

    }

    public void StartSceneEffect()
    {
        globalVolume.profile.TryGet(out LensDistortion lensDistortion);

        DOVirtual.Float(-1, 0, 1, value =>
        {
            lensDistortion.intensity.value = value;
        });
        
    }

}
