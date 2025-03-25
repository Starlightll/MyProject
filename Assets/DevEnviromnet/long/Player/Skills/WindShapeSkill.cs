using UnityEngine;

[CreateAssetMenu(menuName = "Skill/WindShapeSkill")]
public class WindShapeSkill : Skill
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
