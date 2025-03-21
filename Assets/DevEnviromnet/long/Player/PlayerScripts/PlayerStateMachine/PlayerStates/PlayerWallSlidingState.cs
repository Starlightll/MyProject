using UnityEngine;

public class PlayerWallSlidingState : IPlayerState
{
    private PlayerController player;

    public PlayerWallSlidingState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public void Enter()
    {
        Debug.Log("Entering Wall Sliding State");
    }

    public void Execute()
    {
        Debug.Log("Executing Wall Sliding State");
    }

    public void Exit()
    {
        Debug.Log("Exiting Wall Sliding State");
    }
}
