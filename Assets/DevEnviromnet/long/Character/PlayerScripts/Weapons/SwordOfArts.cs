using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/SwordOfArts")]
public class SwordOfArts : Weapon
{
    public float runtimeAttackRange = 0f;


    public GameObject[] slashEffects;

    public override void PerformAttack(Transform attacker, LayerMask enemyLayer, ref int comboCounter)
    {
        // Vector2 attackPosition = (Vector2)attacker.position + attackOffset;
        // Collider2D[] hits = Physics2D.OverlapCircleAll(
        //     attackPosition,
        //     attackRange 
        //     + attackRange * attackRangeMultiplier/100
        //     + attackRange * attacker.GetComponent<PlayerController>().Stats.attackRange/100, attackMask);

        // foreach(Collider2D hit in hits)
        // {
        //     if(hit.TryGetComponent<IDamageable>(out IDamageable damageable))
        //     {
        //         damageable.TakeDamage(baseDamage);
        //     }
        // }
        if (!comboEnabled)
        {
            comboCounter = 0;
        }
        comboCounter++;
        

            switch (comboCounter)
            {
                case 1:
                    Attack1(attacker, enemyLayer, 0);
                    break;
                case 2:
                    Attack2(attacker, enemyLayer, 1);
                    break;
                case 3:
                    Attack3(attacker, enemyLayer, 2);
                    break;
                default:
                    Attack1(attacker, enemyLayer, 0);
                    comboCounter = 1;
                    break;
            }
    }

    private void Attack1(Transform attacker, LayerMask enemyLayer, int comboCounter)
    {
        Debug.Log("Sword of Arts Attack 1");
        /*** Để tính toán được phạm vi gây sát thương đồng bộ với vfx thì cần phải
            * 1. Tính toán được vị trí tấn công
            * 2. Tính toán phạm vi tấn công của VFX.
            * 3. Tính toán phạm vi tấn công của sát thường dựa trên phạm vi tấn công của VFX.

            * Ví dụ:
            vị trí tấn công sẽ là (0, 0) của player.
            Diện tích của VFX sau khi nhân các chỉ số của player ví dụ tăng 1.5 lần diện tích ban đầu sẽ là 30x1.5 = 45;
            diện tích của sát thương sẽ đi theo VFX là 45.
            => sync được vùng gây sát thương với VFX.
         ***/
        //Calculate attack range
        float attackRange = 1f * attackRangeMultiplier;
        Debug.DrawRay(attacker.position, new Vector2(attackRange * 2.7f, 0), Color.red, 1f);
        Debug.DrawRay(attacker.position, new Vector2(-attackRange * 2.7f, 0), Color.red, 1f);

        //Perform hitbox check
        Collider2D[] hits = Physics2D.OverlapCircleAll(attacker.position, attackRange, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(baseDamage * attackDamageMultiplier);
            }
        }
        attacker.transform.parent.GetComponent<PlayerController>()._rb.linearVelocity = new Vector2(attackRange * 2.7f, 0);

        //Show slash effect
        ShowSlashEffect(attacker, comboCounter, attackRange);

        //player dash forward
        Vector2 dashDirection = new Vector2(attackRange, 0);
    }

    private void Attack2(Transform attacker, LayerMask enemyLayer, int comboCounter)
    {
        Debug.Log("Sword of Arts Attack 2");
        float attackRange = 1.5f * attackRangeMultiplier;
        Debug.DrawRay(attacker.position, new Vector2(attackRange * 2.7f, 0), Color.red, 1f);
        Debug.DrawRay(attacker.position, new Vector2(-attackRange * 2.7f, 0), Color.red, 1f);
        //Perform hitbox check
        Collider2D[] hits = Physics2D.OverlapCircleAll(attacker.position, attackRange, enemyLayer);
        attacker.transform.parent.GetComponent<PlayerController>()._rb.linearVelocity = new Vector2(attackRange * 2.7f, 0);

        ShowSlashEffect(attacker, comboCounter, attackRange);
    }

    private void Attack3(Transform attacker, LayerMask enemyLayer, int comboCounter)
    {
        Debug.Log("Sword of Arts Attack 3");
        float attackRange = 2f * attackRangeMultiplier;
        Debug.DrawRay(attacker.position, new Vector2(attackRange * 2.7f, 0), Color.red, 1f);
        Debug.DrawRay(attacker.position, new Vector2(-attackRange * 2.7f, 0), Color.red, 1f);
        //Perform hitbox check
        Collider2D[] hits = Physics2D.OverlapCircleAll(attacker.position, attackRange, enemyLayer);
        attacker.transform.parent.GetComponent<PlayerController>()._rb.linearVelocity = new Vector2(attackRange * 2.7f, 0);

        ShowSlashEffect(attacker, comboCounter, attackRange);
    }

    private void ShowSlashEffect(Transform attacker, int comboCounter, float size = 1f)
    {
        if (slashEffects.Length == 0 || slashEffects[comboCounter] == null)
            return;

        float direction = attacker.parent.transform.localScale.x > 0 ? 1f : -1f;
        // Tạo ra rotation cho vfx theo euler angle
        Vector3 eularRotation = new Vector3(Random.Range(-20, 20), slashEffects[comboCounter].transform.localEulerAngles.y * direction, slashEffects[comboCounter].transform.localEulerAngles.z);
        Debug.Log("Eular Rotation: " + eularRotation);

        //Chuyển euler angle sang quaternion
        Quaternion rotation = Quaternion.Euler(eularRotation);
        GameObject slash = Instantiate(slashEffects[comboCounter], attacker.position, rotation);
        slash.transform.localScale = new Vector3(size, size, size);
        Debug.Log("Slash: " + rotation);

        // slash.transform.eulerAngles = new Vector3(slash.transform.eulerAngles.x, slash.transform.eulerAngles.y * direction, slash.transform.eulerAngles.z);
        // Instantiate(slash, attackPoint.position, slash.transform.rotation);
        // Quaternion rotation = new Quaternion(slashEffects[comboIndex].transform.rotation.x, slashEffects[comboIndex].transform.rotation.y * direction, slashEffects[comboIndex].transform.rotation.z, slashEffects[comboIndex].transform.rotation.w);
        // slash.transform.rotation.Set(rotation.x, rotation.y, rotation.z, rotation.w);
        Destroy(slash, 1f);
    }

    public void hit(Collider2D collider)
    {
        Debug.Log("Hit");
        collider.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(2,3), 1), ForceMode2D.Impulse);
        //Call enemy take damage function here
    }

}
