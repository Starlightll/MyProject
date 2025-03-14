using UnityEngine;

public class BatController : MonoBehaviour
{
    public float detectionRange = 5f;
    public float attackRange = 4f;
    public float flySpeed = 6f;
    public float idleFlyRadius = 3f;

    public Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    private bool isAwake = false;
    private bool isAttacking = false;
    private Vector2 idleTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        

        
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            if (!isAwake)
            {
                WakeUp();
            }

            if (distanceToPlayer < attackRange)
            {
                AttackPlayer();
            }
            else
            {
                FlyAround();
            }
        }
        else
        {
            FlyAround();
        }
    }

    void WakeUp()
    {
        isAwake = true;
        animator.SetTrigger("WakeUp");
    }

    void AttackPlayer()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }


        Vector2 direction = (player.position - transform.position).normalized;

        rb.MovePosition(rb.position + direction * flySpeed * Time.deltaTime);
    }

    void FlyAround()
    {
        isAttacking = false;
        animator.SetBool("isRuning", false);

        if (Vector2.Distance(transform.position, idleTarget) < 0.5f)
        {
            idleTarget = (Vector2)transform.position + Random.insideUnitCircle * idleFlyRadius;
        }


        Vector2 direction = (player.position - transform.position).normalized;

        rb.MovePosition(rb.position + direction * flySpeed * Time.deltaTime);
    }
}
