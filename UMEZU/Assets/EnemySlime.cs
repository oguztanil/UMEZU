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
        Attack
    }

    public State currentState;
    public Transform[] waypoints;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float idleTime = 2f;

    private int currentWaypointIndex = 0;
    private Transform player;
    private NavMeshAgent agent;
    private float idleTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = State.Idle;
    }

    public void InitializeEnemy()
    {

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
    }

    void Attack()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange)
        {
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Perform attack (placeholder for damage function)
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, detectionRange))
        {
            if (hit.transform == player)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
