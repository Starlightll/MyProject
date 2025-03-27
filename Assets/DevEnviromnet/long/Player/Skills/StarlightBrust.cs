using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/StarlightBrust")]
public class StarlightBrust : Skill
{
    
    public override void ActivateSkill(PlayerController player)
    {
        player.Stats.currentMana -= manaCost;
        player.StartCoroutine(PerformStarlightBrust(player));
    }

    public override bool CanActiveSkill(PlayerController player)
    {
        return player.Stats.currentMana >= manaCost && player.PlayerStateMachine.CurrentState is not PlayerDashState;
    }

    private IEnumerator PerformStarlightBrust(PlayerController player)
    {
        Debug.Log("Starlight Brust");
        float playerDirection = player.transform.localScale.x > 0 ? 1f : -1f;
        
        Vector2 attackPosition = player.attackPoint.position;
        Vector3 eularRotation = new Vector3(0, playerDirection == 1?180:0, skillEffects[0].transform.localEulerAngles.z);
        
        Quaternion rotation = Quaternion.Euler(eularRotation);
        GameObject skillEffect = skillEffects[0];
        GameObject effect = Instantiate(skillEffect, attackPosition, rotation);
        int numberOfHits = 20;
        for(int i = 0; i < numberOfHits; i++)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, skillEffects[0].transform.localScale.x * 1.3f, LayerMask.GetMask("Enemy"));
            int unluckyEnemy = Random.Range(0, hitEnemies.Length);
            if (hitEnemies.Length > 0)
            {
                try{
                    IDamageable damageable = hitEnemies[unluckyEnemy].GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        bool isCrit = Random.Range(0, 100) <= player.Stats.critChance;
                        float damage = player.Stats.attackDamage + physicalDamage;
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
            yield return new WaitForSeconds(Random.Range(0.02f, 0.06f));
        }
        
        
        // skillEffect.transform.lo
        yield return new WaitForSeconds(skillDuration);
        Destroy(effect);
    }

    public void hit()
    {
        // Debug.Log("Hit");
        string[] hitTags = { "Enemy", "Boss" };
        
    }


}
