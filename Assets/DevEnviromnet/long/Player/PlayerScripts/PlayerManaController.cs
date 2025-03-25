using UnityEngine;

public class PlayerManaController : MonoBehaviour
{

    [Header("Mana Settings")]
    [SerializeField] private PlayerStats playerStats;
    public float timeToRegen = 1f;
    public float manaRegenAmount = 1f;

    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timeToRegen)
        {
            RegenMana();
            timer = 0;
        }
    }

    private void RegenMana()
    {
        if(playerStats.currentMana < playerStats.maxMana)
        {
            playerStats.currentMana += manaRegenAmount;
        }
    }

}
