using UnityEngine;

public class PlayerKnockBack : MonoBehaviour
{
    private Rigidbody2D rb;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (isKnockedBack) return;

        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        Invoke(nameof(ResetKnockback), knockbackDuration);
    }

    private void ResetKnockback()
    {
        isKnockedBack = false;
    }
}
