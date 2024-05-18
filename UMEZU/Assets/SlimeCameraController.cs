using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SlimeCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float sensitivity = 0.1f; // Adjust this value to control the offset sensitivity

    private CinemachineFramingTransposer framingTransposer;

    void Start()
    {
        if (virtualCamera != null)
        {
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    void Update()
    {
        if (framingTransposer != null)
        {
            Vector2 mousePosition = Input.mousePosition;
            

            Debug.Log($"Screen X: {mousePosition}");

            //framingTransposer.m_ScreenX = 
            //framingTransposer.m_ScreenY = 
        }
    }
}
