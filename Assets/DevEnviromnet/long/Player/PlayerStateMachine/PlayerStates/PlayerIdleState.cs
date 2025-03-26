using UnityEngine;

public class PlayerIdleState : IPlayerState
{

    private PlayerController player;

    public PlayerIdleState(PlayerController playerController) {
        this.player = playerController;
    }

    public void Enter()
    {
        // Debug.Log("Entering Idle State");
    }

    public void Execute()
    {
        // Debug.Log("Executing Idle State");
        if(player.Input.AttackPressed){
            player.PlayerStateMachine.TransitionTo(new PlayerAttackState(player));
        }
        if(player.Input.MoveDirection.x != 0){
            player.PlayerStateMachine.TransitionTo(new PlayerRunState(player));
        }if(player.PlayerMovementController.isDashing){
            player.PlayerStateMachine.TransitionTo(new PlayerDashState(player));
        }
        

    }

    public void Exit()
    {
        // Debug.Log("Exiting Idle State");
    }
}
