using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalk : Enemy, IDamageable
{
    private Vector2 direction;
    private Vector2 MinPos;
    private Vector2 MaxPos;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform attack_Point;
    [SerializeField] private float PainAttack = 1f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float dame2 = 2;
    private int respawnCount = 0;
    private const int maxRespawnCount = 3;

    private Animator animator;


    protected override void Start()
    {
        base.Start();

        MinPos = spawnPosition - new Vector2(PatrolRange, 0);
        MaxPos = spawnPosition + new Vector2(PatrolRange, 0);
        direction = Vector2.right;
        animator = GetComponent<Animator>();

    }

    void Update()
    {

        if (PlayerInAttackRange())
        {
            Attack();
        }
        else if (CheckInRange())
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            Patrol();
        }

    }

    protected override void Attack()
    {

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (animator != null)
            {

                animator.SetTrigger("Attack");
                Collider2D[] hits = Physics2D.OverlapCircleAll(attack_Point.position, PainAttack, playerLayer);
                DealDamage(hits, PhysicalDame);
            }

            isAttacking = true;
            isChasing = false;
            lastAttackTime = Time.time;
        }
    }
    private void DealDamage(Collider2D[] hits, float damage)
    {
        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Debug.Log("Take Dame");
            }
        }

    }

    protected virtual void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;
        animator.SetInteger("State", 3);
        animator.SetBool("Action", false);


        //Distant on x axis between player and enemy
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        if (distanceToPlayer <= 1.5f)
        {
            rb.linearVelocity = Vector2.zero;
            Attack();
            return;
        }
        else if (distanceToPlayer > 1.5f)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            if (directionToPlayer.x > 0)
            {
                direction = Vector2.left;
            }
            else if (directionToPlayer.x < 0)
            {
                direction = Vector2.right;
            }

            if (!IsGroundAhead() || IsObstacleAhead())
            {
                direction *= -1;
                return;
            }


            rb.linearVelocity = new Vector2(Mathf.Sign(directionToPlayer.x) * RunSpeed, rb.linearVelocity.y);
        }
        Debug.Log("Distant: " + distanceToPlayer);

        FaceToward(direction);
    }



    bool IsObstacleAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(enemyEye.position, direction, 0.5f, groundLayer);
        return hit.collider != null;
    }



    protected virtual bool PlayerInAttackRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer < IsAttackRange;
    }
    protected override void Die()
    {
        animator.SetInteger("State", 9);
        StartCoroutine(ReturnToPoolAfterDelay());

    }
    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        ResetState();

        if (respawnCount < maxRespawnCount)
        {
            respawnCount++;
            Respawn();
        }
        else
        {
            EnemySpawner.Instance.ReturnEnemyToPool(this);
        }
    }

    private void Respawn()
    {
        transform.position = spawnPosition;
        currentHealth = Hp;
        healthBar.fillAmount = 1f;
        gameObject.SetActive(true);
        // EnemySpawner.Instance.SpawnEnemy();
    }

    protected override void Patrol()
    {
        if (animator != null)
        {
            animator.SetInteger("State", 2);
            animator.SetBool("Action", false);
        }

        FaceToward(direction);

        if (!IsGroundAhead() || IsObstacleAhead())
        {
            direction *= -1;
        }


        if (transform.position.x > MaxPos.x)
        {
            direction = Vector2.left;


        }
        else if (transform.position.x < MinPos.x)
        {
            direction = Vector2.right;

        }


        rb.linearVelocity = new Vector2(direction.x * WalkSpeed, rb.linearVelocity.y);
    }

    private bool IsGroundAhead()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.5f, groundLayer);
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            IDamageable player = collision.gameObject.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(dame2);
            }

            
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                float knockbackForce = 1000f; // Điều chỉnh lực hất ra theo ý muốn

                // Xác định hướng lùi về sau của người chơi
                Vector2 playerDirection = playerRb.linearVelocity.normalized;
                Vector2 knockbackDirectionOpposite = -playerDirection;

                playerRb.AddForce(knockbackDirectionOpposite * knockbackForce);
            }
        }
    }
}
