using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public List<GameObject> enemyPrefabs;
    private Dictionary<string, ObjectPool<EnemyBase>> enemyPools = new Dictionary<string, ObjectPool<EnemyBase>>();

    private void Awake()
    {
        Instance = this;
    }

    public void CreatePool(string enemyType, GameObject prefab, int poolSize)
    {
        if (!enemyPools.ContainsKey(enemyType))
        {
            ObjectPool<EnemyBase> newPool = new ObjectPool<EnemyBase>(prefab.GetComponent<EnemyBase>(), poolSize, transform);
            enemyPools.Add(enemyType, newPool);
        }
    }

    public EnemyBase GetEnemy(string enemyType, Vector3 position)
    {
        if (enemyPools.ContainsKey(enemyType))
        {
            EnemyBase enemy = enemyPools[enemyType].GetObject();
            enemy.transform.position = position;
            return enemy;
        }
        return null;
    }

    public void ReturnEnemy(EnemyBase enemy)
    {
        enemyPools[enemy.GetType().Name].ReturnObject(enemy);
    }
}
