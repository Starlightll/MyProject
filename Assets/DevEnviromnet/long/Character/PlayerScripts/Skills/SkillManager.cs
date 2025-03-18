using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    [SerializeField] private List<Skill> _unlockedSkills = new List<Skill>();
    [SerializeField] private Skill[] defaultSkills;
    
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
            if(skill.CanActiveSkill(GetComponent<PlayerController>()))
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                {
                    skill.ActivateSkill(GetComponent<PlayerController>());
                }
            }
        }
    }

}
