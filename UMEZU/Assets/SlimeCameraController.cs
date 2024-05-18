using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SlimeCameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform characterTransform;
    private CinemachineVirtualCamera virtualCamera;
    public float sensitivity = 0.4f; // Adjust this value to control the offset sensitivity

    private CinemachineFramingTransposer framingTransposer;

    private Vector3 initialOffset;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        initialOffset = framingTransposer.m_TrackedObjectOffset;
    }

    void Update()
    {
        UpdateOffset();
    }

    void UpdateOffset()
    {

        float mouseY = Mathf.Clamp(Input.mousePosition.y / Screen.height, 0.3f, 0.7f);
        
        float mouseX = Mathf.Clamp((1 -(Input.mousePosition.x / Screen.width)), 0.3f, 0.7f);

        framingTransposer.m_ScreenY = Mathf.Lerp(framingTransposer.m_ScreenY, mouseY, sensitivity * Time.deltaTime);
        framingTransposer.m_ScreenX = Mathf.Lerp(framingTransposer.m_ScreenX, mouseX, sensitivity * Time.deltaTime);
    }
}
