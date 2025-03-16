using UnityEngine;

public class Enemy1 : Enemy, EnemyDame
{

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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDame();
        }
    }
}