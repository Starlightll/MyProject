using UnityEngine;

[CreateAssetMenu(menuName = "Skill/AstraCosmic")]
public class AstraCosmic : Skill
{
    public override void ActivateSkill(PlayerController player)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanActiveSkill(PlayerController player)
    {
        return false;
    }
}

