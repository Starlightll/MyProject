using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour, IDamageable
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
    private bool isDie = false;

    [Header("Phạm vi & Phát hiện")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;
    public GameObject hpUI;
    public float checkPlayerDistanceSound;

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

    private float currentHealth;
    [SerializeField] private float Hp = 100;
    [SerializeField] private Image healthBar;

    [Header("Điểm tấn công")]
    public Transform attackPoint;
    public float PainAttack = 1f;
    public int attackDamage = 20;
    public LayerMask playerLayer;

    private Animator animator;
    private int attackCount = 0;
    public GameObject effectFire;
    public Transform attackPoint2;
    public Transform attackPoint3;
    public float checkPlayerDistance;

     [Header("Audio")]
     public AudioSource audioSource;
     public AudioClip checkPlayerSound;
    public AudioClip attackSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        SetNextPatrolTarget();
        effectFire.SetActive(false);
        currentHealth = Hp;
        UpdateHp();
        hpUI.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (player == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= checkPlayerDistance)
        {
          
            hpUI.SetActive(true);
        }
        if(isDie){
            hpUI.SetActive(false);
        }
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

        if (!isEnraged && currentHealth / Hp <= enragedThreshold)
        {
            EnrageMode();
        }
        if(distanceToPlayer <= checkPlayerDistanceSound)
        {
            if (audioSource != null && checkPlayerSound != null)
            {
                audioSource.volume = 0.1f; 
                audioSource.PlayOneShot(checkPlayerSound);
            }
        }
    }

    //Di chuyển
    private void RandomPatrol()
    {

        if (animator != null)
        {
            animator.SetTrigger("walk");
        }
        if (!isMovingToTarget || Vector2.Distance(transform.position, patrolTarget) < 0.5f || IsObstacleAhead())
        {
            SetNextPatrolTarget();
        }

        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        FlipBoss(direction.x);
    }

    // Xác định điểm đến ngẫu nhiên**
    private void SetNextPatrolTarget()
    {
        float patrolDistance = Random.Range(minPatrolDistance, maxPatrolDistance);
        float heightOffset = Random.Range(-patrolHeightVariation, patrolHeightVariation);

        isFacingRight = !isFacingRight;

        patrolTarget = startPosition + new Vector2(isFacingRight ? patrolDistance : -patrolDistance, heightOffset);
        isMovingToTarget = true;
    }

    //Phát hiện va chạm với Ground để quay đầu
    private bool IsObstacleAhead()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.right, groundCheckDistance, groundLayer);
    }



    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            if (attackCount < 5)
            {
                animator.SetTrigger("skill_1");
                attackCount++;
            }
            else
            {
                animator.SetTrigger("skill_2");
                effectFire.transform.position = attackPoint3.position;
                Vector2 directionToPlayer = (player.position - effectFire.transform.position).normalized;

                // Xoay effectFire để hướng về phía người chơi
                float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
                effectFire.transform.rotation = Quaternion.Euler(0, 0, angle);
                effectFire.SetActive(true);
                attackCount = 0;

                yield return new WaitForSeconds(1f);
                effectFire.SetActive(false);
            }
        }

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    public void AudioAttack()
    {
       
          audioSource.volume = 0.5f; // Giảm âm lượng
        audioSource.PlayOneShot(attackSound);
        
    }

  

    public void AttackPlayer2()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(attackPoint.position, PainAttack, playerLayer);
        Collider2D playerHit2 = Physics2D.OverlapCircle(attackPoint2.position, PainAttack, playerLayer);
       
        if (playerHit != null || playerHit2 != null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, PainAttack, playerLayer);
            DealDamage(1, hits);
            Debug.Log("Player bị trúng đòn!");
        }
    }




    // Lao tới tấn công**
    private IEnumerator DiveAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("run");
        }
        isDiving = true;

        Vector2 direction = new Vector2(player.position.x, player.position.y) - (Vector2)transform.position;
        direction.Normalize();
        FlipBoss(direction.x);
        rb.linearVelocity = direction * diveSpeed;

        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = Vector2.zero;


        isDiving = false;
    }

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
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rb.position, attackRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rb.position, diveAttackRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, PainAttack);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint2.position, PainAttack);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(attackPoint3.position, PainAttack);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rb.position, checkPlayerDistanceSound);
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(attackPoint3.position, checkPlayerDistance);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("hit_2");
        UpdateHp();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void DealDamage(float dame, Collider2D[] hits)
    {
        //Collider2D[] hits = Physics2D.OverlapCircleAll(attack_Point.position, attackRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(dame);
                Debug.Log("Take Dame");
                return;
            }
        }

    }


    public void UpdateHp()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / Hp;
        }
    }

    
    public void Die()
    {
        isDie = true;
        animator.SetTrigger("death");
        Destroy(gameObject, 3f);
        rb.linearVelocity = Vector2.zero;
        effectFire.SetActive(false);
        
    }
}
