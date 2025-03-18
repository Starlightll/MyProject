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
    [SerializeField] public Rigidbody2D _rb;
    [SerializeField] public Animator _anim;


    public PlayerStats Stats => _stats;
    public PlayerInput Input => _input;
    public PlayerConfigs Configs => _configs;

    public PlayerState CurrentState => _stateMachine.CurrentState;
    public Weapon CurrentWeapon => _weaponManager.CurrentWeapon;

    public Transform attackPoint;

    private float comboTimer = 0f;
    private int comboCounter = 0;

    private Vector2 playerDirection;

    public float attackRange = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        // if (_input.Dash)
        // {
        //     _movement.Dash();
        // }
        comboTimer += Time.deltaTime;
        if(_input.AttackPressed)
        {
            if(comboTimer >= CurrentWeapon.comboResetTime)
            {
                comboCounter = 0;
                comboTimer = 0f;
            }
            // _stateMachine.CurrentState(States.Attack);
            CurrentWeapon.PerformAttack(attackPoint, LayerMask.GetMask("Enemy"), ref comboCounter);
            comboTimer = 0f;
        }
    }


    private void FixedUpdate()
    {
        // _stateMachine.;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
