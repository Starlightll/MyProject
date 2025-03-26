using System.Collections.Generic;
using UnityEngine;

public class StarlightBrustCollision : MonoBehaviour
{
    public ParticleSystem Particles;
    public int fireDamage = 10;
    public float burnDuration = 3f;

    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = Particles.GetCollisionEvents(other, collisionEvents);

        if (numCollisionEvents > 0 && other.CompareTag("Enemy"))
        {
            
            Debug.Log("Starlight Brust Collision");
            IDamageable damageable = other.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(10);
                DamagePopup.Create(other.transform.position, Random.Range(5, 15));
            }
        }
    }

    
}
