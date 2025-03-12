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
    [SerializeField] private float groundCheckDistance = 3f; 

    [SerializeField] private List<EnemyBase> groundEnemies;
    [SerializeField] private List<EnemyBase> airEnemies;

    private Dictionary<EnemyBase, ObjectPool<EnemyBase>> enemyPools = new Dictionary<EnemyBase, ObjectPool<EnemyBase>>();
    private Dictionary<Transform, EnemyBase> activeEnemies = new Dictionary<Transform, EnemyBase>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var enemy in groundEnemies)
        {
            enemyPools[enemy] = new ObjectPool<EnemyBase>(enemy, poolSize, transform);
        }

        foreach (var enemy in airEnemies)
        {
            enemyPools[enemy] = new ObjectPool<EnemyBase>(enemy, poolSize, transform);
        }

        foreach (var spawnPoint in spawnPoints)
        {
            activeEnemies[spawnPoint] = null;
        }

        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (activeEnemies[spawnPoint] == null)
            {
                bool hasGround = CheckGround(spawnPoint.position);
                List<EnemyBase> validEnemies = hasGround ? groundEnemies : airEnemies;

                if (validEnemies.Count > 0)
                {
                    int randomIndex = Random.Range(0, validEnemies.Count);
                    EnemyBase enemyPrefab = validEnemies[randomIndex];

                    if (enemyPools.TryGetValue(enemyPrefab, out var pool))
                    {
                        EnemyBase enemy = pool.GetObject();
                        enemy.Initialize(spawnPoint.position);
                        activeEnemies[spawnPoint] = enemy;
                    }
                }
            }
        }
    }

    private bool CheckGround(Vector3 position)
    {
        // Kiểm tra khoảng cách đến mặt đất
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            return true; // Có mặt đất trong khoảng cách cho phép
        }
        return false; // Không có mặt đất gần đó
    }

    public void ReturnEnemyToPool(EnemyBase enemy)
    {
        StartCoroutine(RespawnEnemy(enemy));
    }

    private IEnumerator RespawnEnemy(EnemyBase enemy)
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

        yield return new WaitForSeconds(respawnDelay);

        if (enemySpawnPoint != null && activeEnemies[enemySpawnPoint] == null)
        {
            bool hasGround = CheckGround(enemySpawnPoint.position);
            List<EnemyBase> validEnemies = hasGround ? groundEnemies : airEnemies;

            if (validEnemies.Count > 0)
            {
                int randomIndex = Random.Range(0, validEnemies.Count);
                EnemyBase enemyPrefab = validEnemies[randomIndex];

                if (enemyPools.TryGetValue(enemyPrefab, out var pool))
                {
                    EnemyBase newEnemy = pool.GetObject();
                    newEnemy.Initialize(enemySpawnPoint.position);
                    activeEnemies[enemySpawnPoint] = newEnemy;
                }
            }
        }
    }
}
