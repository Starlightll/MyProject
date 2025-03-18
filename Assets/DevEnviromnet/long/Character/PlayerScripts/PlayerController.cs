using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]   
public class PlayerController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private PlayerStateMachine _stateMachine;
    [SerializeField] private SkillManager _skillManager;
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerConfigs _configs;


    public PlayerStats Stats => _stats;
    public PlayerInput Input => _input;
    public PlayerConfigs Configs => _configs;

    public PlayerState CurrentState => _stateMachine.CurrentState;
    public Weapon CurrentWeapon => _weaponManager.CurrentWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Current Health: " + Stats.maxHealth);
        _skillManager.UpdateSkills();
    }


    private void FixedUpdate()
    {
        // _stateMachine.;
    }
}
