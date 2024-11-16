using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Animator animator;
    private Transform player;

    public float patrolSpeed = 2f;         // Speed while patrolling
    public float chaseSpeed = 5f;         // Speed while chasing the player
    public float detectionRange = 10f;    // Range within which the zombie detects the player
    public float attackRange = 2f;        // Range within which the zombie attacks the player
    public Transform[] patrolPoints;      // Points for patrolling
    private int currentPatrolIndex = 0;

    private bool isPatrolling = false;
    private bool isChasing = false;
    private bool isAttacking = false;
     
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Attack the player
            StartAttacking();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Chase the player
            StartChasing();
        }
        else
        {
            // Patrol if no player is near
            StartPatrolling();
        }

        UpdateAnimatorParameters();
    }

    private void StartPatrolling()
    {
        isPatrolling = true;
        isChasing = false;
        isAttacking = false;

        Patrol();
    }

    private void StartChasing()
    {
        isPatrolling = false;
        isChasing = true;
        isAttacking = false;

        ChasePlayer();
    }

    private void StartAttacking()
    {
        isPatrolling = false;
        isChasing = false;
        isAttacking = true;

        AttackPlayer();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // Move towards the current patrol point
        Transform targetPatrolPoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetPatrolPoint.position, patrolSpeed);

        // Check if the zombie has reached the patrol point
        if (Vector3.Distance(transform.position, targetPatrolPoint.position) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ChasePlayer()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    private void AttackPlayer()
    {
        // Here you can add logic for attacking the player, like dealing damage
        transform.LookAt(player); // Face the player
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z)); // Face the target
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("petroling", isPatrolling);
        animator.SetBool("chasing", isChasing);
        animator.SetBool("attacking", isAttacking);
        animator.SetBool("Die", false); // Set this to true when implementing death
    }
}
