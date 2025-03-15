using UnityEngine;

public class Enemy1 : Enemy
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDame();
        }
    }
}