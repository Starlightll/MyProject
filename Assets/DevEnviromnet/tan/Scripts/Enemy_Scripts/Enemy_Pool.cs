using System.Collections.Generic;
using UnityEngine;

public class Enemy_Pool : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public int poolSize;
    }

    public List<EnemySpawnData> enemyTypes;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (var enemyType in enemyTypes)
        {
            Queue<GameObject> enemyPool = new Queue<GameObject>();

            for (int i = 0; i < enemyType.poolSize; i++)
            {
                GameObject enemy = Instantiate(enemyType.enemyPrefab);
                enemy.SetActive(false);
                enemyPool.Enqueue(enemy);
            }

            poolDictionary.Add(enemyType.enemyPrefab, enemyPool);
        }
    }



    public GameObject GetEnemy(GameObject enemyPrefab, Vector3 spawnPosition)
    {
        if (!poolDictionary.ContainsKey(enemyPrefab))
        {
            return null;
        }

        if (poolDictionary[enemyPrefab].Count > 0)
        {
            GameObject enemy = poolDictionary[enemyPrefab].Dequeue();
            enemy.transform.position = spawnPosition;
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            return null;
        }
    }


    public void ReturnToPool(GameObject enemy, GameObject enemyPrefab)
    {
        enemy.SetActive(false);
        poolDictionary[enemyPrefab].Enqueue(enemy);
    }
}
