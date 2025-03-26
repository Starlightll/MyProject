using System.Collections.Generic;
using UnityEngine;

public class StarlightBrustCollision : MonoBehaviour
{
    //Get all colliders collided with the starlight brust
     private ParticleSystem _particleSystem;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Collided with " + other.name);
        int numCollisionEvents = _particleSystem.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            if (other.CompareTag("Enemy"))  
            {
                IDamageable enemy = other.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    enemy.TakeDamage(0);
                    DamagePopup.Create(other.transform.position, 0, false);
                    Debug.Log($"🔥 {gameObject.name} hit {other.name}, dealing {2} damage!");
                }
            }
        }
    }
}
