using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public int MaxCombo = 3;
    public float ComboResetTime = 1f;
    public float MaxAttackSpeed;
    public float attackSpeed = 1f;
    private int ComboCounter = 0;
    private float lastAttackTime = 0f;    
    private float attackIntervalCooldown = 0f;
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    public GameObject[] slashEffects;

    private int comboIndex = 0;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        attackIntervalCooldown = attackSpeed;

        if(Input.GetMouseButtonDown(0))
        {
            PerformAttack();
            Attack();
        }

        if (Time.time - lastAttackTime > ComboResetTime)
        {
            ComboCounter = 0;
            animator.SetInteger("AttackIndex", ComboCounter);
        }
        
    }

    public void Attack()
    {
        lastAttackTime = Time.time;
        ComboCounter++;
        if (ComboCounter >= MaxCombo)
        {
            ComboCounter = 0;
        }

        animator.SetInteger("AttackIndex", ComboCounter);
        animator.SetTrigger("AttackTrigger");

        
    }

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
    }

    private void ShowSlashEffect()
    {
        if (slashEffects.Length == 0 || slashEffects[ComboCounter] == null)
            return;

        
        GameObject slash = Instantiate(slashEffects[ComboCounter], attackPoint.position, slashEffects[ComboCounter].transform.rotation);

        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Quaternion rotation = new Quaternion(slashEffects[comboIndex].transform.rotation.x, slashEffects[comboIndex].transform.rotation.y * direction, slashEffects[comboIndex].transform.rotation.z, slashEffects[comboIndex].transform.rotation.w);
        slash.transform.rotation.Set(rotation.x, rotation.y, rotation.z, rotation.w);
        Destroy(slash, 1f);
    }

    public void ResetCombo()
    {
        ComboCounter = 0;
        animator.SetInteger("Combo", ComboCounter);
    }
}
