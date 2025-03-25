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

    private int direction = 1;
    private Animator animator;
    private GateController gate;

    [SerializeField] private float leftLimit = -5f;
    [SerializeField] private float rightLimit = 5f;

    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();

        // Check if player exists and is correctly assigned
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // If player is not found, log an error
        if (player == null)
        {
            Debug.LogError("Player object not found! Please assign the player in the scene.");
            return;
        }

        gate = FindAnyObjectByType<GateController>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayerX = Mathf.Abs(player.position.x - transform.position.x);

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
                    Attack(); // Tấn công Player khi ở trong phạm vi
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
        Vector2 obstacleCheckDirection = direction > 0 ? Vector2.right : Vector2.left;
        bool isObstacleAhead = Physics2D.Raycast(groundCheck.position, obstacleCheckDirection, 6f, groundLayer);

        if (!isGroundAhead || isObstacleAhead)
        {
            Flip();
        }

        // Move continuously between leftLimit and rightLimit
        float currentPositionX = transform.position.x;
        if (currentPositionX <= leftLimit || currentPositionX >= rightLimit)
        {
            Flip();
        }

        // Move Boss back and forth between leftLimit and rightLimit
        transform.position += new Vector3(direction * WalkSpeed * Time.deltaTime, 0, 0);
    }

    protected void ChasePlayer()
    {
        if (player == null) return;

        isChasing = true;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        int moveDir = directionToPlayer.x > 0 ? 1 : -1;

        // If Player is to the left of Boss, Boss should flip towards the left, and vice versa.
        if (moveDir != direction && Time.time - lastFlipTime > flipCooldown)
        {
            direction = moveDir;
            Flip();
            lastFlipTime = Time.time;
        }

        // Move towards the player
        transform.position += new Vector3(Mathf.Sign(directionToPlayer.x) * RunSpeed * Time.deltaTime, 0, 0);
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

        // Kích hoạt animation tấn công
        animator.SetTrigger("Attack");  // Đảm bảo rằng "Attack" trigger đã được thiết lập trong Animator

        // Kiểm tra va chạm với Player trong phạm vi tấn công khi Boss tấn công
        if (attack_Point != null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(attack_Point.position, attackRadius, playerLayer);

            if (hits.Length > 0)  // Nếu có Player trong phạm vi tấn công
            {
                foreach (Collider2D hit in hits)
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(PhysicalDame);  // Gọi TakeDamage trên Player để giảm máu
                        Debug.Log("Boss deals damage to Player!");
                    }
                }
            }
        }
    }

    protected override void Flip()
    {
        direction *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    protected override void Die()
    {
        animator.SetTrigger("Die");
        gate.OpenGate();
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
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
