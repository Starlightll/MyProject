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
        Debug.Log("Entering Dash State");
    }

    public void Execute()
    {
        Debug.Log("Executing Dash State");
    }

    public void Exit()
    {
        Debug.Log("Exiting Dash State");
    }
}
