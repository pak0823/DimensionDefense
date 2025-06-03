using UnityEngine;

/// <summary>
/// PoolManager ������Ʈ������ Enemy ������ Ǯ�� ���� ����ϴ� ���г�
/// </summary>
public class EnemyPoolSpawner : CharacterSpawner
{
    [Header("Enemy Prefab")]
    [Tooltip("Ǯ���� Enemy ������")]
    public GameObject enemyPrefab;

    /// <summary>
    /// ������ Enemy �������� ������Ʈ�� Ǯ���� ������ �ʱ� ��ġ�� ��ġ�ϰ� ��ȯ
    /// </summary>
    public override Character Spawn(Vector3 position)
    {
        // PoolManager�� SpawnFromPool ���� ȣ��
        return SpawnFromPool(enemyPrefab, position);
    }
}
