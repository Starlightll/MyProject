using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float enemyMove = 1f;
    [SerializeField] protected float distance = 5f;
    protected Vector3 startPos;
    protected PlayerContronller player;
    protected Rigidbody2D rb;
    protected Animator animator;

    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float chaseSpeed = 5f;
    [SerializeField] protected float groundCheckDistance = 1f;
    [SerializeField] protected float attackRange = 1f;
    protected float detectionRange = 10f;

    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask playerLayer;

    [SerializeField] protected int direction = 1;
    protected bool isChasing = false;
    protected bool isAttacking = false;
    protected float attackCooldown = 0.5f;
    protected float lastAttackTime = 0f;
    private bool isRange = false;

    [SerializeField] protected float hpMax = 50f;
    protected float currentHp;
    [SerializeField] private Image imageHp;
    protected Vector3 initialPosition;

    public virtual void Initialize(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        initialPosition = spawnPosition;
        gameObject.SetActive(true);
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetInteger("State", 3);
            animator.SetBool("Action", false);
        }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPos = transform.position;
        currentHp = hpMax;
        UpdateHpBar();

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                // Debug.Log("Enemy đã tìm thấy Player!");
            }
            else
            {
                // Debug.LogError("Không tìm thấy GameObject có tag 'Player'!");
            }
        }
    }


    protected virtual void Update()
    {
        if (playerTransform == null) return;
        GameObject playerObj = GameObject.FindWithTag("Player");
        playerTransform = playerObj.transform;
        Debug.Log(isRange);

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);


        if (PlayerInAttackRange())
        {
            AttackPlayer();
        }
        else if (PlayerInRange())
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    protected virtual void Patrol()
    {
        isChasing = false;
        isAttacking = false;
        animator.SetInteger("State", 3);
        animator.SetBool("Action", false);

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        Vector2 obstacleCheckDirection = direction > 0 ? Vector2.right : Vector2.left;
        bool isObstacleAhead = Physics2D.Raycast(transform.position, obstacleCheckDirection, 2f, groundLayer);

        if (!isGroundAhead || isObstacleAhead)
        {
            FlipEnemy();
        }

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    protected virtual void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Kiểm tra hướng đi và flip nếu cần
        if ((directionToPlayer.x > 0 && direction < 0) || (directionToPlayer.x < 0 && direction > 0))
        {
            FlipEnemy();
        }

        // Enemy di chuyển về phía player
        // transform.position += new Vector3(directionToPlayer.x * chaseSpeed * Time.deltaTime, 0, 0);
        rb.linearVelocity = new Vector2(directionToPlayer.x * chaseSpeed, 0);
        // Debug.Log("Enemy đang đuổi theo player...");
    }

    protected void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            isAttacking = true;
            isChasing = false;
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
            Debug.Log("Enemy tấn công Player!");
        }
    }

    protected bool PlayerInRange()
    {
        // if (playerTransform == null) return false;
        // bool inRange = Vector2.Distance(transform.position, playerTransform.position) < detectionRange;

        // Debug.Log("Player trong phạm vi phát hiện: " + inRange);
        // return inRange;

        isRange = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        return isRange;


    }




    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("Player"))
    //     {
    //         isRange = true;
    //         Debug.Log("Player đã vào ph ạm vi phát hiện của Enemy!");


    //     }
    // }

    // void OnTriggerExit2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("Player"))
    //     {
    //         isRange = false;
    //     }
    // }

    protected bool PlayerInAttackRange()
    {
        if (playerTransform == null) return false;
        bool inAttackRange = Vector2.Distance(transform.position, playerTransform.position) < attackRange;
        // Debug.Log("Player trong phạm vi tấn công: " + inAttackRange);
        return inAttackRange;
    }

    protected virtual void FlipEnemy()
    {
        direction *= -1;
        transform.localScale = new Vector3(direction, 1, 1);
    }

    protected void UpdateHpBar()

    {
        if (imageHp != null)
        {
            imageHp.fillAmount = currentHp / hpMax;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (playerTransform.position - transform.position) * attackRange);

    }
}
