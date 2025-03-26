using System.Collections;
using System.IO;
using UnityEngine;

public class Boss : Enemy, IDamageable
{
    public Transform attack_Point;
    public float attackRadius = 2.5f;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Transform groundCheck;
    // [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    private float fireDamageTimer = 0f;
    private float fireDamageInterval = 1f;
    private int direction = 1;
    private Animator animator;
    // private bool isAttacking = false;
    private bool is_Chasing = false;
    private GateController gate;
    // private float lastAttackTime = 0f;
    private string savePath;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        gate = FindAnyObjectByType<GateController>();

        string directoryPath = Path.Combine(Application.persistentDataPath, "GameData");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        savePath = Path.Combine(directoryPath, "bossData.json");

        LoadData();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(groundCheck.position, player.position);

        float distanceToPlayerX = Mathf.Abs(player.position.x - transform.position.x);

        fireDamageTimer += Time.deltaTime;

        if (fireDamageTimer >= fireDamageInterval)
        {

            Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, 4, playerLayer);
            if (hits != null)
            {
                DealDamage(MagicDame, hits);
                Debug.Log("Take Dame Fire");
            }

            fireDamageTimer = 0;
        }

        if (distanceToPlayerX < 0.5f)
        {
            isChasing = false;
            isAttacking = false;
            animator.SetBool("isWalking", false);
            //return;
        }
        else
        {
            if (PlayerInAttackRange())
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    animator.SetTrigger("Attack");
                }
            }
            else if (CheckInRange())
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }

    }
    protected override void Patrol()
    {
        isChasing = false;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        Vector2 obstacleCheckDirection = direction > 0 ? Vector2.right : Vector2.left;
        bool isObstacleAhead = Physics2D.Raycast(groundCheck.position, obstacleCheckDirection, 6f, groundLayer);

        if (!isGroundAhead || isObstacleAhead)
        {
            Flip();
        }

        transform.position += new Vector3(direction * WalkSpeed * Time.deltaTime, 0, 0);
    }

    protected override bool CheckInRange()
    {
        float distanceToPlayer = Vector2.Distance(groundCheck.position, player.position);
        return distanceToPlayer < detectionRange;
    }

    protected bool PlayerInAttackRange()
    {
        float distanceToPlayer = Vector2.Distance(attack_Point.position, player.position);
        return distanceToPlayer < attackRadius;
    }

    protected void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;
        animator.SetBool("isWalking", true);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if ((directionToPlayer.x > 0 && direction < 0) || (directionToPlayer.x < 0 && direction > 0))
        {
            Flip();
        }

        transform.position += new Vector3(Mathf.Sign(directionToPlayer.x) * RunSpeed * Time.deltaTime, 0, 0);

    }

    protected override void Attack()
    {
        isAttacking = true;
        isChasing = false;

        lastAttackTime = Time.time;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attack_Point.position, attackRadius, playerLayer);
        DealDamage(PhysicalDame, hits);
    }

    private void OnDrawGizmos()
    {
        if (attack_Point != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 4);
        }
    }

    protected override void Flip()
    {
        direction *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        healthBar.transform.localScale = new Vector3(-healthBar.transform.localScale.x, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
    protected override void Die()
    {
        BossData data = new BossData { health = 0, isDead = true };
        File.WriteAllText(savePath, JsonUtility.ToJson(data));
        animator.SetTrigger("Die");
        gate.OpenGate();
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        //Enemy_Pool enemy_Pool = Object.FindFirstObjectByType<Enemy_Pool>();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        //ResetState();
        //enemy_Pool.ReturnToPool(gameObject);
        //GameObject.Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        SaveData();
        healthBar.fillAmount = currentHealth / Hp;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void DealDamage(float dame, Collider2D[] hits)
    {
        //Collider2D[] hits = Physics2D.OverlapCircleAll(attack_Point.position, attackRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(PhysicalDame);
                Debug.Log("Take Dame");
                return;
            }
        }

    }

    private void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            BossData data = JsonUtility.FromJson<BossData>(json);

            currentHealth = data.health;
            if (data.isDead)
            {
                gameObject.SetActive(false);
            }
            healthBar.fillAmount = currentHealth / Hp;
        }
        else
        {
            currentHealth = Hp;
        }
    }

    private void SaveData()
    {
        BossData data = new BossData { health = currentHealth, isDead = false };
        File.WriteAllText(savePath, JsonUtility.ToJson(data));
    }
}
