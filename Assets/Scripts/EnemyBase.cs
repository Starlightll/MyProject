using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float speed;
    public int health;
    public int damage;
    public float moveRange = 5f;
    public float detectionRange = 5f;
    private float attackRange = 5.5f;
    public float attackCooldown = 1f;

    protected Vector3 initialPosition;
    protected bool movingLeft = true;
    protected Animator animator;
    protected Transform player;
    protected bool isChasing = false;
    protected bool canAttack = true;
    private bool isReturning = false;

    private Rigidbody2D rb;
    private CircleCollider2D detectionCollider;

    private Vector2 direction;

    public virtual void Initialize(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        initialPosition = spawnPosition;
        gameObject.SetActive(true);
        health = 100;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        SetupDetectionCollider();

        if (animator != null)
        {
            animator.SetInteger("State", 3);
            animator.SetBool("Action", false);
        }
    }

    private void SetupDetectionCollider()
    {
        detectionCollider = GetComponent<CircleCollider2D>();
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        }

        detectionCollider.radius = detectionRange;
        detectionCollider.isTrigger = true;
        detectionCollider.offset = Vector2.zero;
    }

    private void Update()
    {
        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else if (isReturning)
        {
            ReturnToInitialPosition();
        }
        else
        {
            Move();
        }
    }

    protected void Move()
    {
        float moveStep = speed * Time.deltaTime;

        // Cập nhật hướng quay mặt enemy
        transform.localScale = new Vector3(movingLeft ? 1 : -1, 1, 1);

        transform.Translate(Vector2.right * (movingLeft ? -moveStep : moveStep));

        if (transform.position.x <= initialPosition.x - moveRange)
        {
            movingLeft = false;
        }
        else if (transform.position.x >= initialPosition.x + moveRange)
        {
            movingLeft = true;
        }
    }

    private void ChasePlayer()
    {
        float moveStep = 3 * speed * Time.deltaTime;
        direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * (moveStep * 50);

        // Cập nhật hướng enemy quay mặt về phía player
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (animator != null)
        {
            animator.SetInteger("State", 3);
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange && canAttack)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Cập nhật hướng enemy quay mặt về phía player
        if (player != null)
        {
            if (transform.position.x > player.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        yield return new WaitForSeconds(attackCooldown);

        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Enemy tấn công Player gây " + damage + " sát thương!");
        }

        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        // EnemySpawner.Instance.ReturnEnemyToPool(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isReturning = false;
            player = other.transform;
            isChasing = true;

            Debug.Log("Player đã vào vùng phát hiện");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            isReturning = true;
            Debug.Log("Player đã rời khỏi vùng phát hiện");
        }
    }

    private void ReturnToInitialPosition()
    {
        float moveStep = speed * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, initialPosition, moveStep);

        // Xác định hướng quay mặt về vị trí ban đầu
        if (transform.position.x > initialPosition.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (Vector2.Distance(transform.position, initialPosition) <= 0.1f)
        {
            isReturning = false;
            player = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (direction != Vector2.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, direction * 10);
        }
    }
}
