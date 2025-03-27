
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;


    [CreateAssetMenu(menuName = "Player/Stats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Vitals")]
        [Min(1)]
        public float maxHealth = 100f;
        [Min(0)]
        public float currentHealth;
        public float maxMana;
        public float currentMana;
        public float maxStamina;
        public float currentStamina;
        
        [Header("Attributes")]
        public float attackDamage;
        public float attackDamageMultiplier = 1f;
        public float magicalDamage;

        public float attackRange;
        public float defense;
        public float maxAttackSpeed = 200f;
        public float attackSpeed;
        public float critChance;
        public float critDamage;
        public float lifeSteal;
        public float manaRegen;
        public float healthRegen;

        [Header("Experience")]
        public float currentExperience;
        public float experienceToNextLevel = 500;
        public float level = 0;
        public int skillPoints;
        public bool isInvincible = false;
        public bool isResetStatsNextTime = false;

        public void InitializeStats()
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
            currentStamina = maxStamina;
        }
        public void ResetStats()
        {
            maxHealth = 100f;
            maxMana = 100f;
            maxStamina = 100f;
            currentHealth = maxHealth;
            currentMana = maxMana;
            currentStamina = maxStamina;
            attackDamage = 5f;
            attackDamageMultiplier = 1f;
            magicalDamage = 5f;
            attackRange = 0f;
            defense = 5f;
            maxAttackSpeed = 200f;
            attackSpeed = 30f;
            critChance = 10f;
            critDamage = 3f;
            lifeSteal = 0f;
            manaRegen = 0f;
            healthRegen = 0f;
            currentExperience = 0f;
            experienceToNextLevel = 500;
            level = 1;
            skillPoints = 0;
            isInvincible = false;
            // isResetStatsNextTime = false;
            // Reset all other stats
        }



        public void LevelUp()
        {
            level++;
            skillPoints++;
            experienceToNextLevel = experienceToNextLevel * 1.35f;            
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
            if(isInvincible)
            {
                return;
            }
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
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
            attackDamage += amount;
            
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
