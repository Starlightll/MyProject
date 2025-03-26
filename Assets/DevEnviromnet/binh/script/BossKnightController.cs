using UnityEngine;

public class BossKnightController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public Transform groundCheck;
    public Transform attackPoint;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    private int direction = 1;
    private Animator animator;
    private bool isAttacking = false;
    private bool isDead = false;
    private Transform playerTarget;

    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float obstacleCheckDistance = 1f;
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int health = 260;

    [SerializeField] private GameObject darkBoltPrefab;
    [SerializeField] private float skill2Cooldown = 8f;
    private float lastSkill2Time;
    private float lastAttackTime;

    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
      

        playerTarget = DetectPlayer();

        if (playerTarget != null) // Nếu phát hiện Player
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
            Collider2D player = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
            if (player != null) // Nếu Player trong phạm vi tấn công
            {
                Attack(player);
                Debug.Log(distanceToPlayer);
            }
            else // Nếu ngoài phạm vi tấn công nhưng vẫn thấy Player
            {
                ChasePlayer();
            }
        }
        else if (!isAttacking) // Nếu không thấy Player thì tuần tra
        {
            Patrol();
        }

        
    }

    private void Patrol()
    {
        animator.SetTrigger("walk"); // Bật animation đi bộ
        animator.SetBool("isrun", false); // Tắt animation chạy

        bool isGroundAhead = Physics2D.OverlapCircle(groundCheck.position, 2 , groundLayer); 
        bool isObstacleAhead = Physics2D.Raycast(groundCheck.position + new Vector3(0 ,5,0), Vector2.right * direction, obstacleCheckDistance, groundLayer);

        if (isObstacleAhead)
        {
            Flip();
        }

        rb.linearVelocity = new Vector3(direction * walkSpeed, rb.linearVelocity.y, 0);
    }

    private void Flip()
    {
        direction *= -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
    }

    private Transform DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        return player != null ? player.transform : null;
    }

    private void ChasePlayer()
    {
        if (isAttacking) return; // Nếu đang tấn công thì không di chuyển nữa

        animator.SetBool("iswalk", false);
        animator.SetBool("isrun", true);

        int moveDirection = playerTarget.position.x > transform.position.x ? 1 : -1;
        direction = moveDirection;

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
        transform.position += new Vector3(direction * chaseSpeed * Time.deltaTime, 0, 0);
    }

    private void Attack(Collider2D player)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        // Kiểm tra có đủ cooldown để dùng Skill 2 không
        bool canUseSkill2 = (Time.time - lastSkill2Time >= skill2Cooldown);
        bool useSkill2 = canUseSkill2 && (Random.value < 0.4f); // 40% cơ hội dùng Skill 2

        if (useSkill2)
        {
            UseSkill2(); // Sử dụng Dark Bolt
        }
        else
        {
            animator.SetTrigger("skill_1"); // Đánh thường
        }

        Invoke("ResetAttack", 1.5f);
    }


    private void UseSkill2()
    {
        if (isDead) return;

        isAttacking = true;
        animator.SetTrigger("skill_2");
        lastSkill2Time = Time.time;

        if (playerTarget != null)
        {
            Vector3 spawnPosition = new Vector3(playerTarget.position.x, playerTarget.position.y + 3f, 0);
            GameObject darkBolt = Instantiate(darkBoltPrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rb = darkBolt.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, -10f);
            }
        }

        Invoke("ResetAttack", 1f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log("Boss nhận " + damage + " sát thương. Máu còn: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("die");
        Debug.Log("Boss đã chết!");
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, Vector2.right * direction);
    }

}
