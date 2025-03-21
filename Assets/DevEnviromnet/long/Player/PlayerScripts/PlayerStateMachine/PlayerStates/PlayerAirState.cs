using UnityEngine;

public class PlayerAirState : IPlayerState
{
    private PlayerController player;

    public PlayerAirState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public void Enter()
    {
        Debug.Log("Entering Air State");
    }

    public void Execute()
    {
        Debug.Log("Executing Air State");
    }

    public void Exit()
    {
        Debug.Log("Exiting Air State");
    }
}
