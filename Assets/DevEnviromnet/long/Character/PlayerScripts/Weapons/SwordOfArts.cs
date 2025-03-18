using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/SwordOfArts")]
public class SwordOfArts : Weapon
{
    [Header("Sword Of Arts")]
    public float attackDamageMultiplier = 1.5f;
    public float attackSpeedMultiplier = 1.5f;
    public float critChanceMultiplier = 1.5f;
    public float critDamageMultiplier = 1.5f;
    public float attackRangeMultiplier = 1.5f;
    public float attackDurationMultiplier = 1.5f;
    public float attackCooldownMultiplier = 1f;
    public float attackKnockbackMultiplier = 1.5f;


    public override void PerformAttack(Transform attacker, LayerMask attackMask)
    {
        Vector2 attackPosition = (Vector2)attacker.position + attackOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPosition,
            attackRange 
            + attackRange * attackRangeMultiplier/100
            + attackRange * attacker.GetComponent<PlayerController>().Stats.attackRange/100, attackMask);
        
        foreach(Collider2D hit in hits)
        {
            if(hit.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(baseDamage);
            }
        }
    }
}
