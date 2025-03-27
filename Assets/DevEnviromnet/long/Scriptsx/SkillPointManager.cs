using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPointManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI defenseText;
        [SerializeField] private TextMeshProUGUI healthText;

    private int tempDamagePointUsed = 0;
    private int tempHealthPointUsed = 0;
    private int tempDefensePointUsed = 0;
    private int totalPointsUsed = 0;
    void Awake()
    {
        //Set the skill point text to "Points: " + stats.skillPoints"
        pointText.text = "Points: " + stats.skillPoints;
        damageText.text = "<color=#FF003E>Damage: " +stats.attackDamage;
        healthText.text = "<color=#77FF00>Max Health: " +stats.maxHealth;
        defenseText.text = "<color=#00FFCE>Defense: " +stats.defense;
    }

    // Update is called once per frame
    void Update()
    {
        pointText.text = "Points: " + (stats.skillPoints - totalPointsUsed);
        if(tempDamagePointUsed > 0)
        {
            //Set the damage text to "Damage: " + stats.attackDamage + "(" + tempDamagePointUsed + ")" with tempDamagePointUsed with yellow color
            damageText.text = "<color=#FF003E>Damage: " + (stats.attackDamage + tempDamagePointUsed * 2) + "</color>" + " <color=yellow>( +" + tempDamagePointUsed * 2 + " )</color>";
        }else{
            damageText.text = "<color=#FF003E>Damage: " +stats.attackDamage + "</color>";
        }
        if(tempHealthPointUsed > 0)
        {
            healthText.text = "<color=#77FF00>Health: " +(stats.maxHealth + tempHealthPointUsed * 10) + "</color>" +  " <color=yellow>( +" + tempHealthPointUsed * 10 + " )</color>";
        }else{
            healthText.text = "<color=#77FF00>Health: " +stats.maxHealth + "</color>";
        }
        if (tempDefensePointUsed > 0)
        {
            defenseText.text = "<color=#00FFCE>Defense: " +(stats.defense + tempDefensePointUsed * 1) + "</color>" +  " <color=yellow>( +" + tempDefensePointUsed * 1 + " )</color>";
        }else{
            defenseText.text = "<color=#00FFCE>Defense: " +stats.defense + "</color>";
        }

    }

    public void AddDamage()
    {
        Debug.Log("Add Damage");
        if(stats.skillPoints - totalPointsUsed <= 0)
        {
            return;
        }
        tempDamagePointUsed += 1;
        totalPointsUsed += 1;
    }

    public void AddHealth()
    {
        if(stats.skillPoints - totalPointsUsed <= 0)
        {
            return;
        }
        tempHealthPointUsed += 1;
        totalPointsUsed += 1;
    }

    public void AddDefense()
    {
        if(stats.skillPoints - totalPointsUsed <= 0)
        {
            return;
        }
        tempDefensePointUsed += 1;
        totalPointsUsed += 1;
    }

    public void ApplyChanges()
    {
        stats.attackDamage += tempDamagePointUsed * 2;
        stats.maxHealth += tempHealthPointUsed * 10;
        stats.defense += tempDefensePointUsed * 1;
        stats.skillPoints -= tempDamagePointUsed + tempHealthPointUsed + tempDefensePointUsed;
        tempDamagePointUsed = 0;
        tempHealthPointUsed = 0;
        tempDefensePointUsed = 0;
        totalPointsUsed = 0;
    }

    public void CancelChanges()
    {
        totalPointsUsed = 0;
        
        tempDamagePointUsed = 0;
        tempHealthPointUsed = 0;
        tempDefensePointUsed = 0;
    }
}
