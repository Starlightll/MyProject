using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/SpearOfNova")]
public class SpearOfNova : Weapon
{

    public override float CalculateTimeBetweenAttacks()
    {
        throw new System.NotImplementedException();
    }

    public override void PerformAttack(Transform attackPoint, LayerMask enemyLayer, ref int comboCounter)
    {
        throw new System.NotImplementedException();
    }
}
