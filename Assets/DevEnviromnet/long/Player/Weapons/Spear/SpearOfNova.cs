using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/SpearOfNova")]
public class SpearOfNova : Weapon
{
    public float runtimeAttackRange = 0f;

    public GameObject[] slashEffects;

    public override void PerformAttack(Transform attacker, LayerMask enemyLayer, ref int comboCounter, PlayerController player)
    {
        if (!comboEnabled)
        {
            comboCounter = 0;
        }
        comboCounter++;
        Debug.Log("Combo Counter: " + comboCounter);
        

            switch (comboCounter)
            {
                case 1:
                    player.StartCoroutine(Attack1(attacker, enemyLayer, 0, player));
                    break;
                case 2:
                    player.StartCoroutine(Attack2(attacker, enemyLayer, 1, player) );
                    break;
                case 3:
                    player.StartCoroutine(Attack3(attacker, enemyLayer, 2, player));
                    break;
                default:
                    player.StartCoroutine(Attack1(attacker, enemyLayer, 0, player));
                    comboCounter = 1;
                    break;
            }
    }

    private IEnumerator Attack1(Transform attacker, LayerMask enemyLayer, int comboCounter, PlayerController player)
    {
        Debug.Log("Sword of Arts Attack 1");
        //Calculate attack range
        float attackRange = 1f * attackRangeMultiplier;
        Debug.DrawRay(attacker.position, new Vector2(attackRange * 2.7f, 0), Color.red, 1f);
        Debug.DrawRay(attacker.position, new Vector2(-attackRange * 2.7f, 0), Color.red, 1f);


        //Perform hitbox check
        int numberOfHits = 1;
        ShowSlashEffect(attacker, comboCounter, attackRange);
        for(int i = 0; i < numberOfHits; i++)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attacker.position, attackRange  * 2.7f, enemyLayer);
            int unluckyEnemy = Random.Range(0, hitEnemies.Length);
             //Show slash effect
            if (hitEnemies.Length > 0)
            {
                try{
                    IDamageable damageable = hitEnemies[unluckyEnemy].GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        bool isCrit = Random.Range(0, 100) <= player.Stats.critChance + critChance;
                        float damage = player.Stats.attackDamage + baseDamage + baseDamage * player.Stats.level/100 * 1.15f;
                        float finalDamage = isCrit ? damage * player.Stats.critDamage : damage;
                        damageable.TakeDamage(finalDamage);
                        try{
                            Enemy enemy = hitEnemies[unluckyEnemy].GetComponent<Enemy>();
                            if(enemy.currentHealth <= 0){
                                player.Stats.currentMana += 5;
                                player.Stats.currentExperience += enemy.expereince;
                            }

                        }catch(System.Exception e){
                            // Debug.Log(e);
                        }
                        DamagePopup.Create(hitEnemies[unluckyEnemy].transform.position, finalDamage, isCrit);
                    }
                }catch(System.Exception e){
                    Debug.Log(e);
                }
               
            }
            yield return new WaitForSeconds(0.02f);
        }

        // PerformHits(hits, attacker);
   
        yield return new WaitForSeconds(0);
    }

    private IEnumerator Attack2(Transform attacker, LayerMask enemyLayer, int comboCounter, PlayerController player)
    {
        Debug.Log("Sword of Arts Attack 2");
        float attackRange = 1.5f * attackRangeMultiplier;
        Debug.DrawRay(attacker.position, new Vector2(attackRange * 2.7f, 0), Color.red, 1f);
        Debug.DrawRay(attacker.position, new Vector2(-attackRange * 4f, 0), Color.red, 1f);
        //Perform hitbox check
        int numberOfHits = 3;
        ShowSlashEffect(attacker, comboCounter, attackRange);
        for(int i = 0; i < numberOfHits; i++)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attacker.position, attackRange  * 2.7f, enemyLayer);
            int unluckyEnemy = Random.Range(0, hitEnemies.Length);
             //Show slash effect
            if (hitEnemies.Length > 0)
            {
                try{
                    IDamageable damageable = hitEnemies[unluckyEnemy].GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        bool isCrit = Random.Range(0, 100) <= player.Stats.critChance + critChance;
                        float damage = player.Stats.attackDamage + baseDamage + baseDamage * player.Stats.level/100 * 1.15f;
                        float finalDamage = isCrit ? damage * player.Stats.critDamage : damage;
                        damageable.TakeDamage(finalDamage);
                        DamagePopup.Create(hitEnemies[unluckyEnemy].transform.position, finalDamage, isCrit);
                        try{
                            Enemy enemy = hitEnemies[unluckyEnemy].GetComponent<Enemy>();
                            if(enemy.currentHealth <= 0){
                                player.Stats.currentMana += 5;
                                player.Stats.currentExperience += enemy.expereince;
                            }

                        }catch(System.Exception e){
                            // Debug.Log(e);
                        }
                    }
                }catch(System.Exception e){
                    Debug.Log(e);
                }
               
            }
            yield return new WaitForSeconds(0.02f);
        }

        // PerformHits(hits, attacker);
   
        yield return new WaitForSeconds(0);
    }

    private IEnumerator Attack3(Transform attacker, LayerMask enemyLayer, int comboCounter, PlayerController player)
    {
        Debug.Log("Sword of Arts Attack 3");
        float attackRange = 2f * attackRangeMultiplier;
        Debug.DrawRay(attacker.position, new Vector2(attackRange * 2.7f, 0), Color.red, 1f);
        Debug.DrawRay(attacker.position, new Vector2(-attackRange * 2.7f, 0), Color.red, 1f);
        //Perform hitbox check
        int numberOfHits = 4;
        ShowSlashEffect(attacker, comboCounter, attackRange);
        for(int i = 0; i < numberOfHits; i++)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attacker.position, attackRange  * 2.7f, enemyLayer);
            int unluckyEnemy = Random.Range(0, hitEnemies.Length);
             //Show slash effect
            if (hitEnemies.Length > 0)
            {
                try{
                    IDamageable damageable = hitEnemies[unluckyEnemy].GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        bool isCrit = Random.Range(0, 100) <= player.Stats.critChance + critChance;
                        float damage = player.Stats.attackDamage + baseDamage + baseDamage * player.Stats.level/100 * 1.15f;
                        float finalDamage = isCrit ? damage * player.Stats.critDamage : damage;
                        damageable.TakeDamage(finalDamage);
                        DamagePopup.Create(hitEnemies[unluckyEnemy].transform.position, finalDamage, isCrit);
                        try{
                            Enemy enemy = hitEnemies[unluckyEnemy].GetComponent<Enemy>();
                            if(enemy.currentHealth <= 0){
                                player.Stats.currentMana += 5;
                                player.Stats.currentExperience += enemy.expereince;
                            }

                        }catch(System.Exception e){
                            // Debug.Log(e);
                        }
                    }
                }catch(System.Exception e){
                    Debug.Log(e);
                }
               
            }
            yield return new WaitForSeconds(0.02f);
        }

        // PerformHits(hits, attacker);
   
        yield return new WaitForSeconds(0);
    }

    private void ShowSlashEffect(Transform attacker, int comboCounter, float size = 1f)
    {
        if (slashEffects.Length == 0 || slashEffects[comboCounter] == null)
            return;

        Vector2 attackPoint = new Vector2(attacker.position.x + attacker.parent.GetComponent<Rigidbody2D>().linearVelocity.x/5, attacker.parent.position.y + attacker.parent.GetComponent<Rigidbody2D>().linearVelocity.y/10);
        Debug.Log("Attack Point: " + attacker.parent.GetComponent<Rigidbody2D>().linearVelocity.x + " " + attacker.position.x);

        float direction = attacker.parent.transform.localScale.x > 0 ?  0f :180f;
        // Tạo ra rotation cho vfx theo euler angle
        Vector3 eularRotation = new Vector3(Random.Range(-20, 20), direction, slashEffects[comboCounter].transform.localEulerAngles.z);
        // Debug.Log("Eular Rotation: " + eularRotation);

        //Chuyển euler angle sang quaternion
        Quaternion rotation = Quaternion.Euler(eularRotation);
        GameObject slash = Instantiate(slashEffects[comboCounter], attackPoint, rotation);
        slash.transform.localScale = new Vector3(size, size, size);
        // Debug.Log("Slash: " + rotation);

        // slash.transform.eulerAngles = new Vector3(slash.transform.eulerAngles.x, slash.transform.eulerAngles.y * direction, slash.transform.eulerAngles.z);
        // Instantiate(slash, attackPoint.position, slash.transform.rotation);
        // Quaternion rotation = new Quaternion(slashEffects[comboIndex].transform.rotation.x, slashEffects[comboIndex].transform.rotation.y * direction, slashEffects[comboIndex].transform.rotation.z, slashEffects[comboIndex].transform.rotation.w);
        // slash.transform.rotation.Set(rotation.x, rotation.y, rotation.z, rotation.w);
        Destroy(slash, 1f);
    }

    public void hit(Collider2D collider, Transform attacker)
    {
        Debug.Log("Hit");
        
        if(collider.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            float totalDamage = baseDamage;
            if(player.GetComponent<PlayerController>().comboCounter == 0)
            {
                totalDamage = baseDamage * 1f;
            }
            else if(player.GetComponent<PlayerController>().comboCounter == 1)
            {
                totalDamage = baseDamage * 1.5f;
            }
            else if(player.GetComponent<PlayerController>().comboCounter == 2)
            {
                totalDamage = baseDamage * 2.5f;
            }else{
                totalDamage = baseDamage * 1f;
            }
            damageable.TakeDamage(totalDamage);
            Debug.Log("Total Damage: " + totalDamage);
            
            Vector2 knockbackDirection = (collider.transform.position - attacker.parent.position).normalized;
            knockbackDirection.y = 0.5f;
            collider.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockback, ForceMode2D.Impulse);
        }
    }
    
    private void hit(IDamageable damageable)
    {
        damageable.TakeDamage(baseDamage);
    }

    private void PerformHits(Collider2D[] hits, Transform attacker)
    {
       foreach (Collider2D enemy in hits)
        {
            if (enemy.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(baseDamage);
                Vector2 knockbackDirection = (enemy.transform.position - attacker.parent.position).normalized;
                knockbackDirection.y = 0.5f;
                enemy.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockback, ForceMode2D.Impulse);
            }
            Debug.Log("Hit");
        }
    }

    private void dash(float power)
    {
        Rigidbody2D rb = player.GetComponent<PlayerController>()._rb;
        rb.AddForce(new Vector2(player.transform.localScale.x * power, 0), ForceMode2D.Impulse);
    }
    

    public override float CalculateTimeBetweenAttacks()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        //This is for Calculate final attacks speed or attack cooldown between attacks.
        //This function ned:
        // 1. Current weapon attack speed
        // 2. Player attack speed
        // 3. Weapon attack speed multiplier

        // result will be calculated as:
        // Player attack speed * player attack speed multiplier as 1
        // Weapon attack speed * weapon attack speed multiplier as 2
        // 1 + 2 + ... + n / max number of attack speed values  => percentage of attack speed
        // time between attacks * 1 - percentage of attack speed
        float weaponAttackSpeed = playerController.CurrentWeapon.attackSpeed * playerController.CurrentWeapon.attackSpeedMultiplier;
        float playerAttackSpeed = playerController.Stats.attackSpeed;

        float percentageOfAttackSpeed = (weaponAttackSpeed + playerAttackSpeed) / playerController.Stats.maxAttackSpeed;
        float timeBetweenAttacks = playerController.CurrentWeapon.attackCooldown - playerController.CurrentWeapon.attackCooldown * percentageOfAttackSpeed;
        return timeBetweenAttacks;
    }
}
