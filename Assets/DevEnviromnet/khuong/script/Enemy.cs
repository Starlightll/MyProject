using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float enemyMove = 1f;
    [SerializeField] protected float distance = 5f;
    private Vector3 startPos;
    private bool moveRight = true;
    protected PlayerContronller player;

    [SerializeField] protected float hpMax = 50f;
    protected float currentHp;
    [SerializeField] private Image imageHp;
    protected Vector3 initialPosition;

    public virtual void Initialize(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        initialPosition = spawnPosition;
        gameObject.SetActive(true);
    }

    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerContronller>();
        startPos = transform.position;
        currentHp = hpMax;
        UpdateHp3();
    }

    protected virtual void Update()
    {
        Move();
    }

    protected void Move()
    {
        float leftBound = startPos.x - distance;
        float rightBound = startPos.x + distance;
        if (moveRight)
        {
            transform.Translate(Vector2.right * enemyMove * Time.deltaTime);
            if (transform.position.x >= rightBound)
            {
                moveRight = false;
                FlipEnemy();
            }
        }
        else
        {
            transform.Translate(Vector2.left * enemyMove * Time.deltaTime);
            if (transform.position.x <= leftBound)
            {
                moveRight = true;
                FlipEnemy();
            }
        }


        // if (player != null)
        // {
        //     transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyMove * Time.deltaTime);
        //     FlipEnemy();
        // }
    }

    protected void FlipEnemy()
    {
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHp3();
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        EnemySpawner.Instance.ReturnEnemyToPool(this);
    }

    protected void UpdateHp3()
    {
        if (imageHp != null)
        {
            imageHp.fillAmount = currentHp / hpMax;
        }
    }
}
