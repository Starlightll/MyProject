using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public List<GameObject> enemyPrefabs;
    private Dictionary<string, ObjectPool<Enemy>> enemyPools = new Dictionary<string, ObjectPool<Enemy>>();

    private void Awake()
    {
        Instance = this;
    }

    public void CreatePool(string enemyType, GameObject prefab, int poolSize)
    {
        if (!enemyPools.ContainsKey(enemyType))
        {
            ObjectPool<Enemy> newPool = new ObjectPool<Enemy>(prefab.GetComponent<Enemy>(), poolSize, transform);
            enemyPools.Add(enemyType, newPool);
        }
    }

    public Enemy GetEnemy(string enemyType, Vector3 position)
    {
        if (enemyPools.ContainsKey(enemyType))
        {
            Enemy enemy = enemyPools[enemyType].GetObject();
            enemy.transform.position = position;
            return enemy;
        }
        return null;
    }

    public void ReturnEnemy(Enemy enemy)
    {
        enemyPools[enemy.GetType().Name].ReturnObject(enemy);
    }
}
