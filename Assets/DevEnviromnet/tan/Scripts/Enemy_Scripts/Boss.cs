using System.Collections;
using UnityEngine;

public class Boss : Enemy, IDamageable
{
    public Transform attack_Point;
    public float attackRadius = 2f;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Transform groundCheck;
    // [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    private int direction = 1;
    private Animator animator;
    // private bool isAttacking = false;
    private bool is_Chasing = false;
    private GateController gate;
    // private float lastAttackTime = 0f;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        gate = FindAnyObjectByType<GateController>();
    }
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(groundCheck.position, player.position);

        float distanceToPlayerX = Mathf.Abs(player.position.x - transform.position.x);


        if (distanceToPlayerX < 0.5f)
        {
            isChasing = false;
            isAttacking = false;
            animator.SetBool("isWalking", false);
            return;
        }

        if (PlayerInAttackRange())
        {
            Attack();
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

        transform.position += new Vector3(direction * WalkSpeed * Time.deltaTime, 0, 0);
    }

    protected override bool CheckInRange()
    {
        float distanceToPlayer = Vector2.Distance(groundCheck.position, player.position);
        return distanceToPlayer < detectionRange;
    }

    protected bool PlayerInAttackRange()
    {
        float distanceToPlayer = Vector2.Distance(attack_Point.position, player.position);
        return distanceToPlayer < attackRadius;
    }

    protected void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if ((directionToPlayer.x > 0 && direction < 0) || (directionToPlayer.x < 0 && direction > 0))
            {
                Flip();
            }

         transform.position += new Vector3(Mathf.Sign(directionToPlayer.x) * RunSpeed * Time.deltaTime, 0, 0);
        
    }

    protected override void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            isAttacking = true;
            isChasing = false;
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
            
        }
    }

    private void OnDrawGizmos()
    {
        if (attack_Point != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attack_Point.position, attackRadius);
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
        Enemy_Pool enemy_Pool = Object.FindFirstObjectByType<Enemy_Pool>();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        ResetState();
        enemy_Pool.ReturnToPool(gameObject);
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
}
