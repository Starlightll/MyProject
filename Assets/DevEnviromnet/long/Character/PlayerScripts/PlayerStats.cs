
using UnityEngine;


    [CreateAssetMenu(menuName = "Player/Stats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Vitals")]
        public float maxHealth = 100f;
        public float currentHealth;
        public float maxMana;
        public float currentMana;
        
        [Header("Attributes")]
        public float attackPower;
        public float attackDamageMultiplier = 1f;
        public float attackRange;
        public float defense;
        public float attackSpeed;
        public float critChance;
        public float critDamage;
        public float lifeSteal;
        public float manaRegen;
        public float healthRegen;

        [Header("Experience")]
        public float currentExperience;
        public float experienceToNextLevel;
        public float level;
        public float skillPoints;

        public void ResetStats()
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
            // Reset all other stats
        }



        public void LevelUp()
        {
            level++;
            skillPoints++;
            experienceToNextLevel = level * 100;
            
        }

        public void AddExperience(float experience)
        {
            currentExperience += experience;
            if (currentExperience >= experienceToNextLevel)
            {
                LevelUp();
            }
            
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                // Die
            }
            
        }
        

        public void Heal(float amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            
        }

        public void UseMana(float amount)
        {
            currentMana -= amount;
            if (currentMana < 0)
            {
                currentMana = 0;
            }
           
        }
        

        public void RestoreMana(float amount)
        {
            currentMana += amount;
            if (currentMana > maxMana)
            {
                currentMana = maxMana;
            }
            
        }

        public void IncreaseMaxHealth(float amount)
        {
            maxHealth += amount;
            currentHealth = maxHealth;
        }
            

        public void IncreaseMaxMana(float amount)
        {
            maxMana += amount;
            currentMana = maxMana;
        }
        

        public void IncreaseAttackPower(float amount)
        {
            attackPower += amount;
            
        }

        public void IncreaseAttackDamageMultiplier(float amount)
        {
            attackDamageMultiplier += amount;
            
        }


        public void IncreaseDefense(float amount)
        {
            defense += amount;
        }

        public void IncreaseAttackSpeed(float amount)
        {
            attackSpeed += amount;
        }

        public void IncreaseCritChance(float amount)
        {
            critChance += amount;
            
        }

        public void IncreaseCritDamage(float amount)
        {
            critDamage += amount;
           
        }

        public void IncreaseLifeSteal(float amount)
        {
            lifeSteal += amount;
            
        }

        public void IncreaseManaRegen(float amount)
        {
            manaRegen += amount;
          
        }

        public void IncreaseHealthRegen(float amount)
        {
            healthRegen += amount;
        }

    }
