using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ozController : MonoBehaviour
{
    
    public float rotationSpeed = 10f;
    Animator anim;
    public bool mouseDown;
    public bool restarting;
    private float moveSpeed => Mathf.Clamp(3 / currentSize, 1, 10);

    private Vector3 forward, right;
    public enum SlimeState { Idle, Moving, PreparingJump, Jumping , colliding}
    public SlimeState currentState;
    private Rigidbody rb;

    [SerializeField] float jumpPrepareSpeed;
    public float maxSize = 100;
    public float minSize = 1;
    [SerializeField] float baseSize = 30;
    public float currentSize = 30;
    Vector3 baseScale = Vector3.one;

    [SerializeField] GameObject splashParticlePrefab;

    bool immovable = false;
    public bool invincible = false;



    [SerializeField] TrajectoryPrediction trajectoryPrediction;

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
        rb = GetComponent<Rigidbody>();

        // Ensure the Rigidbody is not kinematic
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing.");
        }
        else if (rb.isKinematic)
        {
            Debug.LogWarning("Rigidbody component is kinematic. Setting it to non-kinematic.");
            rb.isKinematic = false;
        }

        currentState = SlimeState.Idle;
    }

    public void SetImmovable(bool set)
    {
        immovable = set;
    }

    
    void Update()
    {
        LoseScalePerSec();

        
        if (immovable)
        {
            return;
        }
        if (GameManager.instance.timeStopped)
        {
            return;
        }

        switch (currentState)
        {
            case SlimeState.Idle:
                Move();
                HandleInputStartJump();
                LookAtCursor();
                invincible = false;

                break;
            case SlimeState.Moving:
                Move();
                HandleInputStartJump();
                invincible = false;
                break;
            case SlimeState.PreparingJump:
                LookAtCursor();
                invincible = false;

                break;
            case SlimeState.Jumping:
                invincible = true;
                break;
            case SlimeState.colliding:
                invincible = true;
                break;
            default:
                break;
        }

        CheckFallDown();

    }

    void CheckFallDown()
    {
        if (transform.position.y < -100 && !restarting)
        {
            GameManager.instance.RestartLevel();
            restarting = true;
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

            // Move the player using the Rigidbody
            Vector3 move = heading * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + move);

            // Rotate the player towards the heading direction
            Quaternion targetRotation = Quaternion.LookRotation(heading);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));

            currentState = SlimeState.Moving;
        }
        else
        {
            anim.SetBool("moving", false);
            currentState = SlimeState.Idle;
        }
    }

    public void ScaleUp(float addedSize)
    {
        currentSize = Mathf.Min(currentSize + addedSize, maxSize);
        AdjustScale();

    }
    public void ScaleDown(float lostSize)
    {
        currentSize = Mathf.Max(currentSize - lostSize, minSize);
        AdjustScale();
        CheckDead();
    }

    void AdjustScale()
    {
        float scaleFactor = currentSize / baseSize;
        transform.DOScale(baseScale * scaleFactor, .5f);
        
    }

    private void OnTriggerEnter(Collider other)
    {
       

        if (other.TryGetComponent<SlimeFragment>(out SlimeFragment slimeFragment))
        {
            if (slimeFragment.NotUsed() && currentState != SlimeState.colliding && currentState != SlimeState.Jumping)
            {
                ConsumeObject(slimeFragment);
            }
        }

    }

    void LookAtCursor()
    {
        // Get mouse position normalized (0 to 1)
        float mouseX = 1 - (Input.mousePosition.x / Screen.width);
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
            mouseDown = true;
            anim.SetTrigger("prepJump");
            if (prepareJumpRoutine != null)
            {
                StopCoroutine(prepareJumpRoutine);
            }
            prepareJumpRoutine = StartCoroutine(PrepareJump());
        }
    }

    void LoseScalePerSec()
    {
        currentSize -= Time.deltaTime * 0.7f;
        if (SlimeTimer.instance != null)
        {
            SlimeTimer.instance.SetTimer(currentSize);
        }
        AdjustScale();
    }

    public void GetDamaged(float damageValue)
    {
        if (invincible) return;

        anim.SetTrigger("hurt");
        ScaleDown(damageValue);
        SlimeTimer.instance.Damaged();
        CheckDead();
        GetComponent<SlimePlayerSoundManager>().PlayPunchSound();
        
    }

    void CheckDead()
    {
        if (currentSize <= 1f && !restarting)
        {
            Debug.Log("Dead");
            GameManager.instance.RestartLevel();
            restarting = true;
        }
    }

    IEnumerator PrepareJump()
    {
        currentState = SlimeState.PreparingJump;
        Vector3 jumpLocation = transform.position;
        float jumpPower = 0;
        while (mouseDown)
        {
            jumpPower += Time.deltaTime * jumpPrepareSpeed;
            jumpPower = Mathf.Min(jumpPower, 1);
            jumpLocation = (transform.forward * (jumpPower * 2));
            jumpLocation = transform.position + jumpLocation;
            Debug.Log("Jump location = " + jumpLocation);

            //Send data to trajectory
            trajectoryPrediction.ShowTrajectory(jumpLocation, jumpPower);

            if (Input.GetMouseButtonUp(0))
            {
                if (jumpPower < 0.2f)
                {
                    currentState = SlimeState.Idle;
                    anim.Play("idle");
                    trajectoryPrediction.HideTrajectory();
                    mouseDown = false;
                    break;
                }

                DoJump(jumpLocation);
                mouseDown = false;
                break;

            }
            yield return null;
        }
    }

    Sequence jumpSequence;

    void DoJump(Vector3 jumpLocation)
    {
        jumpSequence = DOTween.Sequence();

        invincible = true;

        jumpSequence
            .AppendCallback(() =>
            {
                currentState = SlimeState.Jumping;
                trajectoryPrediction.HideTrajectory();
                anim.SetTrigger("jump");
                Debug.Log("Jumped");

                invincible = true;
            })
            .Append(this.transform.DOMove(jumpLocation, 1).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                invincible = false;
                anim.SetTrigger("land");
                Debug.Log("Landed");
                currentState = SlimeState.Idle;
                if (splashParticlePrefab != null)
                {
                    VfxManager.instance.InstantiateVFX(splashParticlePrefab, transform.position);
                }
            }));

    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemySlime friendlySlime) && currentState == SlimeState.Moving)
        {
            if (friendlySlime.isFriendly)
            {
                if (currentSize > friendlySlime.currentSize)
                {
                    Vector3 friendlySlimePos = friendlySlime.transform.position;
                    Quaternion rotation = friendlySlime.transform.rotation;
                    GameObject fragmentPrefab = friendlySlime.slimeFragmentPrefab;

                    friendlySlime.gameObject.SetActive(false);

                    GameObject fragmentInstanceGO = Instantiate(fragmentPrefab, friendlySlimePos, rotation);
                    //SlimeFragment fragmentInstance = fragmentInstanceGO.GetComponent<SlimeFragment>();
                    //Debug.Log(fragmentInstance);
                    //if (fragmentInstance.NotUsed())
                    //{
                    //    ConsumeObject(fragmentInstance); 
                    //}
                }
            }
        }

        if (collision.gameObject.TryGetComponent(out EnemySlime enemySlime) && currentState == SlimeState.Jumping)
        {
            Vector3 bounceDirection = (transform.position - collision.contacts[0].point).normalized;
            bounceDirection.y = 0;
            Vector3 bounceTarget = transform.position + bounceDirection * 1.5f; // Replace bounceDistance with your desired distance

            Sequence damageSequence = DOTween.Sequence();

            Sequence bounceSequence = DOTween.Sequence();
            bounceSequence.AppendCallback(() =>
            {
                Debug.Log("Enemy Hit");
                anim.SetTrigger("collideIdle");
                jumpSequence.Kill();
                currentState = SlimeState.colliding;
                enemySlime.GetDamaged(10);
                GetComponent<SlimePlayerSoundManager>().PlayPunchSound();

                if (splashParticlePrefab != null)
                {
                    VfxManager.instance.InstantiateVFX(splashParticlePrefab, transform.position);
                }
            })
            .Append(this.transform.DOMove(bounceTarget, 1).SetEase(Ease.OutQuad))
            .OnComplete(() =>
            {
                anim.SetTrigger("land");
                currentState = SlimeState.Idle; // Set to appropriate state after bounce

            });

        }

        if (collision.gameObject.CompareTag("Obstacle") && currentState == SlimeState.Jumping)
        {
            
            // Calculate bounce direction
            Vector3 bounceDirection = (transform.position - collision.contacts[0].point).normalized;
            Vector3 bounceTarget = transform.position;//new Vector3(-bounceDirection.x, 0, bounceDirection.z);

            // Create a new bounce sequence
            Sequence bounceSequence = DOTween.Sequence();
            bounceSequence.AppendCallback(() =>
            {
                Debug.Log("Obstacle");
                jumpSequence.Kill();
                anim.SetTrigger("collide");
                currentState = SlimeState.colliding;

                if (splashParticlePrefab != null)
                {
                    VfxManager.instance.InstantiateVFX(splashParticlePrefab, transform.position);
                }
            })
             .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                anim.SetTrigger("collideIdle");
            })
            .Append(this.transform.DOMove(bounceTarget, 1).SetEase(Ease.OutQuad))
            .OnComplete(() =>
            {
                currentState = SlimeState.Idle; // Set to appropriate state after bounce
            
            });
        }

       
    }




    public void ConsumeObject(SlimeFragment slimeFragment)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            GetComponent<SlimePlayerSoundManager>().PlayEatingSound();
            slimeFragment.StartConsuming(this);
            SetImmovable(true);
            transform.LookAt(slimeFragment.transform);
        }).Append(transform.DOMove(new Vector3(slimeFragment.transform.position.x, transform.position.y, slimeFragment.transform.position.z), 1f))
        .OnComplete(() =>
        {
            ScaleUp(slimeFragment.healAmount);
            slimeFragment.FinishConsuming(this);
            SetImmovable(false);
        });

    }


}