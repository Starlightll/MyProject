
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Image[] skillCooldownOverlay;
    [SerializeField] private PlayerController playerController;
    private float cooldownTime;
    private float cooldownTimeRemaining;

    public void SetCooldownOverlay(Weapon weapon)
    {
        for (int i = 0; i < weapon.skills.Length; i++)
        {
            skillCooldownOverlay[i].fillAmount = 0;
        }
    }

    public void Update()
    {
        for(int i = 0; i < playerController.CurrentWeapon.skills.Length; i++)
        {
            // if(playerController.Stats.currentMana < playerController.CurrentWeapon.skills[i].manaCost || playerController.Stats.currentStamina < playerController.CurrentWeapon.skills[i].staminaCost)
            // {
            //     skillCooldownOverlay[i].fillAmount = 1;
            // }
        }
    }

    public void StartCooldown(float time, int index)
    {
        cooldownTime = time;
        cooldownTimeRemaining = time;
        skillCooldownOverlay[index].fillAmount = 1;
        StartCoroutine(UpdateCooldown(index));
    }

    // Update is called once per frame
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
