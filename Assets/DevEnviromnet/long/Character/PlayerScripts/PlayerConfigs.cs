using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfigs", menuName = "Player/PlayerConfigs")]
public class PlayerConfigs : ScriptableObject
{
    [Header("Ground Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float acceleration = 50f;
    public float groundDeceleration = 40f;

    [Header("Air Movement")]
    public float airAcceleration = 30f;
    public float jumpForce = 12f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.1f;

    [Header("Wall Movement")]
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpForce = new Vector2(12f, 15f);
    public float wallStickTime = 0.25f;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
}
