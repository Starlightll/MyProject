using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    [SerializeField] private List<Weapon> _unlockedWeapons = new List<Weapon>();
    [SerializeField] private int  _currentWeaponIndex = 0;
    [SerializeField] private Weapon[] defaultWeapons;
    public Weapon CurrentWeapon => _unlockedWeapons[_currentWeaponIndex];

    [SerializeField] private SkillBarManagement _skillBarManagement;
    
    private SkillManager _skillManager;
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(_unlockedWeapons.Count > 0)
            {
                SwitchWeapon(0);
                Debug.Log("Switched to weapon 1");
                _skillBarManagement.UpdateSkillBar(CurrentWeapon);

            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(_unlockedWeapons.Count > 1){
                SwitchWeapon(1);
                Debug.Log("Switched to weapon 2");
                _skillBarManagement.UpdateSkillBar(CurrentWeapon);
            }
        }
    }

    public void UnlockWeapon(Weapon weapon)
    {
        if(!_unlockedWeapons.Contains(weapon))
        {
            _unlockedWeapons.Add(weapon);
        }
    }

    public void SwitchWeapon(int index)
    {
        //_currentWeaponIndex = Mathf.Clamp(index, 0, _availableWeapons.Count - 1); Can use this instead of the if statement
        if(index < _unlockedWeapons.Count && index >= 0)
        {
            _currentWeaponIndex = index;
        }
    }

    public void AddWeapon(Weapon weapon)
    {
        if(!_unlockedWeapons.Contains(weapon))
            _unlockedWeapons.Add(weapon);
    }

    public float CalculateTimeBetweenAttacks()
    {
        //This is for Calculate final attacks speed or attack cooldown between attacks.
        //This function ned:
        // 1. Current weapon attack speed
        // 2. Player attack speed
        // 3. Weapon attack speed multiplier

        // result will be calculated as:
        // Player attack speed * player attack speed multiplier as 1
        // Weapon attack speed * weapon attack speed multiplier as 2
        // 1 + 2 + ... + n / max number of attack speed values  => percentage of attack speed
        // time between attacks * 1 - percentage of attack speed
        float weaponAttackSpeed = _playerController.CurrentWeapon.attackSpeed * _playerController.CurrentWeapon.attackSpeedMultiplier;
        float playerAttackSpeed = _playerController.Stats.attackSpeed;

        float percentageOfAttackSpeed = (weaponAttackSpeed + playerAttackSpeed) / _playerController.Stats.maxAttackSpeed;
        float timeBetweenAttacks = _playerController.CurrentWeapon.attackCooldown - _playerController.CurrentWeapon.attackCooldown * percentageOfAttackSpeed;
        Debug.Log("Time between attacks = " + weaponAttackSpeed + " + " + playerAttackSpeed + " / 200 = " + percentageOfAttackSpeed + " => " + timeBetweenAttacks);
        return timeBetweenAttacks;
    }
}
