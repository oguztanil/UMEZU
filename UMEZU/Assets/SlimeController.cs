using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlimeController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;  // Speed of rotation
    Animator anim;
    public bool mouseDown;

    [SerializeField] private TrajectoryPrediction trajectoryPrediction;

    private Vector3 forward, right;
    public enum SlimeState { Idle, Moving, PreparingJump, Jumping}
    public SlimeState currentState;

    void Start()
    {
        // Calculate the directions based on the isometric view
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);

        right = Camera.main.transform.right;
        right.y = 0;
        right = Vector3.Normalize(right);

        anim = GetComponent<Animator>();

        currentState = SlimeState.Idle;
    }

    void Update()
    {
        
        //ScaleUp();
        //ScaleDown();

        switch (currentState)
        {
            case SlimeState.Idle:
                Move();
                HandleInputStartJump();
                LookAtCursor();
                break;
            case SlimeState.Moving:
                Move();
                HandleInputStartJump();
                break;
            case SlimeState.PreparingJump:
                LookAtCursor();
                break;
            case SlimeState.Jumping:
                
                break;
            default:
                break;
        }
    }

    void Move()
    {
        // Get the input directions
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 rightMovement = right * direction.x;
        Vector3 upMovement = forward * direction.z;

        // Calculate the heading direction
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        // Only move and rotate if there is input
        if (heading != Vector3.zero)
        {
            anim.SetBool("moving", true);
            // Calculate the target position
            Vector3 targetPosition = transform.position + heading * moveSpeed * Time.deltaTime;

            // Move the player
            transform.position = targetPosition;

            // Rotate the player towards the heading direction
            Quaternion targetRotation = Quaternion.LookRotation(heading);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            currentState = SlimeState.Moving;
        }
        else
        {
            anim.SetBool("moving", false);
            
            currentState = SlimeState.Idle;
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

    void LookAtCursor()
    {
        // Get mouse position normalized (0 to 1)
        float mouseX = 1-(Input.mousePosition.x / Screen.width);
        float mouseY = Input.mousePosition.y / Screen.height;

        // Calculate the angle based on screen position
        float angle = Mathf.Atan2(mouseY - 0.5f, mouseX - 0.5f) * Mathf.Rad2Deg;

        // Apply the rotation to the character
        transform.rotation = Quaternion.Euler(0, angle - 45, 0); // Adjust with -45 if character's forward is along the 45-degree axis
    }

    Coroutine prepareJumpRoutine;

    void HandleInputStartJump()
    {
        if (Input.GetMouseButtonDown(0) && !mouseDown)
        {
            currentState = SlimeState.PreparingJump;
            mouseDown = true;
            anim.SetTrigger("prepJump");
            if (prepareJumpRoutine != null)
            {
                StopCoroutine(prepareJumpRoutine);
            }
            prepareJumpRoutine = StartCoroutine(PrepareJump());
        }
    }
    void HandleInputStopJump()
    {
        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            currentState = SlimeState.Idle;
            
        }
    }

    IEnumerator PrepareJump()
    {
        Vector3 jumpLocation = transform.position;
        float jumpPower = 0;
        while (mouseDown)
        {
            jumpPower += Time.deltaTime * 0.4f;
            jumpPower = Mathf.Min(jumpPower, 1);
            jumpLocation = (transform.forward * (jumpPower * 3));

            //Send data to trajectory
            trajectoryPrediction.ShowTrajectory(transform.position, jumpLocation, jumpPower);

            if (Input.GetMouseButtonUp(0))
            {
                DoJump(jumpLocation);
                mouseDown = false;
                break;

            }
            yield return null;
        }
    }

    void DoJump(Vector3 jumpLocation)
    {
        Sequence jumpSequence = DOTween.Sequence();
        jumpSequence
            .AppendCallback(() =>
            {
                currentState = SlimeState.Jumping;
                anim.SetTrigger("jump");
                Debug.Log("Jumped");
            })
            .Append(this.transform.DOMove(transform.position + jumpLocation, 1).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                anim.SetTrigger("land");
                Debug.Log("Landed");
                currentState = SlimeState.Idle;

            }));

    }

}
