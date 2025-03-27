using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]   
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Dependencies")]
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private SkillManager _skillManager;
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private PlayerConfigs _configs;
    [SerializeField] public Rigidbody2D _rb;
    [SerializeField] public Animator _anim;
    [SerializeField] private PlayerHealthController _playerHealthController;
    [SerializeField] private PlayerMovementController _playerMovementController;

    private PlayerStateMachine _playerStateMachine;

    public PlayerStats Stats => _stats;
    public PlayerInput Input => _input;
    public PlayerConfigs Configs => _configs;

    public PlayerMovementController PlayerMovementController => _playerMovementController;
    public PlayerStateMachine PlayerStateMachine => _playerStateMachine;

    public Weapon CurrentWeapon => _weaponManager.CurrentWeapon;

    public Transform attackPoint;

    private float comboTimer = 0f;
    public int comboCounter = 0;
    private float attackTimer = 0f;
    private float attackCooldown = 0f;

    private Vector2 playerDirection;

    public float attackRange = 0;
    public bool isDead = false;
    public Vector3 checkpoint = new Vector3(0, 0, 0);

    // [Header("References")]
    // public Transform groundCheck;
    // public Transform wallCheck;
    // public Transform HeadCheck;
    // public LayerMask groundLayer;
    // public float GroundCheckSize = 0.1f;
    // public float HeadCheckSize = 0.1f;
    // public float wallCheckDistance = 0.5f;
    // public float gravityScale = 5f;
    // public float gravityScaleWallSlide = 1f;
    // public float coyoteTime = 0.2f;

    // [Header("System Variables")]
    // private bool isGrounded;
    // private bool isTouchingHead;
    // private bool isTouchingWall;
    // private bool isWallSliding;
    // private bool canDoubleJump;
    // private bool isDashing;
    // private float dashCooldownTimer;
    // private float dashTimer;
    // private bool facingRight = true;
    // private float horizontalInput;
    // private bool isOnWallJump = false;
    // private float coyoteTimeCounter;

    public Vector2 playerVelocity;




    private void Awake() {
        _playerStateMachine = new PlayerStateMachine(this);
        checkpoint = transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //Initialize the player state machine
        _playerStateMachine.Initialize(_playerStateMachine.idleState);
        if(_stats.isResetStatsNextTime)
        {
            _stats.ResetStats();
        }
        _stats.InitializeStats();
    }


    // Update is called once per frame
    void Update()
    {
        //Handle the player state machine
        _playerStateMachine.Execute();
        // Debug.Log("Current State: " + _playerStateMachine.CurrentState);

        if(_playerStateMachine.CurrentState is PlayerDeadState)
        {
            return;
        }
        
        if(_stats.currentHealth <= 0)
        {
            Debug.Log("Player is dead");
            _playerStateMachine.TransitionTo(new PlayerDeadState(this));
        }
        //Run player Skills manager
        _skillManager.UpdateSkills();

       
        //Test the player got hit by the enemy
        if(UnityEngine.Input.GetKeyDown(KeyCode.H))
        {
            // TakeDamage(Random.Range(2, 10));
            Stats.currentMana -= 10;
        }

         if(_playerStateMachine.CurrentState is PlayerDashState)
        {
            return;
        }
    
        comboTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        //Handle the player attack => Need to be moved HandleAttack() method
        if(_input.AttackPressed && attackTimer >= _weaponManager.CalculateTimeBetweenAttacks())
        {
           HandleAttack();
        }

        //Handle the player movement => Need to be moved HandleMovement() method
    }


    private void FixedUpdate()
    {
        // _stateMachine.;

    }

    
    private void HandleAttack() {
           Debug.Log("Attack Combo" + comboCounter);

            //Need to be complete this task next time
            //Move the attack logic to here
            //Calculate time here.
             if(comboTimer >= CurrentWeapon.attackCooldown)
            {
                Debug.Log("Combo Reset");
                comboCounter = 0;
                comboTimer = 0f;
            }
            float direction = transform.localScale.x > 0 ? 1 : -1;
            playerVelocity = new Vector2(_rb.linearVelocity.x == 0 ? 3 * direction: _rb.linearVelocity.x * 0.3f, /*Mathf.Abs(_rb.linearVelocity.y) * -0.3f*/ _rb.linearVelocity.y < 0? Mathf.Abs(_rb.linearVelocity.y) * -0.2f : _rb.linearVelocity.y *-0.01f);
            // Debug.Log("Player Velocity: " + playerVelocity);
            // _stateMachine.CurrentState(States.Attack);
            CurrentWeapon.PerformAttack(attackPoint, LayerMask.GetMask("Enemy"), ref comboCounter, this);
            comboTimer = 0f;
            attackTimer = 0f;
    }

    private void HandleMovement() {
        //Handle the player movement
        //NB: This method should be moved to the PlayerMovementSystem class
        //Need to be complete this task next time
    }

    //Draw the attack range of the player
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void TakeDamage(float damage)
    {
        // _stats.TakeDamage(damage);
        _anim.SetTrigger("Hurt");
        //Set color to red
        _playerHealthController.TakeDamage(damage);
        
    }
}
