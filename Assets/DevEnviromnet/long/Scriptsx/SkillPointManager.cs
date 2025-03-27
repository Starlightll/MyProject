using TMPro;
using UnityEngine;

public class SkillPointManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private PlayerStats stats;
    void Awake()
    {
        //Set the skill point text to "Points: " + stats.skillPoints"
        pointText.text = "Points: " + stats.skillPoints;
    }

    // Update is called once per frame
    void Update()
    {
        pointText.text = "Points: " + stats.skillPoints;
    }

    public void AddDamage()
    {
        //Add 1 to the player's damage
        stats.attackDamage += 2;
        //Subtract 1 from the player's skill points
        stats.skillPoints -= 1;
    }

    public void AddHealth()
    {
        //Add 1 to the player's max health
        stats.maxHealth += 10;
        //Subtract 1 from the player's skill points
        stats.skillPoints -= 1;
    }

    public void AddStamina()
    {
        //Add 1 to the player's max stamina
        stats.maxStamina += 10;
        //Subtract 1 from the player's skill points
        stats.skillPoints -= 1;
    }

    public void AddDefense()
    {
        //Add 1 to the player's defense
        stats.defense += 2;
        //Subtract 1 from the player's skill points
        stats.skillPoints -= 1;
    }
}
