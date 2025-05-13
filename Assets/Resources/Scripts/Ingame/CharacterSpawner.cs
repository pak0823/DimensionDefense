// CharacterSpawner.cs
using UnityEngine;

/// <summary>
/// ĳ���� ���г��� �⺻ �������̽��� ������Ʈ�� Ǯ�� ���� ����
/// </summary>
public abstract class CharacterSpawner : MonoBehaviour
{
    /// <summary>
    /// ������Ʈ�� Ǯ���� ������ �ν��Ͻ��� ������ Character ������Ʈ�� ��ȯ
    /// </summary>
    protected Character SpawnFromPool(GameObject prefab, Vector3 position)
    {
        var go = Shared.PoolManager.SpawnCharacter(prefab, position);
        return go.GetComponent<Character>();
    }

    /// <summary>
    /// �־��� ��ġ�� ĳ���͸� �����ϰ� �ʱ�ȭ�ؼ� ��ȯ
    /// </summary>
    public abstract Character Spawn(Vector3 position);
}
