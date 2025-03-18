using UnityEngine;

public class Boss : GroundEnemy
{
    //public Transform rootTransform;


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

        transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
    }

    protected override bool PlayerInRange()
    {
        float distanceToPlayer = Vector2.Distance(groundCheck.position, player.position);
        return distanceToPlayer < detectionRange;
    }

    protected override bool PlayerInAttackRange()
    {
        float distanceToPlayer = Vector2.Distance(groundCheck.position, player.position);
        return distanceToPlayer >= (attackRange - 2) && distanceToPlayer <= (attackRange + 2);
    }

    protected override void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (Mathf.Abs(directionToPlayer.x) > 0.1f)
        {
            if ((directionToPlayer.x > 0 && direction < 0) || (directionToPlayer.x < 0 && direction > 0))
            {
                Flip();
            }
        }

         transform.position += new Vector3(Mathf.Sign(directionToPlayer.x) * chaseSpeed * Time.deltaTime, 0, 0);
        
    }

    protected override void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            isAttacking = true;
            isChasing = false;
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
            
        }
    }


}
