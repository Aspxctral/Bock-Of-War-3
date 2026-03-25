using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    [Header("Attack")]
    public float attackRange = 1.8f;
    public float attackCooldown = 1.8f;

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
    private float lastAttackTime;
    private bool isAttacking;

    // For animation fallback when agent velocity isn't reliable
    private Vector3 lastPosition;
    private float manualSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = patrolSpeed;

        lastPosition = transform.position;

        if (patrolPoints.Length > 0)
        {
            // 🔥 pick random starting point
            patrolIndex = Random.Range(0, patrolPoints.Length);

            // move to that point
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    void Update()
    {
        if (isAttacking) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            StartAttack();
        }
        else if (dist < detectionRadius)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        HandleAnimations();
    }

    void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if (agent.isOnNavMesh)
            agent.ResetPath();

        agent.enabled = false;

        anim.SetTrigger("TornadoKick");

        Invoke(nameof(EndAttack), 1.8f); // ← match your kick animation length
    }

    void EndAttack()
    {
        if (agent != null)
        {
            agent.enabled = true;

            // Force re-target to kickstart velocity
            if (isChasing && player != null)
                agent.SetDestination(player.position);
            else if (patrolPoints.Length > 0)
                agent.SetDestination(patrolPoints[patrolIndex].position);

            // Small nudge to wake velocity
            agent.velocity = Vector3.zero;
        }

        isAttacking = false;
    }
    void Patrol()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return; // 🔥 prevents error

        isChasing = false;
        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                patrolIndex = Random.Range(0, patrolPoints.Length); // 🔥 random patrol
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
            agent.ResetPath();
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * chaseSpeed * Time.deltaTime;
        }
    }

    void HandleAnimations()
    {
        // Primary: agent velocity when active
        float speed = 0f;
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            speed = agent.velocity.magnitude;
        }

        // Fallback: manual speed calculation (reliable after agent re-enable)
        if (speed < 0.1f && !isAttacking)
        {
            Vector3 delta = transform.position - lastPosition;
            speed = delta.magnitude / Time.deltaTime;
        }

        lastPosition = transform.position;

        bool walking = speed > 0.3f && !isChasing && !isAttacking; // lowered threshold to catch small speeds
        bool running = speed > 0.3f && isChasing && !isAttacking;

        anim.SetBool("isWalking", walking);
        anim.SetBool("isRunning", running);
    }
}