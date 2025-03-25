using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    [SerializeField] private List<Skill> _unlockedSkills = new List<Skill>();
    [SerializeField] private Skill[] defaultSkills;
    [SerializeField] private Skill[] currentSkills;
    [SerializeField] private PlayerController _playerController;


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
            }
            else
            {
                _cooldownTimers.Add(skill, skill.cooldown);
            }


            if(skill.CanActiveSkill(GetComponent<PlayerController>()) && _cooldownTimers[skill] <= 0)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                {
                    skill.ActivateSkill(GetComponent<PlayerController>());
                }
                if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
                {
                    if(skill is DashSkill)
                    {
                        _cooldownTimers[skill] = skill.cooldown;
                        DashSkill dashSkill = (DashSkill)skill;

                        dashSkill.ActivateSkill(GetComponent<PlayerController>());
                        // _playerController.PlayerStateMachine.TransitionTo(_playerController.PlayerStateMachine.dashState);
                    }
                }
            }
        }
    }

}
