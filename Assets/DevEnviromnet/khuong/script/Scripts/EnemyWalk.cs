using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalk : Enemy
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
        throw new System.NotImplementedException();
    }

    protected override void Patrol()
    {
        if (animator != null)
        {
            animator.SetInteger("State", 3);
            animator.SetBool("Action", false);
        }

        FaceToward(direction);

        if (!IsGroundAhead())
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

    //protected override void TakeDame(float dame)
    //{
    //    throw new System.NotImplementedException();
    //}


}
