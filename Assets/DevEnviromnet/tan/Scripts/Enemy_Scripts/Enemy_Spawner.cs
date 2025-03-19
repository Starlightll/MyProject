using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    public List<Transform> _spawnPoints;
    public float _spawnInterval = 3f;
    private Enemy_Pool enemyPool;
    bool isSpawning = true;
    void Start()
    {
        StartCoroutine(InitializeAndSpawn());
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        while (isSpawning)
        {
            SpawnEnemies();
            yield return new WaitForSeconds(_spawnInterval);
        }
        isSpawning = false;
    }

    IEnumerator InitializeAndSpawn()
    {
        yield return new WaitForSeconds(0.1f);
        enemyPool = FindAnyObjectByType<Enemy_Pool>();

        if (enemyPool == null)
        {
            Debug.LogError("Enemy_Pool not found in the scene!");
            yield break;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    void SpawnEnemies()
    {
        if (enemyPool == null) return;

        // Get the enemy types list directly from the pool
        List<Enemy_Pool.EnemySpawnData> enemySpawnList = enemyPool.GetEnemyTypes();
        int spawnCount = Mathf.Min(enemySpawnList.Count, _spawnPoints.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            var enemyData = enemySpawnList[i];
            Vector3 spawnPosition = _spawnPoints[i].position;

            // Just call GetEnemy, the pool will handle availability internally
            enemyPool.GetEnemy(enemyData.enemyPrefab, spawnPosition);
        }
    }


}