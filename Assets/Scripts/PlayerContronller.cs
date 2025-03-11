using UnityEngine;

public class PlayerContronller : MonoBehaviour
{
    public float speed = 5.0f; // Tốc độ di chuyển
    public float moveJump = 5.0f; // Lực nhảy
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Lấy component Rigidbody
    }

    void Update()
    {
       HandleMove();
        HandleJump();
    }

    void HandleMove()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocityY);

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) , transform.localScale.y, transform.localScale.z); // Nhìn phải
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1*Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Nhìn trái
        }
    }

    // Xử lý nhảy
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, moveJump);
       
        }
    }
}