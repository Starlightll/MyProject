using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public float speed = 4f;
    public float chaseSpeed = 3.5f;
    public float groundCheckDistance = 1f;
    public float attackRange = 1f;
    public float detectionRange = 5f;

    public Transform groundCheck;
    public Transform player;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    private int direction = 1;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isChasing = false;
    private bool isAttacking = false;
    private float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
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

    void Patrol()
    {
        isChasing = false;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        Vector2 obstacleCheckDirection = direction > 0 ? Vector2.right : Vector2.left;
        bool isObstacleAhead = Physics2D.Raycast(transform.position, obstacleCheckDirection, 2f, groundLayer);

        if (!isGroundAhead || isObstacleAhead)
        {
            Flip();
        }

        transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
    }

    void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (directionToPlayer.x > 0 && direction < 0)
        {
            Flip();
        }
        else if (directionToPlayer.x < 0 && direction > 0)
        {
            Flip();
        }

        transform.position += new Vector3(Mathf.Sign(directionToPlayer.x) * chaseSpeed * Time.deltaTime, 0, 0);

    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            isAttacking = true;
            isChasing = false;
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    bool PlayerInRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer < detectionRange;
    }

    bool PlayerInAttackRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer < attackRange;
    }

    void Flip()
    {
        direction *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

}
