using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerController player;

    private float attackTimer = 0.0f;
    private bool isAttackFinished = false;

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

    public void Execute()
    {
        Debug.Log("Executing Attack State");
         //Check if the attack is finished
        attackTimer += Time.deltaTime;
        if (attackTimer >= player.CurrentWeapon.attackCooldown)
        {
            isAttackFinished = true;
        }
        if(isAttackFinished)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Attack State");
        attackTimer = 0.0f;
    }
}

