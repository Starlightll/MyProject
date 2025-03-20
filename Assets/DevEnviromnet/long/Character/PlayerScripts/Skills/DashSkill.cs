using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class DashSkill : Skill
{
    public float dashDuration = 0.3f;
    public float dashSpeed = 20f;

    public override void ActivateSkill(PlayerController player)
    {
        player.StartCoroutine(PerformDash(player));
    }

    public override bool CanActiveSkill(PlayerController player)
    {
        return player.Stats.currentMana >= manaCost;
    }

    private IEnumerator PerformDash(PlayerController player)
    {
        player.Stats.currentMana -= manaCost;
        float originalGravity = player.GetComponent<Rigidbody2D>().gravityScale;
        player.GetComponent<Rigidbody2D>().gravityScale = 1;
        
        // Vector2 dashDirection = player.GetComponent<PlayerInput>().;
        // player.GetComponent<Rigidbody2D>().velocity = dashDirection * dashSpeed;
        
        yield return new WaitForSeconds(dashDuration);
        
        player.GetComponent<Rigidbody2D>().gravityScale = originalGravity;
    }
}
