// PlayerPoolSpawner.cs
using UnityEngine;

public class PlayerPoolSpawner : CharacterSpawner
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private int initialPoolSize = 1;
    private ObjectPool<Player> pool;

    void Awake()
    {
        Debug.Log("playerPrefab: " + playerPrefab);
        Debug.Log("initialPoolSize: " + initialPoolSize);
        pool = new ObjectPool<Player>(playerPrefab, initialPoolSize, transform);
        Debug.Log("pool: " + pool);
    }

    public override Character Spawn(Vector3 pos)
    {
        return pool.Get(pos);
    }
}
