using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Di chuyển")]
    public float minPatrolDistance = 2f;
    public float maxPatrolDistance = 6f;
    public float patrolHeightVariation = 3f;
    public float moveSpeed = 2f;
    private Vector2 patrolTarget;
    private bool isMovingToTarget = false;
    private bool isFacingRight = true;
    private Vector2 startPosition;
    private Rigidbody2D rb;

    [Header("Phạm vi & Phát hiện")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;

    [Header("Tấn công")]
    public Transform player;
    public float attackRange = 3f;
    public float diveAttackRange = 5f;
    public float diveSpeed = 10f;
    private bool isDiving = false;
    private bool isAttacking = false;

    [Header("Tăng cường khi mất máu")]
    public float enragedThreshold = 0.3f;
    public float enragedSpeedMultiplier = 1.5f;
    private bool isEnraged = false;

    private float health = 100;
    private float maxHealth = 100;

    [Header("Điểm tấn công")]
    public Transform attackPoint;
    public float PainAttack = 1f;
    public int attackDamage = 20;
    public LayerMask playerLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetNextPatrolTarget();
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!isDiving && !isAttacking)
        {
            if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(AttackPlayer());
            }
            else if (distanceToPlayer <= diveAttackRange)
            {
                StartCoroutine(DiveAttack());
            }
            else
            {
                RandomPatrol();
            }
        }

        if (!isEnraged && health / maxHealth <= enragedThreshold)
        {
            EnrageMode();
        }
    }

    // 🎯 **Di chuyển tuần tra**
    private void RandomPatrol()
    {
        if (!isMovingToTarget || Vector2.Distance(transform.position, patrolTarget) < 0.5f || IsObstacleAhead())
        {
            SetNextPatrolTarget();
        }

        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        FlipBoss(direction.x);
    }

    // 📌 **Xác định điểm đến ngẫu nhiên**
    private void SetNextPatrolTarget()
    {
        float patrolDistance = Random.Range(minPatrolDistance, maxPatrolDistance);
        float heightOffset = Random.Range(-patrolHeightVariation, patrolHeightVariation);

        isFacingRight = !isFacingRight; // Đảo hướng di chuyển

        patrolTarget = startPosition + new Vector2(isFacingRight ? patrolDistance : -patrolDistance, heightOffset);
        isMovingToTarget = true;
    }

    // ⚠️ **Phát hiện va chạm với Ground để quay đầu**
    private bool IsObstacleAhead()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }




    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        // 🏴‍☠️ Ở đây có thể thêm animation tấn công
        Collider2D playerHit = Physics2D.OverlapCircle(attackPoint.position, PainAttack, playerLayer);

        if (playerHit != null)
        {

            Debug.Log("Player bị trúng đòn!");
        }
        yield return new WaitForSeconds(1f); // Giả lập thời gian ra đòn
        isAttacking = false;
    }

    // ⚡ **Lao tới tấn công**
    private IEnumerator DiveAttack()
    {
        isDiving = true;

        Vector2 direction = new Vector2(player.position.x, player.position.y) - (Vector2)transform.position;
        direction.Normalize();
        FlipBoss(direction.x);
        rb.linearVelocity = direction * diveSpeed;

        yield return new WaitForSeconds(0.5f); // Giữ tốc độ trong 0.5s
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);

        isDiving = false;
    }

    // 🔥 **Tăng cường khi Boss sắp chết**
    private void EnrageMode()
    {
        isEnraged = true;
        moveSpeed *= enragedSpeedMultiplier;
        diveSpeed *= enragedSpeedMultiplier;
    }

    private void FlipBoss(float directionX)
    {
        if ((directionX > 0 && transform.localScale.x < 0) || (directionX < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (rb != null) // Đảm bảo Rigidbody2D đã được gán
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rb.position, attackRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rb.position, diveAttackRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, PainAttack);
        }
    }

}
