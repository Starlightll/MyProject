using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerController player;

    private float attackTimer = 0.0f;
    private bool isAttackFinished = false;

    private Vector2 playerVelocity;

    public PlayerAttackState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public void Enter()
    {
        Debug.Log("Entering Attack State");
        //Play attack animation
        player._anim.SetTrigger("Attack");
       
    
    }

    //This function is called every frame
    public void Execute()
    {
        // Debug.Log("Executing Attack State");
         //Check if the attack is finished
        attackTimer += Time.deltaTime;
        if (attackTimer >= player.CurrentWeapon.CalculateTimeBetweenAttacks())
        {
            isAttackFinished = true;
        }
        if(isAttackFinished && player.Input.MoveDirection.x == 0)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }
        else if(isAttackFinished && player.Input.MoveDirection.x != 0)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.runState);
        }
        
    }

    public void Exit()
    {
        // Debug.Log("Exiting Attack State");
        attackTimer = 0.0f;
        isAttackFinished = false;
    }
}

