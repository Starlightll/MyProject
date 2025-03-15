using UnityEngine;

public class Enemy1 : Enemy, EnemyDame
{


    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 1;
    }

    public void Die()
    {
        Debug.Log("Enemy đã chết!");
        EnemySpawner.Instance.ReturnEnemyToPool(this);
    }

    public void TakeDame(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected override void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;
        animator.SetInteger("State", 2);
        animator.SetBool("Action", true);

        if (transform.position.x < playerTransform.position.x)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        // transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDame();
        }
    }
}