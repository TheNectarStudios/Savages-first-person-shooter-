using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Animator animator;
    private Transform player;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;         // Speed while patrolling
    public float chaseSpeed = 5f;         // Speed while chasing the player
    public float detectionRange = 10f;    // Range within which the zombie detects the player
    public float attackRange = 2f;        // Range within which the zombie attacks the player
    public float patrolRadius = 10f;      // Radius within which the zombie patrols randomly
    public float patrolWaitTime = 3f;     // Time to wait before selecting a new patrol point

    [Header("Audio Settings")]
    public AudioSource continuousRoar;    // Audio source for the continuous roar
    public AudioSource hyperRoar;         // Audio source for the hyper roar

    private Vector3 randomPatrolPoint;
    private float patrolTimer;

    private bool isPatrolling = false;
    private bool isChasing = false;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize patrolling
        SelectRandomPatrolPoint();
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
        if (!continuousRoar.isPlaying)
        {
            hyperRoar.Stop();
            continuousRoar.Play();
        }

        isPatrolling = true;
        isChasing = false;
        isAttacking = false;

        Patrol();
    }

    private void StartChasing()
    {
        if (!hyperRoar.isPlaying)
        {
            continuousRoar.Stop();
            hyperRoar.Play();
        }

        isPatrolling = false;
        isChasing = true;
        isAttacking = false;

        ChasePlayer();
    }

    private void StartAttacking()
    {
        if (!hyperRoar.isPlaying)
        {
            continuousRoar.Stop();
            hyperRoar.Play();
        }

        isPatrolling = false;
        isChasing = false;
        isAttacking = true;

        AttackPlayer();
    }

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (Vector3.Distance(transform.position, randomPatrolPoint) < 0.5f || patrolTimer >= patrolWaitTime)
        {
            SelectRandomPatrolPoint();
            patrolTimer = 0f;
        }

        MoveTowards(randomPatrolPoint, patrolSpeed);
    }

    private void SelectRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;

        randomPatrolPoint = randomDirection;
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
