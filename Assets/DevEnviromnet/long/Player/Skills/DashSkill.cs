using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "Skill/DashSkill")]
public class DashSkill : Skill
{

    public float dashSpeed = 30f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 2f;
    public float dashEndSpeed = 8f;

    private Vector2 playerAttackVelocity;

    private TrailRenderer dashTrail;



    public AnimationCurve dashCurve; // For inspector customization
    public AnimationCurve dashSpeedCurve; // For inspector customization
    public GameObject dashEffectPrefab; // Optional particle effect
    public Color dashTrailColor = new Color(0.5f, 0.8f, 1f, 0.7f);

    public override void ActivateSkill(PlayerController player)
    {
        player.StartCoroutine(PerformDash(player));
    }

    public override bool CanActiveSkill(PlayerController player)
    {
        return player.Stats.currentMana >= manaCost && player.PlayerStateMachine.CurrentState is not PlayerDashState && !player.PlayerMovementController.isTouchingWall;
    }

    private IEnumerator PerformDash(PlayerController player)
    {
       
        player.Stats.currentMana -= manaCost;
        // Setup
        player.PlayerMovementController.isDashing = true;
        float originalGravity = player.PlayerMovementController.rb.gravityScale;
        player.PlayerMovementController.gravityScale = 4f; // No gravity during dash

        // Get dash direction
        Vector2 dashDirection;
        Vector2 rotateDirection;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Directional dash if there's input, otherwise dash in facing direction
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            dashDirection = new Vector2(horizontal, vertical).normalized;
        }
        else
        {
            dashDirection = new Vector2(player.PlayerMovementController.facingRight ? 1f : -1f, 0f);
        }
        rotateDirection = new Vector2(player.PlayerMovementController.facingRight ? -1f : 1f, 0f);

        // Setup dash effects
        if (dashTrail == null && player.TryGetComponent(out TrailRenderer trail))
        {
             dashTrail.emitting = true;
            dashTrail.startColor = dashTrailColor;
        }

        // Spawn dash effect
        if (dashEffectPrefab != null)
        {
            Instantiate(dashEffectPrefab, player.transform.position, Quaternion.identity);
        }

        // Time slow effect for better game feel
        Time.timeScale = 0.9f;

        // // Optional camera shake
        // if (Camera.main != null && Camera.main.TryGetComponent(out CinemachineImpulseSource impulse))
        // {
        //     impulse.GenerateImpulse();
        // }

        // Physics ignore layer
        Physics2D.IgnoreLayerCollision(player.gameObject.layer, LayerMask.NameToLayer("Enemy"), true);


        // Smooth dash execution
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            float speedCurve = dashSpeedCurve != null && dashSpeedCurve.keys.Length > 0 ?
                dashSpeedCurve.Evaluate(elapsedTime / dashDuration) :
                0f; // Fallback to constant speed 
            // Use animation curve for dash shape
            float curveValue = dashCurve != null && dashCurve.keys.Length > 0 ?
                dashCurve.Evaluate(elapsedTime / dashDuration) : 0f; // Fallback to linear curve
            // Apply velocity with curve
            dashDirection.x = speedCurve * (player.PlayerMovementController.facingRight ? 1f : -1f);
            dashDirection.y = curveValue; // Upward curve for better feel

            player.PlayerMovementController.rb.linearVelocity = dashDirection * dashSpeed;

            //Rotate player Z with fixed time and calculate number of rotations based on time
            
            // middleObject.localRotation = Quaternion.Euler(0, 0, 360 * (elapsedTime / dashDuration));

            
            
            player._anim.SetBool("IsDashing", true);

            elapsedTime += Time.deltaTime;
            Debug.Log(elapsedTime);
            yield return null;
        }
        // Restore normal time
        Time.timeScale = 1f;
        //Set rotation angle to 0 when dash ends

        // End dash with momentum preservation
        // dashTimer = 0;
        player.PlayerMovementController.isDashing = false;
        player._anim.SetBool("IsDashing", false);

        // // End velocity has some momentum in dash direction
        // rb.linearVelocity = dashDirection * dashEndSpeed;

        // Return to normal gravity
        player.PlayerMovementController.rb.gravityScale = originalGravity;

        // Turn off dash effects
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        // Re-enable collisions
        Physics2D.IgnoreLayerCollision(player.gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
    }
}
