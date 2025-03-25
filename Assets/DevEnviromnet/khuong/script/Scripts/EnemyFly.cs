
using UnityEngine;
using System.Collections;


public class EnemyFly : Enemy, IDamageable
{

    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float minPatrolDistance = 5f;
    [SerializeField] private float maxPatrolDistance = 10f;
    [SerializeField] private float patrolHeightVariation = 2f;
    // [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private Animator animator;
    // private bool isAttacking = false;
    // private float lastAttackTime = 0f;
    private Vector2 patrolTarget;
    private bool movingRight = true;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        spawnPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetNextPatrolTarget();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isAttacking && distanceToPlayer > attackRange)
        {
            isAttacking = false;
            animator.ResetTrigger("Attack");
        }

        if (distanceToPlayer < detectionRange)
        {
            if (distanceToPlayer < attackRange && Time.time - lastAttackTime > attackCooldown)
            {
                Attack();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    protected override void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }

    void ChasePlayer()
    {
        isAttacking = false;
        animator.SetInteger("State", 3); // Đặt State = 3
        animator.SetBool("Action", false);

        Vector2 direction = (player.position - transform.position).normalized;
        Flip(direction.x);
        transform.position += (Vector3)direction * FlySpeed * Time.deltaTime;
    }

    protected override void Patrol()
    {
        if (isAttacking) return;

        animator.SetInteger("State", 2); // Đặt State = 3
        animator.SetBool("Action", false);

        if (Vector2.Distance(transform.position, patrolTarget) < 0.5f || IsObstacleAhead())
        {
            SetNextPatrolTarget();
        }

        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        Flip(direction.x);
        transform.position += (Vector3)direction * FlySpeed * Time.deltaTime;

    }

    void SetNextPatrolTarget()
    {
        float patrolDistance = Random.Range(minPatrolDistance, maxPatrolDistance);
        float heightOffset = Random.Range(-patrolHeightVariation, patrolHeightVariation);

        movingRight = !movingRight;

        patrolTarget = spawnPosition + new Vector2(movingRight ? patrolDistance : -patrolDistance, heightOffset);
    }

    bool IsObstacleAhead()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 3f, obstacleLayer);
        return hit.collider != null;
    }

    void Flip(float directionX)
    {
        if ((directionX > 0 && transform.localScale.x < 0) || (directionX < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    protected override void Die()
    {
        animator.SetInteger("State", 9);
        StartCoroutine(ReturnToPoolAfterDelay());


    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        EnemySpawner enemy_Pool = Object.FindFirstObjectByType<EnemySpawner>();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        ResetState();
        enemy_Pool.ReturnEnemyToPool(this);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / Hp;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
