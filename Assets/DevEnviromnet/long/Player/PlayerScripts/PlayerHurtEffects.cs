using System.Collections;
using UnityEngine;

public class PlayerHurtEffects : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderer;
    public Color hurtColor = Color.red;
    public float flashDuration = 0.1f;

    private void Start()
    {
        // spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FlashRed()
    {
        
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        foreach (SpriteRenderer sprite in spriteRenderer)
        {
            sprite.color = hurtColor;
        }
        
        yield return new WaitForSeconds(flashDuration);
    
        foreach (SpriteRenderer sprite in spriteRenderer)
        {
            sprite.color = Color.white;
        }
    }
}
