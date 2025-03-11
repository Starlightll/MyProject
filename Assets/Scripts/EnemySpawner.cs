using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private EnemyBase[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float respawnDelay = 30f;

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
        
        foreach (var enemyPrefab in enemyPrefabs)
        {
            enemyPools[enemyPrefab] = new ObjectPool<EnemyBase>(enemyPrefab, poolSize, transform);
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
                int randomIndex = Random.Range(0, enemyPrefabs.Length);
                EnemyBase enemyPrefab = enemyPrefabs[randomIndex];

                EnemyBase enemy = enemyPools[enemyPrefab].GetObject();
                enemy.Initialize(spawnPoint.position);
                activeEnemies[spawnPoint] = enemy;
            }
        }
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
            
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            EnemyBase enemyPrefab = enemyPrefabs[randomIndex];

            EnemyBase newEnemy = enemyPools[enemyPrefab].GetObject();
            newEnemy.Initialize(enemySpawnPoint.position);
            activeEnemies[enemySpawnPoint] = newEnemy;
        }
    }
}
