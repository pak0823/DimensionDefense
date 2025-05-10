// PoolManager.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CharacterDefinition ����Ʈ�� ������� �ڵ����� Ǯ�� �����������ϴ� �Ŵ���
/// </summary>
public class PoolManager : MonoBehaviour
{

    [Header("Character Definitions for Pooling")]
    [Tooltip("�� ����Ʈ�� ��ϵ� ��� CharacterDefinition.prefab�� ���� Ǯ�� �����մϴ�.")]
    public List<CharacterDefinition> definitions;

    [Header("Pool Containers")]
    [Tooltip("Player ���� Ǯ �ν��Ͻ����� ���� Transform")]
    public Transform playerPoolContainer;
    [Tooltip("Enemy ���� Ǯ �ν��Ͻ����� ���� Transform")]
    public Transform enemyPoolContainer;

    [Header("Default Pool Settings")]
    [Tooltip("Ǯ ���� �� �⺻���� �Ҵ��� �ʱ� ũ���Դϴ�.")]
    public int defaultPoolSize = 10;

    // prefab �� �ش� Character ���� Ǯ
    private Dictionary<GameObject, ObjectPool<Character>> poolDict;

    void Awake()
    {
        if (Shared.PoolManager != null && Shared.PoolManager != this)
        {
            Destroy(gameObject);
            return;
        }
        Shared.PoolManager = this;
        poolDict = new Dictionary<GameObject, ObjectPool<Character>>();
        CreatePools();
    }

    // CharacterDefinition ����Ʈ�� ��ȸ�ϸ� Ǯ ����
    private void CreatePools()
    {
        //foreach (var def in definitions)
        //{
        //    var prefab = def.prefab;
        //    if (prefab == null) continue;

        //    // Character ������Ʈ�� �ִ� �����ո� Ǯ ����
        //    var comp = prefab.GetComponent<Character>();
        //    if (comp != null && !poolDict.ContainsKey(prefab))
        //    {
        //        var pool = new ObjectPool<Character>(comp, defaultPoolSize, this.transform);
        //        poolDict.Add(prefab, pool);
        //    }
        //}
        foreach (var def in definitions)
        {
            var prefab = def.prefab;
            if (prefab == null) continue;

            // Character ������Ʈ�� �ִ� �����ո� Ǯ ����
            var comp = prefab.GetComponent<Character>();
            if (comp != null && !poolDict.ContainsKey(prefab))
            {
                // Ǯ�� �ν��Ͻ��� ��ġ�� �θ� �����̳� ����
                Transform parent = def.isEnemy ? enemyPoolContainer : playerPoolContainer;
                if (parent == null)
                {
                    // �����̳ʰ� ���Ҵ�� ��� �⺻ PoolManager ������Ʈ��
                    parent = this.transform;
                }

                // �ش� �����̳ʸ� �θ�� Ǯ ����
                var pool = new ObjectPool<Character>(comp, defaultPoolSize, parent);
                poolDict.Add(prefab, pool);
            }
        }
    }

    /// <summary>
    /// Ǯ���� GameObject �ν��Ͻ��� �����ų�, �������� ������ Instantiate �մϴ�.
    /// </summary>
    public GameObject Spawn(GameObject prefab, Vector3 position)
    {
        if (poolDict.TryGetValue(prefab, out var pool))
        {
            var character = pool.Get(position);
            return character.gameObject;
        }
        // Ǯ�� ������ �Ϲ� Instantiate
        var go = Instantiate(prefab, position, Quaternion.identity);
        return go;
    }

    /// <summary>
    /// ������Ʈ�� Ǯ�� ��ȯ(Deactivate �� �ش� �����̳� ������ ����)
    /// </summary>
    public void Return(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);

        var charComp = go.GetComponent<Character>();
        if (charComp != null)
        {
            var prefab = charComp.definitionPrefab;
            if (prefab != null && poolDict.TryGetValue(prefab, out var pool))
            {
                // ���� Ǯ �����̳�(Parent)�� ��������
                go.transform.SetParent(pool.parent);
                return;
            }
        }
        // Ǯ�� ������ �⺻ PoolManager �Ʒ���
        go.transform.SetParent(this.transform);
    }
}
