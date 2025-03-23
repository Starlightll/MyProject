using UnityEngine;
public class TrapController : MonoBehaviour
{
    public int damage = 10;
    public float damageInterval = 1.0f;
    private float lastDamageTime = 0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null && Time.time >= lastDamageTime + damageInterval)
        {
            damageable.TakeDamage(damage);
            lastDamageTime = Time.time;
            Debug.Log("Take dame");
        }
    }
}
