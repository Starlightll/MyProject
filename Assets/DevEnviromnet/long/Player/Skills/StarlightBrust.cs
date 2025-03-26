using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/StarlightBrust")]
public class StarlightBrust : Skill
{
    public override void ActivateSkill(PlayerController player)
    {
        player.Stats.currentMana -= manaCost;
        player.StartCoroutine(PerformStarlightBrust(player));
    }

    public override bool CanActiveSkill(PlayerController player)
    {
        return player.Stats.currentMana >= manaCost && player.PlayerStateMachine.CurrentState is not PlayerDashState;
    }

    private IEnumerator PerformStarlightBrust(PlayerController player)
    {
        Debug.Log("Starlight Brust");
        float playerDirection = player.transform.localScale.x > 0 ? 1f : -1f;
        
        Vector2 attackPosition = player.attackPoint.position + new Vector3(playerDirection * 1.5f, 0f, 0f);

        Vector3 eularRotation = new Vector3(0, playerDirection == 1?180:0, skillEffects[0].transform.localEulerAngles.z);

        //Convert to Quaternion
        Quaternion rotation = Quaternion.Euler(eularRotation);
        GameObject skillEffect = skillEffects[0];
        GameObject effect = Instantiate(skillEffect, attackPosition, rotation);
        // skillEffect.transform.lo
        yield return new WaitForSeconds(skillDuration);
        Destroy(effect);
    }


}
