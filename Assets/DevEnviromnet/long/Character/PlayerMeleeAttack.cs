using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    public GameObject[] slashEffects;
    public int comboIndex = 0;

    public void PerformAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            // enemy.GetComponent<Enemy>()?.TakeDamage(attackDamage);
            Debug.Log("Hit: " + enemy.name);
        }

        ShowSlashEffect();
        Debug.Log("Attack performed");
        comboIndex = (comboIndex + 1) % slashEffects.Length;
    }

    private void ShowSlashEffect()
    {
        if (slashEffects.Length == 0 || slashEffects[comboIndex] == null)
            return;

        GameObject slash = Instantiate(slashEffects[comboIndex], attackPoint.position, slashEffects[comboIndex].transform.rotation);

        // float direction = transform.localScale.x > 0 ? 1f : -1f;
        // Quaternion rotation = new Quaternion(slashEffects[comboIndex].transform.rotation.x, slashEffects[comboIndex].transform.rotation.y * direction, slashEffects[comboIndex].transform.rotation.z, slashEffects[comboIndex].transform.rotation.w);
        // slash.transform.rotation = rotation;
        Destroy(slash, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    
}
