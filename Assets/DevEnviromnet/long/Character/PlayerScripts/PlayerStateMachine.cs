using System.Collections;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; }


    // public void HandleInput(PlayerInput input)
    // {
    //     if(CurrentState == PlayerState.Attacking || CurrentState == PlayerState.Dashing)
    //         return;

    //     HandleMovement(input.MoveDirection);
    //     HandleJump(input.JumpPressed);
    //     HandleAttack(input.AttackPressed);
    // }

    // private void HandleMovement(Vector2 direction)
    // {
    //     _rb.linearVelocity = new Vector2(
    //         direction.x * _configs.runSpeed,
    //         _rb.linearVelocity.y
    //     );
    // }

    // private void HandleJump(bool jumpPressed)
    // {
    //     if(jumpPressed && IsGrounded())
    //     {
    //         _rb.AddForce(Vector2.up * _configs.jumpForce, ForceMode2D.Impulse);
    //     }
    // }

    // private void HandleAttack(bool attackPressed)
    // {
    //     if(attackPressed && Time.time >= _attackCooldownTimer)
    //     {
    //         StartCoroutine(PerformAttack());
    //     }
    // }

    // private IEnumerator PerformAttack()
    // {
    //     CurrentState = PlayerState.Attacking;
    //     _anim.SetTrigger(_weaponManager.CurrentWeapon.attackTrigger);
    //     _weaponManager.CurrentWeapon.PerformAttack(transform, LayerMask.GetMask("Enemy"));
        
    //     _attackCooldownTimer = Time.time + _weaponManager.CurrentWeapon.attackRate;
        
    //     yield return new WaitForSeconds(_weaponManager.CurrentWeapon.animationDuration);
        
    //     if(IsGrounded())
    //         CurrentState = PlayerState.Grounded;
    //     else
    //         CurrentState = PlayerState.Airborne;
    // }

    // private bool IsGrounded()
    // {
    //     // Implement ground check using raycast or overlap circle
    //     return Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("Ground"));
    // }

    // private bool IsTouchingWall()
    // {
    //     return Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, groundLayer); 
    // }
}
