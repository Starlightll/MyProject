using System.Collections;
using System.Collections.Generic;
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
            Attack();
        }
        else
        {
            isChasing = false;

        }

        Patrol();
    }

    protected override void Attack()
    {
        if (player == null) return;

        // Enemy sẽ rượt đuổi player
        ChasePlayer();
    }

    private void ChasePlayer()
    {

        direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * RunSpeed; // Enemy di chuyển nhanh về phía player

        FaceToward(direction);

        if (Vector2.Distance(transform.position, player.position) <= AttackRange && canAttack)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        rb.linearVelocity = Vector2.zero;

        if (player != null)
        {
            FaceToward(direction);
        }

        yield return new WaitForSeconds(AttackSpeed);

        if (player != null && Vector2.Distance(transform.position, player.position) <= AttackRange)
        {
            Debug.Log("Enemy tấn công Player gây " + PhysicalDame + " sát thương!");
            // Thêm logic gây sát thương lên player nếu cần
        }

        canAttack = true;
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
