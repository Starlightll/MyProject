using UnityEngine;
public class TrapController : MonoBehaviour
{
    public int damage = 10;
    public float damageInterval = 1.0f;
    private bool playerInTrap = false;
    private float damageTimer = 0f;

    private void Update()
    {
        
        if (playerInTrap)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                DealDamage();
                damageTimer = damageInterval;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrap = true;
            damageTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrap = false;
        }
    }

    private void DealDamage()
    {
        Debug.Log("Cook");
    }
}