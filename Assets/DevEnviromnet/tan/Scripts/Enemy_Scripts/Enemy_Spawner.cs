using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    public List<Transform> _spawnPoints;
    public float _spawnInterval = 3f;
    int time = 0;

    private Enemy_Pool enemyPool;

    void Start()
    {
        enemyPool = FindFirstObjectByType<Enemy_Pool>();
        StartCoroutine(InitializeAndSpawn());
    }


    IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            SpawnEnemies();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    IEnumerator InitializeAndSpawn()
    {
        yield return new WaitForSeconds(0.1f);

        enemyPool = FindAnyObjectByType<Enemy_Pool>();

        StartCoroutine(SpawnEnemiesRoutine());
    }

    void SpawnEnemies()
    {
        if (enemyPool == null) return;

        List<Enemy_Pool.EnemySpawnData> enemySpawnList = enemyPool.enemyTypes;

        for (int i = 0; i < enemySpawnList.Count; i++)
        {
            var enemyData = enemySpawnList[i];

            if (i < _spawnPoints.Count)
            {
                Vector3 spawnPosition = _spawnPoints[i].position;
                enemyPool.GetEnemy(enemyData.enemyPrefab, spawnPosition);
            }
            else
            {
                Debug.LogWarning("null");
            }
        }
    }
}
