using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkeletonGreen : Enemy, IDamageable
{
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float experience;

    private int direction = 1;
    private Animator animator;

    // private bool isAttacking = false;
    private bool is_Chasing = false;
    private PlayerController playerController;
    // private float lastAttackTime = 0f;
    // private float currentHealth = 100;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerController = GetComponent<PlayerController>();
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
            audioSource.PlayOneShot(attackSound);
            audioSource.PlayOneShot(attackSound);

            Invoke(nameof(DealDamage), delay);
        }
    }
    protected override void Flip()
    {
        direction *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        healthBar.transform.localScale = new Vector3(-healthBar.transform.localScale.x, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }


    protected override void Die()
    {
        animator.SetTrigger("Die");
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
        animator.SetTrigger("Hurt");
        Invoke(nameof(ResetHurt), 0.3f);
    }

    void ResetHurt()
    {
        animator.ResetTrigger("Hurt");
        animator.SetBool("isHurt", false);
    }


    private void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, 2, playerLayer);
        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(PhysicalDame);
                Debug.Log("Take Dame");
            }
        }

    }

    private void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, 1);
        }
    }

}
