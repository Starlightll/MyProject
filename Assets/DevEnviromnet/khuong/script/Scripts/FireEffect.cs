using UnityEngine;

public class FireEffect : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip fireSound;
    public float attackDamage = 20f;

   
    void OnTriggerEnter2D(Collider2D other)
    {
        audioSource.PlayOneShot(fireSound);
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(attackDamage);
            Debug.Log("Take Dame Fire");
        }
    }
}
