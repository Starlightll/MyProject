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
        // Debug.Log("Entering Run State");
    }

    public void Execute()
    {
        if(player.Input.AttackPressed){
            player.PlayerStateMachine.TransitionTo(new PlayerAttackState(player));
        }
        if(player.Input.MoveDirection.x == 0){
            player.PlayerStateMachine.TransitionTo(new PlayerIdleState(player));
        }if(player.PlayerMovementController.isDashing){
            player.PlayerStateMachine.TransitionTo(new PlayerDashState(player));
        }
    }

    public void Exit()
    {
        // Debug.Log("Exiting Run State");
    }
}
