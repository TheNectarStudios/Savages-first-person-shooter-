using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Animator animator;
    private Transform player;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;

    [Header("Audio Settings")]
    public AudioSource continuousRoar;
    public AudioSource hyperRoar;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    private Vector3 randomPatrolPoint;
    private float patrolTimer;

    private bool isPatrolling = true;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize health and patrolling
        currentHealth = maxHealth;
        SelectRandomPatrolPoint();
    }

    void Update()
    {
        if (isDead)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartAttacking();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            StartChasing();
        }
        else
        {
            StartPatrolling();
        }

        UpdateAnimatorParameters();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

   private void Die()
{
    isDead = true;
    animator.SetTrigger("Die"); // Trigger the die animation
    continuousRoar.Stop();
    hyperRoar.Stop();

    // Disable further movement and AI logic
    isPatrolling = false;
    isChasing = false;
    isAttacking = false;

    // Disable collider if necessary
    Collider collider = GetComponent<Collider>();
    if (collider != null)
        collider.enabled = false;

    // Freeze Rigidbody to stop motion and rotation
    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    // Optionally destroy the zombie after the death animation finishes
    // Adjust the time to match the animation length
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

        isPatrolling = true;
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

        isPatrolling = true;
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
        transform.LookAt(player);
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("patrolling", isPatrolling);
        animator.SetBool("chasing", isChasing);
        animator.SetBool("attacking", isAttacking);
    }

    private void  OnTriggerEnter(Collider other)
    {
        // Check if the object hitting the zombie has the "Bullet" tag
        if (other.CompareTag("Bullet"))
        {
            // Apply damage (you can customize this value based on your bullet system)
            TakeDamage(20);
            Debug.Log("Zombie hit by bullet!");
            Debug.Log("Current Health: " + currentHealth);

            // Destroy the bullet after it hits
          
        }
    }
}
