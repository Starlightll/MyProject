using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillBarManagement : MonoBehaviour
{
    [SerializeField] private Image[] skillIcons;
     [SerializeField] private Image[] skillCooldownOverlay;
    [SerializeField] private Sprite nullSkillSlot;
    [SerializeField] private PlayerController playerController;
    private float cooldownTime;
    private float cooldownTimeRemaining;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Weapon weapon = playerController.CurrentWeapon;
        UpdateSkillBar(weapon);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.CurrentWeapon.skills[0].staminaCost > playerController.Stats.currentStamina){
            skillCooldownOverlay[0].fillAmount = 1;
        }else{
            skillCooldownOverlay[0].fillAmount = playerController.CurrentWeapon.skills[0].initialCooldown / playerController.CurrentWeapon.skills[0].cooldown;
        }
        if(playerController.CurrentWeapon.skills[1].staminaCost > playerController.Stats.currentStamina){
            skillCooldownOverlay[1].fillAmount = 1;
        }else{
        skillCooldownOverlay[1].fillAmount = playerController.CurrentWeapon.skills[1].initialCooldown / playerController.CurrentWeapon.skills[1].cooldown;
        }
    }

    public void UpdateSkillBar(Weapon weapon) {
        // Update the skill bar with the weapon's skill
        for (int i = 0; i < skillIcons.Length; i++) {
            if (i < weapon.skills.Length) {
                skillIcons[i].sprite = weapon.skills[i].icon == null ? nullSkillSlot: weapon.skills[i].icon;
            } else {
                skillIcons[i].sprite = nullSkillSlot;
                skillIcons[i].color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    public void SetCooldownOverlay(Weapon weapon)
    {
        for (int i = 0; i < weapon.skills.Length; i++)
        {
            skillCooldownOverlay[i].fillAmount = 0;
        }
    }

    
    public void StartCooldown(float time, int index)
    {
        cooldownTime = time;
        cooldownTimeRemaining = time;
        skillCooldownOverlay[index].fillAmount = 1;
        StartCoroutine(UpdateCooldown(index));
    }

    private IEnumerator UpdateCooldown(int index)
    {
        while (cooldownTimeRemaining > 0)
        {
            cooldownTimeRemaining -= Time.deltaTime;
            skillCooldownOverlay[index].fillAmount = cooldownTimeRemaining / cooldownTime;
            yield return null;
        }
    }   
}
