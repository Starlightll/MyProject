using UnityEngine;

public class butterflycontroller : Enemy
{
    private Animator animator;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetBool("attack", true);
            Debug.Log("Butterfly is attacking!");
            // Thêm logic tấn công nếu cần
        }
    }

    protected override void Die()
    {
        
    }

    protected override void Patrol()
    {
        float newX = spawnPosition.x + Mathf.PingPong(Time.time * FlySpeed, PatrolRange) - (PatrolRange / 2);
        transform.position = new Vector2(newX, transform.position.y);
        animator.SetBool("isMoving", true);     
    }

    protected override void TakeDame(float dame)
    {
        Hp -= dame;
        Debug.Log($"Butterfly took {dame} damage. Remaining HP: {Hp}");

        if (Hp <= 0)
        {
            Die();
        }
    }

    void Update()
    {
        if (CheckInRange())
        {
            Attack();
        }
        else
        {
            isAttacking = false;
            animator.SetBool("attack", false);
            Patrol();
        }
    }
}
