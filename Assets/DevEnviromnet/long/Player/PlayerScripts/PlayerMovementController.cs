using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float wallSlideSpeed = 2f;
    public float dashSpeed = 30f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 2f;
    public float dashEndSpeed = 8f; // Speed preserved after dash ends


    [Header("References")]

    public Animator animator;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform HeadCheck;
    public LayerMask groundLayer;
    public float GroundCheckSize = 0.1f;
    public float HeadCheckSize = 0.1f;
    public float wallCheckDistance = 0.5f;
    public float gravityScale = 5f;
    public float gravityScaleWallSlide = 1f;
    public AnimationCurve dashCurve; // For inspector customization
    public AnimationCurve dashSpeedCurve; // For inspector customization
    public GameObject dashEffectPrefab; // Optional particle effect
    public Color dashTrailColor = new Color(0.5f, 0.8f, 1f, 0.7f);


    [Header("System Variables")]
    public bool isGrounded;
    public bool isTouchingHead;
    public bool isTouchingWall;
    public bool isWallSliding;
    public bool canDoubleJump;
    public bool isDashing = false;
    public float dashCooldownTimer = 2f;
    public float dashTimer;
    public bool facingRight = true;
    public float horizontalInput;
    public bool isOnWallJump = false;
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private Vector2 momentumAfterDash;
    public PlayerController _playerController;

    private Vector2 playerAttackVelocity;

    private TrailRenderer dashTrail;

    public Transform middleObject;
    


    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_playerController.PlayerStateMachine.CurrentState is PlayerDeadState or PlayerDashState)
        {
            return;
        }
        // Get input
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Check environment
        CheckGrounded();
        CheckHead();
        CheckWall();

        // Handle jumping
        if (Input.GetButtonDown("Jump"))
        {

            if ((isGrounded && !isTouchingWall) || (coyoteTimeCounter > 0 && !isTouchingWall))
            {
                Jump();
                canDoubleJump = true;
            }
            else if (isWallSliding || isTouchingWall)
            {
                coyoteTimeCounter = coyoteTime; // Reset coyote time
                canDoubleJump = false;
                WallJump();
            }
            else if (canDoubleJump)
            {
                DoubleJump();
                canDoubleJump = false;
            }
        }

        // Handle dashing
        dashTimer += Time.deltaTime;
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) && dashTimer >= dashCooldown && !isDashing && _playerController.Stats.currentStamina >= 40 && !isTouchingWall)
        {
            Debug.Log("Dash");
            // rb.linearVelocity = new Vector2(0, 0);
            _playerController.Stats.currentStamina -= 40;
            StartCoroutine(Dash());
            _playerController.PlayerStateMachine.TransitionTo(_playerController.PlayerStateMachine.dashState);

        }

        // // Update cooldowns
        // if (dashCooldownTimer > 0)
        //     dashCooldownTimer -= Time.deltaTime;

        // // Handle wall sliding
        // if (isTouchingWall && !isGrounded && horizontalInput != 0)
        // {
        //     isWallSliding = true;
        //     rb.gravityScale = 0.5f; // Reduce gravity while wall sliding
        //     rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed); // Slide down the wall
        //     // animator.SetBool("IsWallSliding", true);


        // }
        // else
        // {
        //     isWallSliding = false;
        // }

        // Update animations
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (_playerController.PlayerStateMachine.CurrentState is PlayerDeadState)
        {
            return;
        }
        if (_playerController.PlayerStateMachine.CurrentState is not (PlayerAttackState or PlayerDashState))
        {
            // Move the player
            Move();

            // Handle wall sliding
            if (isWallSliding && isOnWallJump == false && !isGrounded)
            {

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed); // Slide down the wall
            }
        }
        if (_playerController.PlayerStateMachine.CurrentState is PlayerAttackState && _playerController.CurrentWeapon is SwordOfArts)
        {
            float direction = facingRight ? 1 : -1;
            rb.linearVelocity = new Vector2(_playerController.playerVelocity.x == 0 ? 5 * direction : Mathf.Abs(_playerController.playerVelocity.x) * direction, _playerController.playerVelocity.y);
        }
    }

    private void Move()
    {
        
        // if (isWallSliding || isOnWallJump) return;
        // Set velocity with consistent x and maintaining current y
        if (isTouchingWall && isGrounded && !isOnWallJump)
        {
            rb.linearVelocity = new Vector2(0f, 0f);
        }else if (!isGrounded && !isDashing && !isWallSliding && !isOnWallJump)
        {
            //Blend movement with current velocity
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y), 0.3f);
        }
        else if (!isOnWallJump && !isDashing && isGrounded)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }

        // Flip character based on movement direction
        if (horizontalInput > 0 && !facingRight)
            Flip();
        else if (horizontalInput < 0 && facingRight)
            Flip();
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        // animator.SetTrigger("Jump");
    }

    private void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.8f);
        // animator.SetTrigger("DoubleJump");
    }

    private void WallJump()
    {
        isOnWallJump = true;
        Vector2 jumpDirection = facingRight ? Vector2.left : Vector2.right;
        jumpDirection.y = jumpForce / 1.2f;
        jumpDirection.x *= jumpForce / 3;
        // rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
        Debug.Log("Wall Jump:" + jumpDirection);
        rb.linearVelocity = jumpDirection;
        // rb.AddForce(jumpDirection, ForceMode2D.Impulse);
        StartCoroutine(MaintainWallJumpVelocity());
    }

    private IEnumerator MaintainWallJumpVelocity()
    {
        yield return new WaitForSeconds(0.2f);
        isOnWallJump = false;
    }

    private IEnumerator Dash()
    {
        // Setup
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 4f; // No gravity during dash

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
            dashDirection = new Vector2(facingRight ? 1f : -1f, 0f);
        }
        rotateDirection = new Vector2(facingRight ? -1f : 1f, 0f);

        // Setup dash effects
        if (dashTrail == null && TryGetComponent(out dashTrail))
        {
            dashTrail.emitting = true;
            dashTrail.startColor = dashTrailColor;
        }

        // Spawn dash effect
        if (dashEffectPrefab != null)
        {
            Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
        }

        // Time slow effect for better game feel
        Time.timeScale = 0.9f;

        // // Optional camera shake
        // if (Camera.main != null && Camera.main.TryGetComponent(out CinemachineImpulseSource impulse))
        // {
        //     impulse.GenerateImpulse();
        // }

        // Physics ignore layer
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        // Trigger dash animation
        animator.SetTrigger("Dash");

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
            dashDirection.x = speedCurve * (facingRight ? 1f : -1f);
            dashDirection.y = curveValue; // Upward curve for better feel

            rb.linearVelocity = dashDirection * dashSpeed;

            //Rotate player Z with fixed time and calculate number of rotations based on time
            
            // middleObject.localRotation = Quaternion.Euler(0, 0, 360 * (elapsedTime / dashDuration));

            
            
            _playerController._anim.SetBool("IsDashing", true);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Restore normal time
        Time.timeScale = 1f;
        //Set rotation angle to 0 when dash ends
        middleObject.localRotation = Quaternion.Euler(0, 0, 0);

        // End dash with momentum preservation
        dashTimer = 0;
        isDashing = false;
        _playerController._anim.SetBool("IsDashing", false);

        // // End velocity has some momentum in dash direction
        // rb.linearVelocity = dashDirection * dashEndSpeed;

        // Return to normal gravity
        rb.gravityScale = originalGravity;

        // Turn off dash effects
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        // Re-enable collisions
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
        
        // Set cooldown
        dashCooldownTimer = dashCooldown;
    }

    // public IEnumerable Dash(Vector2 direction, float power, float duration)
    // {   
    //     isDashing = true;
    //     rb.gravityScale = 2f;
    //     rb.linearVelocity = direction * power;
    //     yield return new WaitForSeconds(duration);
    //     rb.linearVelocity = Vector2.zero;
    //     isDashing = false;
    // }

    private void CheckGrounded()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundCheckSize, groundLayer);
        if (isGrounded) // If grounded and not wall sliding
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time
        }
        else if (!isGrounded && !isWallSliding) // If not grounded and not wall sliding
        {
            coyoteTimeCounter -= Time.deltaTime;

        }
    }

    private void CheckHead()
    {
        isTouchingHead = Physics2D.OverlapCircle(HeadCheck.position, GroundCheckSize, groundLayer);

    }

    private void CheckWall()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        isTouchingWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayer);
        if (isTouchingWall && !isGrounded)
        {
            // Check if the player is sliding down the wall
            isWallSliding = true;
            // rb.gravityScale = gravityScaleWallSlide; // Reduce gravity while wall sliding
            // rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed); // Slide down the wall
            coyoteTimeCounter = coyoteTime; // Reset coyote timea
            // animator.SetBool("IsWallSliding", true);
            return;
        }
        else if (!isTouchingWall || isGrounded) // If not touching wall or grounded
        {
            rb.gravityScale = gravityScale; // Reset gravity scale
            // animator.SetBool("IsWallSliding", false);
            // animator.SetBool("IsTouchingWall", false);
            isWallSliding = false;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        int direction = facingRight ? 1 : -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
    }


    private void UpdateAnimations()
    {
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(horizontalInput));
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsWallSliding", isWallSliding);
        animator.SetBool("IsTouchingWall", isTouchingWall);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(groundCheck.position, GroundCheckSize);

        Gizmos.color = Color.cyan;
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawRay(wallCheck.position, direction * wallCheckDistance);

        Gizmos.color = Color.yellow;
    }
}