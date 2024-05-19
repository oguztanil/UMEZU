using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySlime : MonoBehaviour
{
    public enum State
    {
        Idle,
        Patrol,
        Attack,
        Investigate
    }

    public State currentState;
    public Transform[] waypoints;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float idleTime = 2f;
    public float investigateTime = 2f; // Time to investigate each waypoint around the last known location
    public float fieldOfView = 45f; // Field of view angle
    public float investigateRadius = 2f; // Radius around the last known location to create investigation waypoints

    private int currentWaypointIndex = 0;
    private Transform player;
    private NavMeshAgent agent;
    private float idleTimer = 0f;
    private float investigateTimer = 0f;
    private Vector3 lastKnownPlayerPosition;
    private Vector3[] investigateWaypoints;
    private int currentInvestigateWaypointIndex = 0;
    private bool playerInSight = false;
    private bool reachedLastKnownPosition = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = State.Idle;
    }

    public void InitializeEnemy()
    {
        // Initialize any other enemy-specific parameters here if needed
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrol:
                Patrol();
                break;
            case State.Attack:
                Attack();
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
            currentState = State.Attack;
        }

        FaceForward();
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        if (CanSeePlayer())
        {
            currentState = State.Attack;
        }

        FaceMovementDirection();
    }

    void Attack()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange || !CanSeePlayer())
        {
            currentState = State.Investigate;
            investigateTimer = 0f;
            reachedLastKnownPosition = false;
            agent.SetDestination(lastKnownPlayerPosition);
            return;
        }

        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Perform attack (placeholder for damage function)
        }

        FaceMovementDirection();
    }

    void Investigate()
    {
        if (CanSeePlayer())
        {
            currentState = State.Attack;
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
}