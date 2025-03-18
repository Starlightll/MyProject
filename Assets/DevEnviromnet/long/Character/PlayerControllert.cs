using System.Collections;
using UnityEngine;

public class PlayerControllerTem : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float wallSlideSpeed = 2f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    
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

    
    [Header("System Variables")]
    private bool isGrounded;
    private bool isTouchingHead;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canDoubleJump;
    private bool isDashing;
    private float dashCooldownTimer;
    private float dashTimer;
    private bool facingRight = true;
    private float horizontalInput;
    private bool isOnWallJump = false;
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    
    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
            
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
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
        // if (Input.GetButtonDown("Fire1") && dashCooldownTimer <= 0 && !isDashing)
        // {
        //     StartCoroutine(Dash());
        // }
        
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
        if (isDashing) return;
        
        // Move the player
        Move();
        
        // Handle wall sliding
        if (isWallSliding && isOnWallJump == false && !isGrounded)
        {
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed); // Slide down the wall
        }
    }
    
    private void Move()
    {
        // if (isWallSliding || isOnWallJump) return;
        // Set velocity with consistent x and maintaining current y
        if(isTouchingWall && isGrounded && !isOnWallJump)
        {
            rb.linearVelocity = new Vector2(0f, 0f);
        }else if (!isOnWallJump)
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
    jumpDirection.y = jumpForce/1.2f;
    jumpDirection.x *= jumpForce/3;
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
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 1f;
        dashTimer = dashDuration;
        
        // Dash direction matches facing direction
        float dashDirection = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
        
        // animator.SetTrigger("Dash");
        
        yield return new WaitForSeconds(dashDuration);
        
        isDashing = false;
        rb.gravityScale = originalGravity;
        dashCooldownTimer = dashCooldown;
    }
    
    private void CheckGrounded()
    {
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundCheckSize, groundLayer);
        if (isGrounded) // If grounded and not wall sliding
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time
        }
        else if(!isGrounded && !isWallSliding) // If not grounded and not wall sliding
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
        else if(!isTouchingWall || isGrounded) // If not touching wall or grounded
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
        transform.Rotate(0f, 180f, 0f);
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
