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

    protected Rigidbody2D rb;

    public Vector2 spawnPosition;

    protected virtual void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

    }


    protected abstract void Attack();
    protected abstract void Patrol();
    protected abstract void Die();
    protected abstract void TakeDame(float dame);

    protected virtual void CheckInRange()
    {
        Debug.Log("Check In Range");
    }

    protected void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected void FaceToward(Vector2 direction)
    {
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * direction.x, transform.localScale.y, transform.localScale.z);
    }



}
