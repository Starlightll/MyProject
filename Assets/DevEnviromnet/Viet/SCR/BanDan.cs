using UnityEngine;

public class BanDan : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage = 10f; // Sát thương của viên đạn

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    public void SetSpeed(float bulletSpeed)
    {
        speed = bulletSpeed;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage); // Gây sát thương cho Player
            }

            Destroy(gameObject); // Hủy viên đạn sau khi va chạm
        }
    }
}
