using UnityEngine;

public class BanDan : MonoBehaviour
{
    public float speed = 10f;  // Tốc độ mặc định
    private Transform target;  // Mục tiêu viên đạn nhắm đến, ở đây là Player
    public float damage = 10f;  // Sát thương của viên đạn

    public void SetTarget(Transform player)
    {
        target = player;
    }

    public void SetSpeed(float bulletSpeed)  // Phương thức SetSpeed để cài đặt tốc độ viên đạn
    {
        speed = bulletSpeed;
    }

    void Update()
    {
        if (target != null)
        {
            // Di chuyển viên đạn về phía Player
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Kiểm tra xem viên đạn có va chạm với Player hay không
            if (Vector2.Distance(transform.position, target.position) < 0.5f)
            {
                // Gây sát thương cho Player
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);  // Gây sát thương cho Player
                    Debug.Log("Player bị trúng đạn!");
                }

                // Hủy viên đạn sau khi va chạm
                Destroy(gameObject);
            }
        }
    }
}
