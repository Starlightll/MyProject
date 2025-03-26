using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

    [CreateAssetMenu(fileName = "Skill", menuName = "Player/Skill")]
    public abstract class Skill : ScriptableObject
    {
        [Header("Unlock Conditions")]
        public bool unlocked;
        public int requiredLevel;
        public int requiredSkillPoints;
        public Skill[] requiredSkills;
        public Weapon requiredWeapon;


        [Header("Skill Info")]
        public string skillName;
        public Sprite icon;
        public string skillDescription;
        public float cooldown;
        public float manaCost;
        public float staminaCost;
        public float healthCost;
        public float skillDuration;
        public float physicalDamage;
        public float magicalDamage;

        private bool _isOnCooldown = false;

        public float initialCooldown;

        [Header("Skill Animation")]
        public string animationTrigger;
        public float animationDuration;
        public Vector2 skillOffset;

        [Header("Skill Sounds")]
        public AudioClip skillSound;
        public AudioClip skillHitSound;
        public AudioClip skillMissSound;
        
        [Header("Skill Effects")]
        public GameObject[] skillEffects;
        public GameObject[] skillHitEffects;
        public GameObject[] skillMissEffects;


        public abstract void ActivateSkill(PlayerController player);
        public abstract bool CanActiveSkill(PlayerController player);

        public IEnumerator StartCooldown()
        {
            _isOnCooldown = true;
            yield return new WaitForSeconds(cooldown);
            _isOnCooldown = false;
        }

        public bool IsOnCooldown() => _isOnCooldown;
        
    }
