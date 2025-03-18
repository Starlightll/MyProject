using UnityEngine;

public abstract class FlyingEnemy : MonoBehaviour
{
    public float detectionRange = 5f;
    public float attackRange = 3f;
    public float flySpeed = 6f;
    public float minPatrolDistance = 5f;
    public float maxPatrolDistance = 10f;
    public float patrolHeightVariation = 2f;
    public float attackCooldown = 1f;
    public LayerMask obstacleLayer;

    Transform player;
    private Animator animator;

    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private Vector2 patrolTarget;
    private bool movingRight = true;
    private Vector2 startPos;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        startPos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetNextPatrolTarget();
    }

    protected virtual void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isAttacking && distanceToPlayer > attackRange)
        {
            isAttacking = false;
            animator.ResetTrigger("Attack");
        }

        if (distanceToPlayer < detectionRange)
        {
            if (distanceToPlayer < attackRange && Time.time - lastAttackTime > attackCooldown)
            {
                AttackPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    protected virtual void AttackPlayer()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }

    protected virtual void ChasePlayer()
    {
        isAttacking = false;
        animator.SetBool("isRuning", true);

        Vector2 direction = (player.position - transform.position).normalized;
        Flip(direction.x);
        transform.position += (Vector3)direction * flySpeed * 5 * Time.deltaTime;
    }

    protected virtual void Patrol()
    {
        if (isAttacking) return;

        animator.SetBool("isRuning", true);

        if (Vector2.Distance(transform.position, patrolTarget) < 0.5f || IsObstacleAhead())
        {
            SetNextPatrolTarget();
        }

        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        Flip(direction.x);
        transform.position += (Vector3)direction * flySpeed * Time.deltaTime;

    }

    protected virtual void SetNextPatrolTarget()
    {
        float patrolDistance = Random.Range(minPatrolDistance, maxPatrolDistance);
        float heightOffset = Random.Range(-patrolHeightVariation, patrolHeightVariation);
        movingRight = !movingRight;

        patrolTarget = startPos + new Vector2(movingRight ? patrolDistance : -patrolDistance, heightOffset);
    }

    bool IsObstacleAhead()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2f, obstacleLayer);
        return hit.collider != null;
    }

    void Flip(float directionX)
    {
        if ((directionX > 0 && transform.localScale.x < 0) || (directionX < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
