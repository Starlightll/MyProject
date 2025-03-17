using UnityEngine;

public class EnemyWalk : Enemy
{
    private Vector2 direction;
    private Vector2 MinPos;
    private Vector2 MaxPos;


    protected override void Start()
    {
        base.Start();

        MinPos = spawnPosition - new Vector2(PatrolRange, 0);
        MaxPos = spawnPosition + new Vector2(PatrolRange, 0);
        direction = Vector2.right;

    }

    void Update()
    {
        if (CheckInRange())
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;

        }

        Patrol();
    }

    protected override void Attack()
    {
        throw new System.NotImplementedException();
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }

    protected override void Patrol()
    {
        FaceToward(direction);
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



    protected override void TakeDame(float dame)
    {
        throw new System.NotImplementedException();
    }


}
