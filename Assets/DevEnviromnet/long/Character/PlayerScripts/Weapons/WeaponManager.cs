using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    [SerializeField] private List<Weapon> _unlockedWeapons = new List<Weapon>();
    [SerializeField] private int  _currentWeaponIndex = 0;
    [SerializeField] private Weapon[] defaultWeapons;
    public Weapon CurrentWeapon => _unlockedWeapons[_currentWeaponIndex];

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




}
