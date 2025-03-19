using UnityEngine;

[CreateAssetMenu(menuName = "Player/Weapon")]
public abstract class Weapon : ScriptableObject
{
    public string weaponName;
    public Sprite weaponIcon;
    public float baseDamage;
    public float attackSpeed;
    public float attackRate;
    public float critChance;
    public float critDamage;
    public float attackRange;
    public float attackDuration;
    public float attackCooldown;
    public float attackKnockback;
    public float attackStunDuration;
    public float attackManaCost;
    public float attackLifeSteal;
    public float attackHealthRegen;
    public float attackManaRegen;
    public float attackCooldownReduction;
    public float attackDamageReduction;
    public float attackSpeedReduction;
    public float attackDamageMultiplier = 1.5f;
    public float attackSpeedMultiplier = 1.5f;
    public float critChanceMultiplier = 1.5f;
    public float critDamageMultiplier = 1.5f;
    public float attackRangeMultiplier = 1.5f;
    public float attackDurationMultiplier = 1.5f;
    public float attackCooldownMultiplier = 1f;
    public float attackKnockbackMultiplier = 1.5f;

    [Header("Attack Animation")]
    public string attackTrigger;
    public float animationDuration;
    public Vector2 attackOffset;

    [Header("Attack Sounds")]
    public AudioClip attackSound;
    public AudioClip attackHitSound;
    public AudioClip attackMissSound;

    [Header("Attack Effects")]
    public GameObject[] attackEffects;
    public GameObject[] attackHitEffects;
    public GameObject[] attackMissEffects;

    [Header("Combo System")]
    public bool comboEnabled;
    public int maxComboCount;
    public float comboResetTime;
    public float comboResetDuration;

    public abstract void PerformAttack(Transform attackPoint, LayerMask enemyLayer, ref int comboCounter);
    

}
