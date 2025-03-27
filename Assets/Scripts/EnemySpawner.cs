using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float respawnDelay = 30f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 5f;

    [SerializeField] private List<Enemy> groundEnemies;
    [SerializeField] private List<Enemy> airEnemies;

    private Dictionary<Enemy, ObjectPool<Enemy>> enemyPools = new Dictionary<Enemy, ObjectPool<Enemy>>();
    private Dictionary<Transform, Enemy> activeEnemies = new Dictionary<Transform, Enemy>();

    private Enemy_Pool1 enemyPool;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Tìm đối tượng Enemy_Pool trong cảnh
        enemyPool = FindObjectOfType<Enemy_Pool1>();
        
    }

    private void Start()
    {
        foreach (var enemy in groundEnemies)
            enemyPools[enemy] = new ObjectPool<Enemy>(enemy, poolSize, transform);

        foreach (var enemy in airEnemies)
            enemyPools[enemy] = new ObjectPool<Enemy>(enemy, poolSize, transform);

        foreach (var spawnPoint in spawnPoints)
            activeEnemies[spawnPoint] = null;

        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    public void SpawnEnemy()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (activeEnemies[spawnPoint] == null)
            {
                bool hasGround = CheckGround(spawnPoint.position);
                List<Enemy> validEnemies = hasGround ? groundEnemies : airEnemies;

                if (validEnemies.Count > 0)
                {
                    int randomIndex = Random.Range(0, validEnemies.Count);
                    Enemy enemyPrefab = validEnemies[randomIndex];

                    if (enemyPools.TryGetValue(enemyPrefab, out var pool))
                    {
                        Enemy enemy = pool.GetObject();

                        if (enemy.spawnPosition == Vector2.zero)
                        {
                            enemy.spawnPosition = spawnPoint.position;
                        }
                        enemy.transform.position = enemy.spawnPosition;

                        activeEnemies[spawnPoint] = enemy;
                    }
                }
            }
        }
    }

    bool CheckGround(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, groundCheckRadius, groundLayer);
        return hit.collider != null;
    }

    public void ReturnEnemyToPool(Enemy enemy)
    {
        PooledEnemy pooledEnemy = enemy.GetComponent<PooledEnemy>();
        if (pooledEnemy == null)
        {
            Debug.LogWarning("Enemy doesn't have PooledEnemy component");
            return;
        }

        if (enemyPool != null)
        {
            enemyPool.ReturnToPool(enemy.gameObject);
        }
    }

    private IEnumerator RespawnEnemy(Enemy enemy)
    {
        Transform enemySpawnPoint = null;

        foreach (var entry in activeEnemies)
        {
            if (entry.Value == enemy)
            {
                enemySpawnPoint = entry.Key;
                activeEnemies[enemySpawnPoint] = null;
                break;
            }
        }

        // Đợi trước khi spawn lại
        yield return new WaitForSeconds(respawnDelay);

        SpawnEnemy();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var spawnPoint in spawnPoints)
        {
            Gizmos.DrawSphere(spawnPoint.position, 0.2f);
        }
    }
}
