using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBarManager : MonoBehaviour
{
    [SerializeField] private Image staminaFill;
    [SerializeField] private Image staminaFillEase;
    [SerializeField] public float fillSpeed = 1f;
    [SerializeField] public float fillEaseSpeed = 0.1f;
    [SerializeField] private PlayerStats _stats;

    public float timeBeforeEase = 1f;
    private float easeTimer = 0f;
    private bool isEasing = false;
     private float previousManaRatio;
    private float currentVelocity;

    public PlayerStats Stats => _stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Stats.currentStamina = Stats.maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        float staminaRatio = Stats.currentStamina / Stats.maxStamina;
        float epsilon = 0.0001f;
        
        if(Mathf.Abs(staminaFill.fillAmount - staminaRatio) > epsilon)
        {
           if(staminaFill.fillAmount < staminaRatio)
            {
                staminaFill.fillAmount = Mathf.SmoothDamp(
                    staminaFill.fillAmount, 
                    staminaRatio, 
                    ref currentVelocity, 
                    fillSpeed
                );
            }else{
                staminaFill.fillAmount = staminaRatio;
            }
            easeTimer = 0f;
            if(staminaRatio > previousManaRatio)
            {
                staminaFillEase.fillAmount = staminaRatio;
            }
        }

        previousManaRatio = staminaRatio;
        
        easeTimer += Time.deltaTime;
        if(Mathf.Abs(staminaFillEase.fillAmount - staminaRatio) > epsilon && easeTimer >= timeBeforeEase) 
        {
            if(staminaRatio < staminaFillEase.fillAmount)
            {
                staminaFillEase.fillAmount = Mathf.SmoothDamp(
                    staminaFillEase.fillAmount, 
                    staminaRatio, 
                    ref currentVelocity, 
                    fillEaseSpeed
                );
            }
        }
    }
}
