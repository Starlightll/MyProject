using UnityEngine;

[CreateAssetMenu(menuName = "Skill/CosmicWinds")]
public class CosmicWinds : Skill
{
    public override bool CanActiveSkill(PlayerController player)
    {
        return player.Stats.currentMana >= manaCost && player.PlayerStateMachine.CurrentState is not PlayerDashState and PlayerDeadState;
    }

    public override void ActivateSkill(PlayerController player)
    {
        throw new System.NotImplementedException();
    }

}
