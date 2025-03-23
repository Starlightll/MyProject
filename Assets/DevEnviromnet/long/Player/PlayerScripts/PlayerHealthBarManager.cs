using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarManager : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Image healthFillEase;
    [SerializeField] public float fillSpeed = 1f;
    [SerializeField] public float fillEaseSpeed = 0.1f;
    [SerializeField] private PlayerStats _stats;

    public float timeBeforeEase = 1f;
    private float easeTimer = 0f;
    private bool isEasing = false;
     private float previousHealthRatio;
    private float currentVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // _stats.currentHealth = _stats.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        float healthRatio = _stats.currentHealth / _stats.maxHealth;
        float epsilon = 0.0001f;
        
        if(Mathf.Abs(healthFill.fillAmount - healthRatio) > epsilon)
        {
            if(healthFill.fillAmount < healthRatio)
            {
                healthFill.fillAmount = Mathf.SmoothDamp(
                    healthFill.fillAmount, 
                    healthRatio, 
                    ref currentVelocity, 
                    fillSpeed
                );
            }else{
                healthFill.fillAmount = healthRatio;
            }
            easeTimer = 0f;
            if(healthRatio > previousHealthRatio)
            {
                healthFillEase.fillAmount = healthRatio;
            }
        }

        previousHealthRatio = healthRatio;
        
        easeTimer += Time.deltaTime;
        if(Mathf.Abs(healthFillEase.fillAmount - healthRatio) > epsilon && easeTimer >= timeBeforeEase) 
        {
            if(healthRatio < healthFillEase.fillAmount)
            {
                healthFillEase.fillAmount = Mathf.SmoothDamp(
                    healthFillEase.fillAmount, 
                    healthRatio, 
                    ref currentVelocity, 
                    fillEaseSpeed
                );
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _stats.currentHealth -= damage;
        if (_stats.currentHealth <= 0)
        {
            _stats.currentHealth = 0;
        }
    }
}
