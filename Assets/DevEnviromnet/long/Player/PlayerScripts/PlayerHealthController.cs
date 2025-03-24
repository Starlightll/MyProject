using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{

    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerHurtEffects hurtEffect;
    [SerializeField] private PlayerKnockBack knockBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage, Vector2? knockbackDirection = null, float knockbackForce = 0)
    {
        if(stats.isInvincible){
            return;
        }
        stats.currentHealth -= damage;
        if (stats.currentHealth <= 0)
        {
            stats.currentHealth = 0;
        }
        if (hurtEffect != null) {
            Debug.Log("FlashRed");
            hurtEffect.FlashRed();
        }
        if (knockbackDirection != null){
            knockBack.ApplyKnockback(knockbackDirection.Value, knockbackForce);
        }
    }
}
