using UnityEngine;

public class PlayerRunState : IPlayerState
{
    private PlayerController player;

    public PlayerRunState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public void Enter()
    {
        Debug.Log("Entering Run State");
    }

    public void Execute()
    {
        if(player.Input.AttackPressed){
            player.PlayerStateMachine.TransitionTo(new PlayerAttackState(player));
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Run State");
    }
}
