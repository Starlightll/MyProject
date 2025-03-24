using UnityEngine;

public class PlayerStaminaController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Stamina Settings")]
    [SerializeField] private PlayerStats playerStats;
    public float timeBeforeStaminaRegen = 1f;
    public float regenAmount = 40f;

    private bool needToRegen = false;

    private float timer = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerStats.currentStamina < playerStats.maxStamina)
        {
            RegenStamina();
        }
    }

    private void RegenStamina()
    {
        timer += Time.deltaTime;
        if(timer >= timeBeforeStaminaRegen)
        {
            playerStats.currentStamina += regenAmount;
            timer = 0;
        }
    }
}
