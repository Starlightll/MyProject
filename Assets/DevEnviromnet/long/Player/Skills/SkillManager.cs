using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    [SerializeField] private List<Skill> _unlockedSkills = new List<Skill>();
    [SerializeField] private Skill[] defaultSkills;
    [SerializeField] private Skill[] currentSkills;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private SkillBarManagement _skillBarManagement;
    [SerializeField] private PlayerStaminaController _playerStaminaController;


    private Dictionary<Skill, float> _cooldownTimers = new Dictionary<Skill, float>();
    
    private void Awake() {
        foreach(Skill skill in defaultSkills)
        {
            UnlockSkill(skill);
        }
    }

    public void UnlockSkill(Skill skill)
    {
        if(!_unlockedSkills.Contains(skill))
        {
            _unlockedSkills.Add(skill);
        }
    }

    public void UpdateSkills()
    {
        foreach(Skill skill in _unlockedSkills)
        {
            if(_cooldownTimers.ContainsKey(skill))
            {
                if(_cooldownTimers[skill] > 0)
                {
                    _cooldownTimers[skill] -= Time.deltaTime;
                }
                if(_cooldownTimers[skill] > skill.cooldown){
                    _cooldownTimers[skill] = skill.cooldown;
                }
            }
            else
            {
                _cooldownTimers.Add(skill, skill.cooldown);
            }
            skill.initialCooldown = _cooldownTimers[skill] <= 0 ? 0 : _cooldownTimers[skill];
        }

        //Skill 1: Dash
        if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Skill skill = _playerController.CurrentWeapon.skills[0];
            if(skill.CanActiveSkill(_playerController) && _cooldownTimers[skill] <= 0)
            {
                //Start UI display cooldown
                // _skillBarManagement.StartCooldown(_playerController.CurrentWeapon.skills[0].cooldown, 0);

                //Activate skill
                skill.ActivateSkill(_playerController);
                _playerStaminaController.timer = 0;
                _cooldownTimers[skill] = skill.cooldown;
            }
        }
        
        //Skill 2: Weapon Skill
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Skill skill = _playerController.CurrentWeapon.skills[1];
            if(skill.CanActiveSkill(_playerController) && _cooldownTimers[skill] <= 0)
            {
                //Start UI display cooldown
                // _skillBarManagement.StartCooldown(_playerController.CurrentWeapon.skills[1].cooldown, 1);

                //Activate skill
                skill.ActivateSkill(_playerController);
                _cooldownTimers[skill] = skill.cooldown;
            }
        }


        // foreach(Skill skill in _playerController.CurrentWeapon.skills){
        //     if(skill.CanActiveSkill(GetComponent<PlayerController>()) && _cooldownTimers[skill] <= 0)
        //     {
        //         if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        //         {
        //             skillCooldownUI.StartCooldown(_playerController.CurrentWeapon.skills[0].cooldown, 0);
                    
        //             _cooldownTimers[skill] = skill.cooldown;
        //         }
        //         if(Input.GetKeyDown(KeyCode.Q))
        //         {
        //             skillCooldownUI.StartCooldown(_playerController.CurrentWeapon.skills[1].cooldown, 1);
        //             _playerController.CurrentWeapon.skills[1].ActivateSkill(_playerController);
        //             _cooldownTimers[skill] = skill.cooldown;
        //         }
        //     }
        // }
    }

}
