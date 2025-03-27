using UnityEngine;

public class FireEffect : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip fireSound;

   
    void OnTriggerEnter2D(Collider2D other)
    {
        audioSource.PlayOneShot(fireSound);
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(20);
            Debug.Log("Take Dame Fire");
        }
    }
}
