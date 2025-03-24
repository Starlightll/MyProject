using System.Collections;
using UnityEngine;

public class Boss2 : Enemy, IDamageable
{
    public Transform attack_Point;
    public float attackRadius = 2.5f;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private float flipCooldown = 0.5f;
    private float lastFlipTime = 0f;

    private float fireDamageTimer = 0f;
    private float fireDamageInterval = 1f;
    private int direction = 1;
    private Animator animator;
    private bool isAttacking = false;
    private bool isChasing = false;
    private GateController gate;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        gate = FindAnyObjectByType<GateController>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayerX = Mathf.Abs(player.position.x - transform.position.x);
        fireDamageTimer += Time.deltaTime;

        // Boss tự gây damage theo chu kỳ (AOE fire)
        if (fireDamageTimer >= fireDamageInterval)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, 4, playerLayer);
            if (hits.Length > 0)
            {
                DealDamage(MagicDame, hits);
                Debug.Log("Boss deals fire damage!");
            }
            fireDamageTimer = 0;
        }

        // Nếu player quá gần, đứng yên
        if (distanceToPlayerX < 0.5f)
        {
            isChasing = false;
            isAttacking = false;
            animator.SetBool("isWalking", false);
        }
        else
        {
            if (PlayerInAttackRange())
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    animator.SetTrigger("Attack");
                }
            }
            else if (CheckInRange())
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }
    }

    protected override void Patrol()
    {
        isChasing = false;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        Vector2 obstacleDir = direction > 0 ? Vector2.right : Vector2.left;
        bool isObstacleAhead = Physics2D.Raycast(groundCheck.position, obstacleDir, 1f, groundLayer);

        if ((!isGroundAhead || isObstacleAhead) && Time.time - lastFlipTime > flipCooldown)
        {
            Flip();
            lastFlipTime = Time.time;
        }

        transform.position += new Vector3(direction * WalkSpeed * Time.deltaTime, 0, 0);
    }

    protected void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        int moveDir = directionToPlayer.x > 0 ? 1 : -1;

        if (moveDir != direction && Time.time - lastFlipTime > flipCooldown)
        {
            direction = moveDir;
            Flip();
            lastFlipTime = Time.time;
        }

        float newX = transform.position.x + direction * RunSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    protected override bool CheckInRange()
    {
        float distanceToPlayer = Vector2.Distance(groundCheck.position, player.position);
        return distanceToPlayer < detectionRange;
    }

    protected bool PlayerInAttackRange()
    {
        if (attack_Point == null || player == null) return false;
        float distanceToPlayer = Vector2.Distance(attack_Point.position, player.position);
        return distanceToPlayer < attackRadius;
    }

    protected override void Attack()
    {
        isAttacking = true;
        isChasing = false;
        lastAttackTime = Time.time;

        if (attack_Point != null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(attack_Point.position, attackRadius, playerLayer);
            DealDamage(PhysicalDame, hits);
        }
    }

    protected override void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    protected override void Die()
    {
        animator.SetTrigger("Die");
        gate.OpenGate();
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

    private void DealDamage(float damage, Collider2D[] hits)
    {
        foreach (Collider2D hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log("Boss deals damage!");
            }
        }
    }

    // ✅ NHẬN DAMAGE từ Player mà KHÔNG cần sửa Player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            TakeDamage(10f); // Sát thương cố định từ player
            Debug.Log("Boss nhận sát thương từ Player!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attack_Point != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attack_Point.position, attackRadius);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}
