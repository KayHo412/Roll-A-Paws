using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;
    private bool isDead = false;

    private int currentAnimationHash;
    private int vertParamHash;
    private int stateParamHash;
    private bool useBlendTree = false;

    [Header("Target Tracking")]
    public Transform player;

    [Header("AI Settings")]
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float wanderRadius = 8f;

    // Exposed to the Inspector for easy tuning!
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float wanderSpeed = 2f;

    private static readonly int IdleState = Animator.StringToHash("idle");
    private static readonly int MoveState = Animator.StringToHash("move");
    private static readonly int AttackState = Animator.StringToHash("attack");
    private static readonly int DeathState = Animator.StringToHash("dissolve");

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentAnimationHash = IdleState;

        if (anim != null)
        {
            vertParamHash = Animator.StringToHash("Vert");
            stateParamHash = Animator.StringToHash("State");
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.nameHash == vertParamHash) { useBlendTree = true; break; }
            }
        }
    }

    void Update()
    {
        // Safety check: Don't do anything if dead, missing player, or agent isn't ready
        if (isDead || player == null || agent == null || !agent.enabled) return;

        // Check if the NavMesh Agent can calculate a complete, valid path to the player
        NavMeshPath path = new NavMeshPath();
        bool hasValidPath = agent.CalculatePath(player.position, path) && path.status == NavMeshPathStatus.PathComplete;

        if (hasValidPath)
        {
            // --- CHASE MODE ---
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackDistance)
            {
                agent.isStopped = true;
                HandleMovementAnimation(false, false, AttackState);
            }
            else
            {
                agent.isStopped = false;
                agent.speed = chaseSpeed; // Uses your custom Chase Speed value
                agent.SetPath(path);
                HandleMovementAnimation(true, true, MoveState); // Full run
            }
        }
        else
        {
            // --- WANDER MODE ---
            agent.isStopped = false;
            agent.speed = wanderSpeed; // Uses your custom Wander Speed value

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius + transform.position;
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }

            HandleMovementAnimation(true, false, MoveState); // Gentle walk
        }
    }

    void HandleMovementAnimation(bool isMoving, bool isRunning, int ghostStateHash)
    {
        if (anim == null) return;

        if (useBlendTree)
        {
            if (isMoving)
            {
                anim.SetFloat(vertParamHash, 1f);
                anim.SetFloat(stateParamHash, isRunning ? 1f : 0f);
            }
            else
            {
                anim.SetFloat(vertParamHash, 0f);
                anim.SetFloat(stateParamHash, 0f);
            }
        }
        else
        {
            if (currentAnimationHash == ghostStateHash) return;
            anim.CrossFade(ghostStateHash, 0.1f, 0, 0);
            currentAnimationHash = ghostStateHash;
        }
    }

    public void TriggerDissolve()
{
    if (isDead) return;
    isDead = true;

    // Freeze pathfinding instantly for both enemy types
    if (agent != null && agent.enabled) agent.isStopped = true;

    if (useBlendTree)
    {
        // --- CHICKEN HEADLESS FIX ---
        // No death animation state available in this asset pack, vanish instantly!
        Destroy(gameObject);
    }
    else
    {
        // --- GHOST FALLBACK ---
        // Play the original dissolve crossfade animation and destroy after 2 seconds
        if (anim != null) anim.CrossFade(DeathState, 0.1f, 0, 0);
        Destroy(gameObject, 2.0f);
    }
}
}