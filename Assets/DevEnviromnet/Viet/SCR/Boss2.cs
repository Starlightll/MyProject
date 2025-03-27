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

    private int direction = 1; // 1 means moving right, -1 means moving left
    private Animator animator;
    private GateController gate;

    [SerializeField] private float leftLimit = -5f;
    [SerializeField] private float rightLimit = 5f;

    public GameObject bulletPrefab;  // Prefab của viên đạn (BanDan)
    public Transform firePoint;      // Điểm phát đạn
    public float bulletSpeed = 10f;  // Tốc độ của viên đạn

    [SerializeField] private AudioSource audioSource;  // AudioSource để phát âm thanh
    [SerializeField] private AudioClip attackSound;    // Âm thanh tấn công
    [SerializeField] private AudioClip dieSound;       // Âm thanh chết

    // Patrol distance and position tracking
    private float patrolDistance = 5f;  // Set how far the boss should move before flipping
    private float patrolStartPosX;      // Store the starting X position for the patrol

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

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
                Patrol();  // Điều khiển di chuyển qua lại
            }
        }
    }

    protected override void Patrol()
    {
        isChasing = false;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        // If patrol hasn't started, initialize the starting position
        if (patrolStartPosX == 0)
        {
            patrolStartPosX = transform.position.x;
        }

        // Check how far the boss has moved since starting the patrol
        float distanceTraveled = Mathf.Abs(transform.position.x - patrolStartPosX);

        // If the boss has traveled the patrol distance, flip direction and set a new start position
        if (distanceTraveled >= patrolDistance)
        {
            Flip();  // Flip direction
            patrolStartPosX = transform.position.x;  // Reset patrol start position after flipping
        }

        // Move the boss smoothly back and forth based on the direction
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

        if (moveDir != direction && Time.time - lastFlipTime > flipCooldown)
        {
            direction = moveDir;
            Flip();
            lastFlipTime = Time.time;
        }

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
        animator.SetTrigger("Attack");

        // Play attack sound
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

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

        // Call the Shoot method to shoot a bullet
        Shoot();
    }

    // Shoot method for firing a bullet towards the player
    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || player == null)
        {
            Debug.LogError("Thiếu giá trị: Bullet, FirePoint hoặc Player chưa được gán!");
            return;
        }

        // Tạo viên đạn tại firePoint
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        BanDan bulletComponent = newBullet.GetComponent<BanDan>();  // Dùng script BanDan cho bullet

        if (bulletComponent != null)
        {
            // Căn chỉnh rotation của viên đạn sao cho hướng về player (bắn thẳng về phía người chơi)
            Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            newBullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Cài đặt tốc độ và hướng di chuyển cho viên đạn
            bulletComponent.SetTarget(player);  // Set the player as the target for the bullet
            bulletComponent.SetSpeed(bulletSpeed);  // Set the bullet speed

            // Đảm bảo viên đạn bay thẳng, chỉ di chuyển theo trục X
            Vector3 bulletPosition = newBullet.transform.position;
            bulletPosition.y = firePoint.position.y; // Đảm bảo Y không thay đổi, viên đạn bay thẳng

            newBullet.transform.position = bulletPosition;

            Debug.Log("Boss đã bắn đạn!");
        }
        else
        {
            Debug.LogError("Lỗi: Prefab đạn không có script BanDan!");
        }
    }

    protected override void Flip()
    {
        // Reverse the direction on the X-axis
        direction *= -1;

        // Flip the sprite on the X-axis only (not Y-axis)
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // Flip the health bar to match the boss's new direction (only change X scale)
        healthBar.transform.localScale = new Vector3(-healthBar.transform.localScale.x, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    protected override void Die()
    {
        animator.SetTrigger("Die");

        // Play death sound
        if (audioSource != null && dieSound != null)
        {
            audioSource.PlayOneShot(dieSound);
        }

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
