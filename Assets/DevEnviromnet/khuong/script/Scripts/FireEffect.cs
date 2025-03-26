using UnityEngine;

public class FireEffect : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(20);
            Debug.Log("Take Dame Fire");
        }
    }
}
