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
        // Cast a ray from the mouse position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Get the character's position
            Vector3 characterPosition = characterTransform.position;
            
            // Get the world position of the mouse click
            Vector3 mousePosition = hit.point;
            Vector3 fixedMousePosition = new Vector3(mousePosition.x, characterPosition.y, mousePosition.z);
            
            // Calculate the offset relative to the character's position
            Vector3 worldOffset = (fixedMousePosition - characterPosition) * sensitivity;

            // Convert the world offset to the camera's local space
            Vector3 cameraLocalOffset = cam.transform.InverseTransformDirection(worldOffset);

            // Clamp the cameraLocalOffset components
            cameraLocalOffset.x = Mathf.Clamp(cameraLocalOffset.x, -1.2f, 1.2f);
            cameraLocalOffset.y = Mathf.Clamp(cameraLocalOffset.y, -0.3f, 0.5f);
            cameraLocalOffset.z = Mathf.Clamp(cameraLocalOffset.z, -0.5f, 0.5f); // Optionally clamp Z if necessary
            
            // Update the offset, including Z if necessary
            framingTransposer.m_TrackedObjectOffset = initialOffset + new Vector3(cameraLocalOffset.x, cameraLocalOffset.y, cameraLocalOffset.z);

            Debug.Log("New Tracked Object Offset: " + framingTransposer.m_TrackedObjectOffset);
        }
    }
}
