using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class EnemySlime : MonoBehaviour
{
    public enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Damaged,
        Flee,
        Investigate
    }

    [Header("SlimeSettings")]
    public bool isFriendly = false;
    public float currentSize = 30;
    public float maxSize = 100;
    public Vector3 baseScale = Vector3.one;
    public float minSize = 1;
    public float baseSize = 30;
    [SerializeField] GameObject slimeFragmentPrefab;

    [Header("AI Settings")]
    public State currentState;
    public Transform[] waypoints;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float idleTime = 2f;
    public float investigateTime = 2f; // Time to investigate each waypoint around the last known location
    public float fieldOfView = 45f; // Field of view angle
    public float investigateRadius = 2f; // Radius around the last known location to create investigation waypoints
    public float attackableSize = 40f;
    public float fleeSpeed = 3f;
    public float fleeTime = 5f;
    public float fleeRadius = 1f;
    public float damageValue = 5f;
    public float attackDelay = 1f;
    
    private float fleeStartTime;

    private int currentWaypointIndex = 0;
    private Transform player;
    private ozController slimeController;
    private NavMeshAgent agent;
    private float idleTimer = 0f;
    private float investigateTimer = 0f;
    private Vector3 lastKnownPlayerPosition;
    private Vector3[] investigateWaypoints;
    private int currentInvestigateWaypointIndex = 0;
    private bool playerInSight = false;
    private bool reachedLastKnownPosition = false;
    private bool fleeing = false;

    public Animator anim;

    bool isAttacking = false;
    public float attackCooldown = 1f;
    float attackTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        slimeController = GameManager.instance.GetPlayerSlime();
        player = slimeController.transform;
        currentState = State.Idle;

    }

    public void InitializeEnemy()
    {
        // Initialize any other enemy-specific parameters here if needed
    }

    void Update()
    {
        // Eğer oyuncu, NPC'nin "fleeRadius" içine girmişse ve NPC'nin durumu "Idle" ise
        if (player != null && currentState == State.Flee)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > fleeRadius)
            {
                // NPC'nin "Flee" durumuna geç
                currentState = State.Idle;
            }
        }

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chasing();
                break;
            case State.Flee: // Flee durumu eklendi
                Flee();
                break;
            case State.Damaged:
                Damaged();
                break;
                
            case State.Attack:
                Attacking();
                break;
            case State.Investigate:
                Investigate();
                break;
        }
    }

    void Idle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;
            currentState = State.Patrol;
        }

        if (CanSeePlayer())
        {
            if (isFriendly) 
            {
                currentState = State.Flee;

            }
            else
            {
                currentState = State.Chase;

            }
        }

        FaceForward();
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        anim.SetBool("moving", true);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        if (CanSeePlayer())
        {
            if (isFriendly)
            {
                currentState = State.Flee;
            }
            else
            {
                currentState = State.Chase;
            }

            return;


        }
        else
        {
            FaceMovementDirection();
        }

    }


    void Chasing()
    {
        if (player == null) return;

        

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //if (distanceToPlayer > detectionRange || !CanSeePlayer())
        //{
        //    currentState = State.Investigate;
        //    investigateTimer = 0f;
        //    reachedLastKnownPosition = false;
        //    agent.SetDestination(lastKnownPlayerPosition);
        //    return;
        //}

        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackRange && currentState != State.Attack)
        {
            Debug.Log($"Distance to player: {distanceToPlayer} and attack range is {attackRange}");
            currentState = State.Attack;
            
            return;

        }
        

        FaceMovementDirection();
    }

    private float attackCooldownTimer = 0;
    void Attacking()
    {

        if (attackCooldownTimer >= 1 && slimeController.invincible == false)
        {
            Debug.Log("Attacked");
            anim.SetTrigger("attack");
            attackCooldownTimer = 0;

            transform.LookAt(player);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer >= attackRange)
            {

                currentState = State.Chase;
                return;

            }
            currentState = State.Chase;
            return;
        }
        
        attackCooldownTimer += Time.deltaTime;
    }

    public void DamagePlayer()
    {
        slimeController.GetDamaged(damageValue);
        
    }
    public void GetDamaged(float damageValue)
    {
        
        anim.SetTrigger("hurt");
        ScaleDown(damageValue);
        currentState = State.Damaged;
        damagedDurationTimer = 0;
        ThrowSlimeFragment();
        CheckDead();

    }
    private void ThrowSlimeFragment()
    {
        if (slimeFragmentPrefab != null)
        {
            // Instantiate the slime fragment at the slime's position
            GameObject slimeFragment = Instantiate(slimeFragmentPrefab, transform.position, Quaternion.identity);

            // Get the Rigidbody component of the slime fragment
            Rigidbody rb = slimeFragment.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calculate a random direction slightly away from the slime on the X and Z axes
                Vector3 throwDirection = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f)).normalized;

                // Apply force to the slime fragment to throw it away
                float throwForce = 5f; // Adjust this value as needed
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }
        }
    }
    float damagedDuration = 1.5f;
    float damagedDurationTimer = 0;
    void Damaged()
    {
        if (damagedDurationTimer < damagedDuration)
        {
            damagedDurationTimer += Time.deltaTime;
            transform.position = transform.position;
        }
        else
        {
            currentState = State.Chase;
        }

    }

    void Flee()
    {
        if (player == null) return;

        
        // Oyuncunun konumunu al
        Vector3 playerPosition = player.position;

        // NPC'nin bulunduğu konumdan oyuncunun konumunu çıkar ve bu kaçış yönü olarak belirlenir
        Vector3 fleeDirection = transform.position - playerPosition;

        // Yönü normalize et, sadece yönünü al, uzunluğu önemli değil
        fleeDirection.Normalize();

        float fleeDistance = 1000f; // This value determines how far the NPC will try to flee
        Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

        // Check if the flee target is within the NavMesh, otherwise find the nearest point on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, fleeDistance, NavMesh.AllAreas))
        {
            fleeTarget = hit.position; // Set fleeTarget to the nearest valid position on the NavMesh
        }

        // Set the NavMeshAgent destination to the flee target
        agent.SetDestination(fleeTarget);



        // NPC'nin dönüşü, kaçış yönünün tersine doğru yapılır
        transform.rotation = Quaternion.LookRotation(fleeDirection);
        
        FaceMovementDirection();
    }

    void Investigate()
    {
        if (CanSeePlayer())
        {
            currentState = State.Chase;
            return;
        }

        if (!reachedLastKnownPosition)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                reachedLastKnownPosition = true;
                GenerateInvestigateWaypoints();
                currentInvestigateWaypointIndex = 0;
                agent.SetDestination(investigateWaypoints[currentInvestigateWaypointIndex]);
            }
            return;
        }

        if (CanSeePlayer())
        {
            currentState = State.Attack;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            investigateTimer += Time.deltaTime;
            if (investigateTimer >= investigateTime)
            {
                investigateTimer = 0f;
                currentInvestigateWaypointIndex++;

                if (currentInvestigateWaypointIndex >= investigateWaypoints.Length)
                {
                    currentState = State.Patrol;
                    agent.SetDestination(waypoints[currentWaypointIndex].position);
                    return;
                }

                agent.SetDestination(investigateWaypoints[currentInvestigateWaypointIndex]);
            }
        }

        FaceMovementDirection();
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer <= fieldOfView / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
            {
                if (hit.transform == player)
                {
                    // Oyuncunun boyut bilgisine erişim
                    ozController playerController = player.GetComponent<ozController>();
                    if (playerController != null)
                    {
                        // Burada oyuncunun boyutunu kullanabilirsiniz
                        if (playerController.currentSize > attackableSize)
                        {
                            float playerSize = playerController.currentSize;
                            Debug.Log("Player'ın boyutu: " + playerSize + ". KAÇ!");
                            currentState = State.Flee; // NPC'nin boyutu 50'den büyükse flee durumuna geçer
                            return false;
                        }
                        
                        else if (playerController.currentSize <= attackableSize)
                        {
                            float playerSize = playerController.currentSize;
                            Debug.Log("Player'ın boyutu: " + playerSize + ". SALDIR!");
                        }
                    }

                    playerInSight = true;
                    lastKnownPlayerPosition = player.position; // Update last known position
                    return true;
                }
            }
        }

        if (playerInSight)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (lastKnownPlayerPosition - transform.position).normalized, out hit, detectionRange))
            {
                if (hit.transform != player)
                {
                    lastKnownPlayerPosition = hit.point; // Update to the point of lost line of sight
                    playerInSight = false;
                }
            }
        }

        return false;
    }

    private void FaceForward()
    {
        if (agent.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
    }

    private void FaceMovementDirection()
    {
        if (agent.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
    }

    private void GenerateInvestigateWaypoints()
    {
        investigateWaypoints = new Vector3[3];

        for (int i = 0; i < 3; i++)
        {
            float angle = i * 120f; // 120 degrees between each waypoint
            Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * investigateRadius;
            investigateWaypoints[i] = lastKnownPlayerPosition + offset;
        }

        // Drop a debug sphere at the last known player position
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.transform.position = lastKnownPlayerPosition;
        debugSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Set size to 0.1f
        debugSphere.GetComponent<Collider>().enabled = false;

        // Set color to red
        Renderer renderer = debugSphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        Destroy(debugSphere, investigateTime * 3); // Destroy after the investigation time
    }

    private void OnDrawGizmosSelected()
    {
        // NPC'nin kaçma yarıçapını çiz
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw the field of view cone
        Vector3 forward = transform.forward * detectionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position + leftBoundary, transform.position + rightBoundary);

        // Draw investigate waypoints if they exist
        if (investigateWaypoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var point in investigateWaypoints)
            {
                Gizmos.DrawWireSphere(point, 0.1f);
            }
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


    void CheckDead()
    {
        
        if (currentSize <= 1f)
        {
            Debug.Log("Dead");

            this.gameObject.SetActive(false);
        }
    }
}