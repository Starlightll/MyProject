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
        if(stats.currentHealth > stats.maxHealth){
            stats.currentHealth = stats.maxHealth;
        }
        if (stats.currentHealth <= 0)
        {
            stats.currentHealth = 0;
        }
    }

    public void TakeDamage(float damage, Vector2? knockbackDirection = null, float knockbackForce = 0)
    {
        if(stats.isInvincible){
            return;
        }

        float calculatedDamage = Mathf.Round(damage / (1 + (stats.defense / 100f)) * 1000f) / 1000f;
        stats.currentHealth -= calculatedDamage;

        DamagePopup.Create(transform.position, calculatedDamage, false);
        if (hurtEffect != null) {
            Debug.Log("FlashRed");
            hurtEffect.FlashRed();
        }
        if (knockbackDirection != null){
            knockBack.ApplyKnockback(knockbackDirection.Value, knockbackForce);
        }
    }
}
