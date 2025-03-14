using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovementController : MonoBehaviour
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
    public LayerMask groundLayer;
    public float GroundCheckSize = 0.1f;
    public float wallCheckDistance = 0.5f;
    public float gravityScale = 5f;
    public float gravityScaleWallSlide = 1f;

    
    [Header("System Variables")]
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canDoubleJump;
    private bool isDashing;
    private float dashCooldownTimer;
    private float dashTimer;
    private bool facingRight = true;
    private float horizontalInput;

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
        CheckWall();

        // Debug.Log("CheckGrounded: " + isGrounded);
        // Debug.Log("CheckWall: " + isTouchingWall);
        // Debug.Log("IsWallSliding: " + isWallSliding);
        // Debug.Log("Coyote Time Counter: " + coyoteTimeCounter);
        


        Debug.Log("Gravity Scale: " + rb.gravityScale);
        
        // Handle jumping
        if (Input.GetButtonDown("Jump"))
        {
            
            if (isGrounded || coyoteTimeCounter > 0)
            {
                Jump();
                canDoubleJump = true;
            }
            else if (isWallSliding)
            {
                coyoteTimeCounter = coyoteTime; // Reset coyote time
                canDoubleJump = true; 
                WallJump();
            }
            else if (canDoubleJump)
            {
                DoubleJump();
                canDoubleJump = false;
            }
        }
        
        // Handle dashing
        if (Input.GetButtonDown("Fire1") && dashCooldownTimer <= 0 && !isDashing)
        {
            StartCoroutine(Dash());
        }
        
        // Update cooldowns
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
            
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
        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
    }
    
    private void Move()
    {
        // Set velocity with consistent x and maintaining current y
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        
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
        // Wall jump away from wall
        float jumpDirection = facingRight ? -1f : 1f;
        rb.linearVelocity = new Vector2(jumpDirection * moveSpeed, jumpForce);
        Flip();
        // animator.SetTrigger("WallJump");
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
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time
        }
        else if(!isGrounded && !isWallSliding) // If not grounded and not wall sliding

        {
            coyoteTimeCounter -= Time.deltaTime;
            // if (coyoteTimeCounter <= 0)
            // {
            //     canDoubleJump = false; // Disable double jump if not grounded
            // }
        }
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
        // // Flip the character
        // facingRight = !facingRight;
        // Vector3 localScale = transform.localScale;
        // localScale.x *= -1;
        // transform.localScale = localScale;

        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
        
        // // Flip the wall check position
        // Vector3 wallCheckPosition = wallCheck.position;
        // wallCheckPosition.x *= -1;
        // wallCheck.position = wallCheckPosition;
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
    }
}