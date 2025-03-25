using System.Collections;
using UnityEngine;

public class Bat : Enemy, IDamageable
{
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float minPatrolDistance = 5f;
    [SerializeField] private float maxPatrolDistance = 10f;
    [SerializeField] private float patrolHeightVariation = 2f;
    // [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask playerLayer;

    private Animator animator;
    // private bool isAttacking = false;
    // private float lastAttackTime = 0f;
    private Vector2 patrolTarget;
    private bool movingRight = true;
    private Vector2 startPos;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        startPos = transform.position;
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1, playerLayer);
        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(PhysicalDame);
                Debug.Log("Take Dame");
            }
        }
        lastAttackTime = Time.time;
    }

    void ChasePlayer()
    {
        isAttacking = false;
        animator.SetBool("isRuning", true);

        Vector2 direction = (player.position - transform.position).normalized;
        Flip(direction.x);
        transform.position += (Vector3)direction * FlySpeed * Time.deltaTime;
    }

    protected override void Patrol()
    {
        if (isAttacking) return;

        animator.SetBool("isRuning", true);

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

        patrolTarget = startPos + new Vector2(movingRight ? patrolDistance : -patrolDistance, heightOffset);
    }

    bool IsObstacleAhead()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayer);
        return hit.collider != null;
    }

    void Flip(float directionX)
    {
        if ((directionX > 0 && transform.localScale.x < 0) || (directionX < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            healthBar.transform.localScale = new Vector3(-healthBar.transform.localScale.x, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        }
    }

    protected override void Die()
    {
        animator.SetTrigger("Die");
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        Enemy_Pool enemy_Pool = Object.FindFirstObjectByType<Enemy_Pool>();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        ResetState();
        enemy_Pool.ReturnToPool(gameObject);
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
