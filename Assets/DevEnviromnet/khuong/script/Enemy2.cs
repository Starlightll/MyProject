using UnityEngine;

public class Enemy2 : Enemy
{
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

        transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
    }


}
