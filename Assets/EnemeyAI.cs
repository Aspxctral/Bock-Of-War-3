using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;      // Assign points in order
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 2f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public float detectionRadius = 8f;
    public float stoppingDistance = 1.5f;

    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private float waitTimer = 0f;

    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
            agent.speed = patrolSpeed;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is close
        isChasing = distanceToPlayer <= detectionRadius;

        if (isChasing)
        {
            agent.speed = chaseSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(player.position);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0f;

        if (!agent.pathPending && agent.remainingDistance <= 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPointIndex].position);
                waitTimer = 0f;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        if (patrolPoints != null)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);
            }
        }
    }
}