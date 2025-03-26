using UnityEngine;

public class BossKnightController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public Transform groundCheck;
    public Transform attackPoint;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public Transform player;
    private int direction = 1;
    private Animator animator;
    private bool isAttacking = false;
    private bool isDead = false;
    private Transform playerTarget;
    [SerializeField] private Transform check1;
    [SerializeField] private Transform check2;
    [SerializeField] private float checkDistance = 2f;
    // Khoảng cách để xác định 2 điểm chạm nhau
    [SerializeField] private int skill1Damage = 15;
    [SerializeField] private int skill2Damage = 30;

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

    private bool lastUsedSkill2 = false;
    private int skill1Count = 0;

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
    private void SpawnDarkBolt()
    {
        if (player != null)
        {
            Vector3 spawnPosition = new Vector3(player.position.x, player.position.y + 5f, 0);
            GameObject darkBolt = Instantiate(darkBoltPrefab, spawnPosition, Quaternion.identity);      

            Rigidbody2D rb = darkBolt.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, -12f); // Dark Bolt rơi xuống Player
            }
        }
    }
    private void Patrol()
    {
        animator.SetBool("iswalk", true);// Bật animation đi bộ
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
        if (isAttacking)
        {
            return;
        }
         // Nếu đang tấn công thì không di chuyển nữa

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

        skill1Count++; // Mỗi lần đánh Skill 1, tăng biến đếm
        Debug.Log("Skill 1 count: " + skill1Count);
        UseSkill1();
       
        if (skill1Count >= 5) // Nếu đánh đủ 5 lần thì kích hoạt Skill 2
        {
            skill1Count = 0; // Reset đếm sau khi dùng Skill 2
            UseSkill2();
        }

        Invoke("ResetAttack", 1f); // Reset attack state sau khi hoàn tất combo
    }

    private void UseSkill1()
    {
        if (isDead) return;

        isAttacking = true;
        animator.SetTrigger("skill_1"); // Gọi animation đánh thường
    }

    private void UseSkill2()
    {
        if (isDead) return;

        isAttacking = true;
        lastSkill2Time = Time.time;

        animator.SetTrigger("skill_2"); // Gọi animation Skill 2

        // Chờ animation Skill 2 kết thúc, sau đó kiểm tra vị trí check1 & check2
        Invoke("CheckAndSpawnDarkBolt", 3.5f);
    }

    private void CheckAndSpawnDarkBolt()
    {
        if (isDead) return;

        float currentDistance = Vector2.Distance(check1.position, check2.position);
        Debug.Log($"Khoảng cách giữa check1 và check2: {currentDistance} (Yêu cầu: {checkDistance})");

        if (currentDistance <= checkDistance)
        {
            Debug.Log("Spawn Dark Bolt!");
            SpawnDarkBolt();
        }
        else
        {
            Debug.Log("Không spawn Dark Bolt, khoảng cách chưa đủ!");
        }
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

    // Gọi từ Animation Event để gây sát thương đúng thời điểm
    private void ApplyDamageSkill1()
    {
        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (player != null)
        {
            IDamageable damageable = player.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(skill1Damage);
                Debug.Log("Player mất " + skill1Damage + " máu tại thời điểm chém!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, Vector2.right * direction);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(check1.position, check2.position);
    }

}
