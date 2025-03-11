using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float speed;
    public int health;
    public int damage;
    public float moveRange = 5f; 

    protected Vector3 initialPosition; 
    protected bool movingLeft = true; 

    protected Animator animator;

    public virtual void Initialize(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        initialPosition = spawnPosition;
        gameObject.SetActive(true);
        health = 100; 

        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsRunning", false); 
        }

        StartCoroutine(DelayedMove());
    }

    private IEnumerator DelayedMove()
    {
        yield return new WaitForSeconds(1f); 
        if (animator != null) animator.SetBool("IsRunning", true); 
    }

    private void Update()
    {
        Move();
    }

    protected void Move()
    {
        if (movingLeft)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            transform.localScale = new Vector3(-1, 1, 1); 
        }
        else
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            transform.localScale = new Vector3(1, 1, 1); 
        }
        if (transform.position.x <= initialPosition.x - moveRange)
        {
            movingLeft = false;
        }
        else if (transform.position.x >= initialPosition.x + moveRange)
        {
            movingLeft = true;
        }
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
        EnemySpawner.Instance.ReturnEnemyToPool(this);
    }
}
