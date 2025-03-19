using UnityEngine;
using UnityEngine.UI;

public class Skeleton : Enemy, IDamageable
{
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Image healthBar;
    private int direction = 1;
    private Animator animator;
    private bool isAttacking = false;
    private bool is_Chasing = false;
    private float lastAttackTime = 0f;
    private float currentHealth = 100;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
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
        is_Chasing = false;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        Vector2 obstacleCheckDirection = direction > 0 ? Vector2.right : Vector2.left;
        bool isObstacleAhead = Physics2D.Raycast(transform.position, obstacleCheckDirection, 2f, groundLayer);

        if (!isGroundAhead || isObstacleAhead)
        {
            Flip();
        }

        transform.position += new Vector3(direction * WalkSpeed * Time.deltaTime, 0, 0);
    }

    protected override bool CheckInRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer < detectionRange;
    }

    protected virtual void ChasePlayer()
    {
        is_Chasing = true;
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

        transform.position += new Vector3(Mathf.Sign(directionToPlayer.x) * RunSpeed * Time.deltaTime, 0, 0);

    }

    private bool PlayerInAttackRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer < attackRange;
    }

    protected override void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            isAttacking = true;
            isChasing = false;
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;

            //Invoke(nameof(DealDamage), 0.3f);
        }
    }
    protected override void Flip()
    {
        direction *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }


    protected override void Die()
    {
        throw new System.NotImplementedException();
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth/Hp;
        if (Hp <= 0)
        {
            Die();
        }
    }

    //private void DealDamage()
    //{
    //    float distanceToPlayer = Vector2.Distance(transform.position, player.position);
    //    if (distanceToPlayer <= attackRange)
    //    {
    //        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
    //        if (playerHealth != null)
    //        {
    //            playerHealth.TakeDamage(attackDamage);
    //        }
    //    }
    //}
}
