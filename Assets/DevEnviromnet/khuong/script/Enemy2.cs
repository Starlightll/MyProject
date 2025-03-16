using UnityEngine;

public class Enemy2 : Enemy
{
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 0;
    }


    protected override void Patrol()
    {
        isChasing = false;
        isAttacking = false;
        animator.SetInteger("State", 3);
        animator.SetBool("Action", false);


        Vector2 obstacleCheckDirection = direction > 0 ? Vector2.right : Vector2.left;
        bool isObstacleAhead = Physics2D.Raycast(transform.position, obstacleCheckDirection, 2f, groundLayer);


        float leftBound = startPos.x - distance;
        float rightBound = startPos.x + distance;

        if (isObstacleAhead || transform.position.x <= leftBound || transform.position.x >= rightBound)
        {
            FlipEnemy();
        }

        // transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    protected override void ChasePlayer()
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




}
