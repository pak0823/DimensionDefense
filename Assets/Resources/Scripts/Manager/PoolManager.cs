// PoolManager.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CharacterDefinition ����Ʈ�� ������� �ڵ����� Ǯ�� �����������ϴ� �Ŵ���
/// </summary>
public class PoolManager : MonoBehaviour
{

    [Header("Character Definitions for Pooling")]
    public List<CharacterDefinition> definitions;   // Ǯ���� ĳ���� ������ ���

    [Header("Projectile Pooling")]
    public List<GameObject> projectilePrefabs;      // Ǯ���� ����ü ������ ���
    

    [Header("Pool Containers")]
    public Transform playerPoolContainer;     // Player Ǯ �ν��Ͻ����� ���� �����̳�
    public Transform enemyPoolContainer;      // Enemy Ǯ �ν��Ͻ����� ���� �����̳�
    public Transform projectileContainer;     // Projectile Ǯ �ν��Ͻ����� ���� �����̳�

    [Header("Default Pool Settings")]
    public int defaultPoolSize = 10;    //Ǯ ���� �� �⺻���� �Ҵ��� �ʱ� ũ��

    // prefab �� [�ش� Objedt] ���� Ǯ
    private Dictionary<GameObject, ObjectPool<Character>> poolDict;
    private Dictionary<GameObject, ObjectPool<Projectile>> projectilePoolDict;
    private Dictionary<GameObject, ObjectPool<Lightning>> LightningPoolDict;

    void Awake()
    {
        if (Shared.PoolManager != null && Shared.PoolManager != this)
        {
            Destroy(gameObject);
            return;
        }

        Shared.PoolManager = this;

        CreatePools();
    }

    // CharacterDefinition ����Ʈ�� ��ȸ�ϸ� Ǯ ����
    private void CreatePools()
    {
        poolDict = new Dictionary<GameObject, ObjectPool<Character>>();

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

        //projectilePoolDict = new Dictionary<GameObject, ObjectPool<Projectile>>();
        LightningPoolDict = new Dictionary<GameObject, ObjectPool<Lightning>>();

        //foreach (var projPrefab in projectilePrefabs)
        //{
        //    if (projPrefab == null) continue;
        //    var comp = projPrefab.GetComponent<Projectile>();
        //    if (comp == null) continue;

        //    projectilePoolDict[projPrefab] =
        //      new ObjectPool<Projectile>(comp, defaultPoolSize, projectileContainer);
        //}

        foreach (var projPrefab in projectilePrefabs)
        {
            if (projPrefab == null) continue;
            var comp = projPrefab.GetComponent<Lightning>();
            if (comp == null) continue;

            LightningPoolDict[projPrefab] =
              new ObjectPool<Lightning>(comp, defaultPoolSize, projectileContainer);
        }
    }

    /// <summary>
    /// Ǯ���� GameObject �ν��Ͻ��� �����ų�, �������� ������ Instantiate �մϴ�.
    /// </summary>
    public GameObject SpawnCharacter(GameObject prefab, Vector3 position)
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
    public void ReturnCharacter(GameObject _poolObj)
    {
        if (_poolObj == null) return;
        _poolObj.SetActive(false);

        var charComp = _poolObj.GetComponent<Character>();
        if (charComp != null)
        {
            var prefab = charComp.definitionPrefab;
            if (prefab != null && poolDict.TryGetValue(prefab, out var pool))
            {
                // ���� Ǯ �����̳�(Parent)�� ��������
                _poolObj.transform.SetParent(pool.parent);
                return;
            }
        }
        // Ǯ�� ������ �⺻ PoolManager �Ʒ���
        _poolObj.transform.SetParent(this.transform);
    }

    // Ǯ���� ������
    public GameObject SpawnProjectile(GameObject prefab, Vector3 pos)
    {
        //if (projectilePoolDict.TryGetValue(prefab, out var pool))
        //    return pool.Get(pos).gameObject;
        //return Instantiate(prefab, pos, Quaternion.identity);

        if (LightningPoolDict.TryGetValue(prefab, out var pool))
            return pool.Get(pos).gameObject;
        return Instantiate(prefab, pos, Quaternion.identity);
    }

    // Ǯ�� ��ȯ�ϱ�
    public void ReturnProjectile(GameObject _poolObj)
    {
        //if (_poolObj == null) return;
        //var proj = _poolObj.GetComponent<Projectile>();
        //if (proj != null && projectilePoolDict.TryGetValue(proj.definitionPrefab, out var pool))
        //{
        //    _poolObj.SetActive(false);
        //    _poolObj.transform.SetParent(pool.parent, worldPositionStays: false);
        //    return;
        //}

        if (_poolObj == null) return;
        var proj = _poolObj.GetComponent<Lightning>();
        if (proj != null && projectilePoolDict.TryGetValue(proj.definitionPrefab, out var pool))
        {
            _poolObj.SetActive(false);
            _poolObj.transform.SetParent(pool.parent, worldPositionStays: false);
            return;
        }
        Destroy(_poolObj);
    }
}
