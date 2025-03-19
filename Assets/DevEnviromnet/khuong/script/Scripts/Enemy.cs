using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{

    [SerializeField] protected float WalkSpeed = 2f;
    [SerializeField] protected float RunSpeed = 4f;
    [SerializeField] protected float FlySpeed = 5f;
    [SerializeField] protected float Hp = 100f;
    [SerializeField] protected float Mana = 100f;
    [SerializeField] protected float Defend = 0;
    [SerializeField] protected float PhysicalDame = 0;
    [SerializeField] protected float AttackSpeed = 0;
    [SerializeField] protected float AttackRange = 0;
    [SerializeField] protected float MagicDame = 0;
    [SerializeField] protected float PatrolRange = 5f;

    protected bool canAttack = false;

    protected Rigidbody2D rb;

    public Vector2 spawnPosition;

    protected Transform player;

    protected bool isChasing = false;
    [SerializeField] protected Transform enemyEye;

    protected virtual void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

    }


    protected abstract void Attack();
    protected abstract void Patrol();
    protected abstract void Die();
    protected abstract void TakeDame(float dame);



    protected virtual bool CheckInRange()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= AttackRange;
    }


    protected virtual void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected void FaceToward(Vector2 direction)
    {
        float temp = transform.position.x - enemyEye.position.x;

        if (rb.linearVelocity.x < 0)
        {
            if (temp > 0)
            {
                return;
            }
            else
            {
                Flip();
                return;
            }

        }
        if (rb.linearVelocity.x > 0)
        {
            if (temp < 0)
            {
                return;
            }
            else
            {
                Flip();
                return;
            }
        }


    }


}
