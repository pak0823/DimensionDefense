using UnityEngine;

/// <summary>
/// PoolManager 레지스트리에서 Enemy 프리팹 풀을 꺼내 사용하는 스패너
/// </summary>
public class EnemyPoolSpawner : CharacterSpawner
{
    [Header("Enemy Prefab")]
    [Tooltip("풀링할 Enemy 프리팹")]
    public GameObject enemyPrefab;

    /// <summary>
    /// 지정된 Enemy 프리팹을 레지스트리 풀에서 꺼내어 초기 위치에 배치하고 반환
    /// </summary>
    public override Character Spawn(Vector3 position)
    {
        // PoolManager의 SpawnFromPool 헬퍼 호출
        return SpawnFromPool(enemyPrefab, position);
    }
}
