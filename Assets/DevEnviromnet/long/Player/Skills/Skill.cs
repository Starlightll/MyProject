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
        public float skillDuration;


        public abstract void ActivateSkill(PlayerController player);
        public abstract bool CanActiveSkill(PlayerController player);
        
        
    }
