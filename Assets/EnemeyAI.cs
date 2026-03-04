using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTime = 2f;

    [Header("Chase")]
    public float chaseSpeed = 4.5f;
    public float detectionRadius = 8f;

    private NavMeshAgent agent;
    private Animator anim;

    private int patrolIndex = 0;
    private float waitTimer;

    private bool isChasing;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.speed = patrolSpeed;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < detectionRadius)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        HandleAnimations();
    }

    void Patrol()
    {
        isChasing = false;

        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[patrolIndex].position);
                waitTimer = 0f;
            }
        }
    }

    void ChasePlayer()
    {
        isChasing = true;

        agent.speed = chaseSpeed;
        agent.stoppingDistance = 0f;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > 1f)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            // Disable Nav steering when very close
            agent.ResetPath();

            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * chaseSpeed * Time.deltaTime;
        }
    }

    void HandleAnimations()
    {
        float speed = agent.velocity.magnitude;

        bool walking = speed > 0.1f && !isChasing;
        bool running = speed > 0.1f && isChasing;

        anim.SetBool("isWalking", walking);
        anim.SetBool("isRunning", running);
    }
}