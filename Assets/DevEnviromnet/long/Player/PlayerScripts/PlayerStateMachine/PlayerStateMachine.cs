using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class PlayerStateMachine
{
    public IPlayerState CurrentState { get; private set; }

    //Reference to state objects
    public PlayerIdleState idleState;
    public PlayerRunState runState;
    public PlayerAirState airState;
    public PlayerAttackState attackState;
    public PlayerWallSlidingState wallSlidingState;
    public PlayerDashState dashState;

    //Event to notify others objects when the state changes (Not used for now, just for future implementations)
    public event Action<IPlayerState> stateChanged;

    //Constructor pass in necessary parameters.
    public PlayerStateMachine(PlayerController playerController)
    {
        this.idleState = new PlayerIdleState(playerController);
        this.runState = new PlayerRunState(playerController);
        this.airState = new PlayerAirState(playerController);
        this.attackState = new PlayerAttackState(playerController);
        this.wallSlidingState = new PlayerWallSlidingState(playerController);
        this.dashState = new PlayerDashState(playerController);

    }


    public void Initialize(IPlayerState state)
    {
        CurrentState = state;
        CurrentState.Enter();

        //Notify others that the state has changed
        stateChanged?.Invoke(CurrentState);
    }


    //Exits the current state and enters the new state
    public void TransitionTo(IPlayerState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();

        //Notify others that the state has changed
        stateChanged?.Invoke(CurrentState);
    }

    //Update the current state
    public void Execute()
    {
        if(CurrentState != null)
        {
            CurrentState.Execute();
        }
    }

}
