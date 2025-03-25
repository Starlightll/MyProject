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
            }

            isAttacking = true;
            isChasing = false;
            lastAttackTime = Time.time;
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

            if (!IsGroundAhead())
            {
                direction *= -1;
                return;
            }


            rb.linearVelocity = new Vector2(Mathf.Sign(directionToPlayer.x) * RunSpeed, rb.linearVelocity.y);
        }
        Debug.Log("Distant: " + distanceToPlayer);

        FaceToward(direction);
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
        EnemySpawner enemy_Pool = UnityEngine.Object.FindFirstObjectByType<EnemySpawner>();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        ResetState();
        enemy_Pool.ReturnEnemyToPool(this);
    }

    protected override void Patrol()
    {
        if (animator != null)
        {
            animator.SetInteger("State", 2);
            animator.SetBool("Action", false);
        }

        FaceToward(direction);

        if (!IsGroundAhead() || !IsGroundAhead2())
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
    private bool IsGroundAhead2()
    {
        return Physics2D.Raycast(enemyEye.position, Vector2.right, 3, groundLayer);
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


}
