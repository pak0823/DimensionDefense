using UnityEngine;

/// <summary>
/// PoolManager ������Ʈ������ Player ������ Ǯ�� ���� ����ϴ� ���г�
/// </summary>
public class PlayerPoolSpawner : CharacterSpawner
{
    [Header("Player Prefab")]
    [Tooltip("Ǯ���� Player ������")]
    public GameObject playerPrefab;

    /// <summary>
    /// ������ Player �������� ������Ʈ�� Ǯ���� ������ �ʱ� ��ġ�� ��ġ�ϰ� ��ȯ
    /// </summary>
    public override Character Spawn(Vector3 position)
    {
        // PoolManager�� SpawnFromPool ���� ȣ��
        return SpawnFromPool(playerPrefab, position);
    }
}
