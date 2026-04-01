using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    [Header("Attack")]
    public float attackRange = 1.8f;        // Distance to start tornado kick
    public float attackCooldown = 1.8f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTime = 2f;

    [Header("Chase")]
    public float chaseSpeed = 4.5f;
    public float detectionRadius = 8f;
    public float stoppingDistance = 1.2f;   // ← New: how close to stop before kicking

    private NavMeshAgent agent;
    private Animator anim;
    private int patrolIndex = 0;
    private float waitTimer;
    private bool isChasing;
    private float lastAttackTime;
    private bool isAttacking;

    private Vector3 lastPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = patrolSpeed;
        lastPosition = transform.position;

        if (patrolPoints.Length > 0)
        {
            patrolIndex = Random.Range(0, patrolPoints.Length);
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

        agent.enabled = false;           // Pause agent during kick
        anim.SetTrigger("TornadoKick");

        Invoke(nameof(EndAttack), 1.8f); // Match your kick animation length
    }

    void EndAttack()
    {
        if (agent != null)
        {
            agent.enabled = true;

            // Force re-target after kick
            if (isChasing && player != null)
                agent.SetDestination(player.position);
            else if (patrolPoints.Length > 0)
                agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        isAttacking = false;
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
                patrolIndex = Random.Range(0, patrolPoints.Length);
                agent.SetDestination(patrolPoints[patrolIndex].position);
                waitTimer = 0f;
            }
        }
    }

    void ChasePlayer()
    {
        isChasing = true;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = stoppingDistance;   // ← Important

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > stoppingDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            // Very close - stop moving and prepare for kick
            agent.ResetPath();
            // Do NOT manually move the enemy here - let the attack handle it
        }
    }

    void HandleAnimations()
    {
        float speed = 0f;

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            speed = agent.velocity.magnitude;
        }

        if (speed < 0.1f && !isAttacking)
        {
            Vector3 delta = transform.position - lastPosition;
            speed = delta.magnitude / Time.deltaTime;
        }

        lastPosition = transform.position;

        bool walking = speed > 0.4f && !isChasing && !isAttacking;
        bool running = speed > 0.8f && isChasing && !isAttacking;

        anim.SetBool("isWalking", walking);
        anim.SetBool("isRunning", running);
    }
}