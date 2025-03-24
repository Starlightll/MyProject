using UnityEngine;

public class PlayerDashState : IPlayerState
{
    private PlayerController player;

    public PlayerDashState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public void Enter()
    {
        // Debug.Log("Entering Dash State");
    }

    public void Execute()
    {
        Debug.Log("Executing Dash State");
        player.Stats.isInvincible = true;
        if(!player.PlayerMovementController.isDashing || player.PlayerMovementController.isTouchingWall){
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }
    }

    public void Exit()
    {
        player.Stats.isInvincible = false;
        // Debug.Log("Exiting Dash State");
    }
}
