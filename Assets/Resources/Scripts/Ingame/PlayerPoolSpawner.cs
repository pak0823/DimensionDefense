using UnityEngine;

/// <summary>
/// PoolManager 레지스트리에서 Player 프리팹 풀을 꺼내 사용하는 스패너
/// </summary>
public class PlayerPoolSpawner : CharacterSpawner
{
    [Header("Player Prefab")]
    [Tooltip("풀링할 Player 프리팹")]
    public GameObject playerPrefab;

    /// <summary>
    /// 지정된 Player 프리팹을 레지스트리 풀에서 꺼내어 초기 위치에 배치하고 반환
    /// </summary>
    public override Character Spawn(Vector3 position)
    {
        // PoolManager의 SpawnFromPool 헬퍼 호출
        return SpawnFromPool(playerPrefab, position);
    }
}
