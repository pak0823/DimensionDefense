using UnityEngine;

public class EnemyPoolSpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int initialPoolSize = 10;
    private ObjectPool<Enemy> pool;

    void Awake()
    {
        pool = new ObjectPool<Enemy>(enemyPrefab, initialPoolSize, this.transform);
    }

    public Enemy Spawn(Vector3 pos)
    {
        return pool.Get(pos);
    }

    public void Despawn(Enemy e)
    {
        pool.Release(e);
    }
}
