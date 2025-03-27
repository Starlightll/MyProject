using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossKnightController : MonoBehaviour, IDamageable
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
    [SerializeField] private GameObject hpUI;
    [SerializeField] private Image hpBar;
    [SerializeField] private float hp = 100;
    [SerializeField] private float knockbackForce = 70f;
    [SerializeField] private int contactDamage = 5; // Sát thương khi va chạm
    private float currentHp;



    private float lastSkill2Time;
    private float lastAttackTime;

    private bool lastUsedSkill2 = false;
    private int skill1Count = 0;
    private int skill1UsageCount = 0;
    private Rigidbody2D rb;

    //audio
    [SerializeField] private AudioClip chaseSound; // Âm thanh đuổi player
    [SerializeField] private AudioClip skill1;
    [SerializeField] private AudioClip skill2;
    [SerializeField] private AudioSource audioSource;
    public GameObject wall;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHp = hp;
        audioSource = GetComponent<AudioSource>();
        hpUI.SetActive(false); // Fix the error by removing GameObject method call
       
    }

    private void Update()
    {

        playerTarget = DetectPlayer();

        if (playerTarget != null) // Nếu phát hiện Player
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer < detectionRange) // Nếu Player ở gần, hiện thanh máu
            {
                hpUI.SetActive(true);
            }
            else
            {
                hpUI.SetActive(false);
            }
            Collider2D player = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
            if (player != null) // Nếu Player trong phạm vi tấn công
            {
                Attack(player);

            }
            else // Nếu ngoài phạm vi tấn công nhưng vẫn thấy Player
            {
                audioSource.volume = 0.05f;
                PlaySound(chaseSound);
                ChasePlayer();
            }
        }
        else if (!isAttacking) // Nếu không thấy Player thì tuần tra
        {
            hpUI.SetActive(false);
            Patrol();
        }

        if (isDead)
        {
            hpUI.SetActive(false);
        }

    }
    private void SpawnDarkBolt()
    {
        if (player != null)
        {
            Vector3 spawnPosition = new Vector3(player.position.x, player.position.y + 18f, 0);
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

        bool isGroundAhead = Physics2D.OverlapCircle(groundCheck.position, 2, groundLayer);
        bool isObstacleAhead = Physics2D.Raycast(groundCheck.position + new Vector3(0, 5, 0), Vector2.right * direction, obstacleCheckDistance, groundLayer);

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
    public void PlaySkillSound()
    {
        audioSource.volume = 1f;
        PlaySound(skill1);
    }

    public void PlaySkill2Sound()
    {
        audioSource.volume = 1f;
        PlaySound(skill2);
    }

    private void Attack(Collider2D player)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        skill1Count++;
        skill1UsageCount++;// Mỗi lần đánh Skill 1, tăng biến đếm
        Debug.Log("Skill 1 count: " + skill1Count);
        Debug.Log(skill1UsageCount);
        UseSkill1();
        if (skill1Count >= 3) // Nếu đánh đủ 5 lần thì kích hoạt Skill 2
        {
            skill1Count = 0; // Reset đếm sau khi dùng Skill 2
            UseSkill2();
        }
        if (skill1UsageCount >= 2) // Nếu đã dùng Skill 1 hai lần thì dùng Skill 3
        {
            skill1UsageCount = 0; // Reset đếm sau khi dùng Skill 3
            UseSkill3();
        }
        Invoke("ResetAttack", 1f); // Reset attack state sau khi hoàn tất combo
    }
   

    private void UseSkill1()
    {
        if (isDead) return;

        isAttacking = true;
        animator.SetTrigger("skill_1");
        
        // Gọi animation đánh thường
    }
    private void UseSkill3()
    {
        if (isDead) return;

        isAttacking = true;
        animator.SetTrigger("evade_1");// Gọi animation Skill 3 (lộn)

        float rollForce = 30f; // Điều chỉnh lực lộn
        float rollDuration = 0.6f; // Thời gian chờ trước khi reset trạng thái

        // Áp dụng lực đẩy theo hướng Boss đang nhìn
        rb.linearVelocity = new Vector2(direction * rollForce, 0);

        // Chờ một khoảng thời gian rồi cho phép di chuyển lại
        Invoke("ResetAttack", rollDuration);
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



    private void Die()
    {
        isDead = true;
        animator.SetTrigger("death");

        Debug.Log("Boss đã chết!");
        Destroy(gameObject, 2f);
        gate gate = FindFirstObjectByType<gate>();
        if (gate != null)
        {
            gate.CloseGate();
        }
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
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable player = collision.gameObject.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(contactDamage); // Gây sát thương lên Player
            }

            // Đẩy Player ra xa
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        string hitAnimation = Random.value > 0.5f ? "hit_1" : "hit_2";
        animator.SetTrigger(hitAnimation);
        Debug.Log("Boss nhận " + damage + " sát thương. Máu còn: " + health);
        currentHp -= damage;
        hpBar.fillAmount = (float)currentHp / hp;
        if (currentHp <= 0)
        {

            Die();
        }
    }
}
