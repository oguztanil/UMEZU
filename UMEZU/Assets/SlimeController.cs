using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlimeController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;  // Speed of rotation

    private Vector3 forward, right;

    void Start()
    {
        // Calculate the directions based on the isometric view
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);

        right = Camera.main.transform.right;
        right.y = 0;
        right = Vector3.Normalize(right);
    }

    void Update()
    {
        Move();
        ScaleUp();
        ScaleDown();
        InitiateJump();
    }

    void Move()
    {
        // Get the input directions
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 rightMovement = right * direction.x;
        Vector3 upMovement = forward * direction.z;

        // Calculate the heading direction
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        // Only move and rotate if there is input
        if (heading != Vector3.zero)
        {
            // Calculate the target position
            Vector3 targetPosition = transform.position + heading * moveSpeed * Time.deltaTime;

            // Move the player
            transform.position = targetPosition;

            // Rotate the player towards the heading direction
            Quaternion targetRotation = Quaternion.LookRotation(-heading);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void ScaleUp()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 scale;
            scale = transform.localScale;
            scale *= 0.1f;
            transform.DOScale(this.transform.localScale + scale, 0.5f);
        }
    }
    void ScaleDown()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 scale;
            scale = transform.localScale;
            scale *= 0.1f;
            transform.DOScale(this.transform.localScale - scale, 0.5f);

        }
    }

    void InitiateJump()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 clickPosition = hit.point;
                Debug.Log("Mouse click position in world space: " + clickPosition);
            }
        }
    }

}
