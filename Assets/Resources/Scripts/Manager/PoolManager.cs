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
    public int characterPoolSize = 5;    //ĳ���� Ǯ ���� �� �Ҵ��� ũ��
    public int projectileSize = 3;      //����ü �� ���� ���� Ǯ ���� �� �Ҵ��� ũ��

    // prefab �� [�ش� Objedt] ���� Ǯ
    private Dictionary<GameObject, ObjectPool<Character>> poolDict;
    private Dictionary<GameObject, ObjectPool<Projectile>> projectilePoolDict;
    private Dictionary<GameObject, ObjectPool<Range>> RnagePoolDict;

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
                var pool = new ObjectPool<Character>(comp, characterPoolSize, parent);
                poolDict.Add(prefab, pool);
            }
        }

        projectilePoolDict = new Dictionary<GameObject, ObjectPool<Projectile>>();
        RnagePoolDict = new Dictionary<GameObject, ObjectPool<Range>>();

        if(projectilePoolDict != null)
        {
            foreach (var projPrefab in projectilePrefabs)
            {
                if (projPrefab == null) continue;
                var comp = projPrefab.GetComponent<Projectile>();
                if (comp == null) continue;

                projectilePoolDict[projPrefab] =
                  new ObjectPool<Projectile>(comp, projectileSize, projectileContainer);
            }
        }

        if (RnagePoolDict != null)
        {
            foreach (var projPrefab in projectilePrefabs)
            {
                if (projPrefab == null) continue;
                var comp = projPrefab.GetComponent<Range>();
                if (comp == null) continue;

                RnagePoolDict[projPrefab] =
                  new ObjectPool<Range>(comp, projectileSize, projectileContainer);
            }
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
        if (projectilePoolDict.TryGetValue(prefab, out var pool_projectile))
            return pool_projectile.Get(pos).gameObject;

        if (RnagePoolDict.TryGetValue(prefab, out var pool_lightning))
            return pool_lightning.Get(pos).gameObject;

        return Instantiate(prefab, pos, Quaternion.identity);
    }

    // Ǯ�� ��ȯ�ϱ�
    public void ReturnProjectile(GameObject _poolObj)
    {
        if (_poolObj == null) return;

        var projectile = _poolObj.GetComponent<Projectile>();
        var lightning = _poolObj.GetComponent<Range>();

        if (projectile != null && projectilePoolDict.TryGetValue(projectile.definitionPrefab, out var pool_projectile))
        {
            _poolObj.SetActive(false);
            _poolObj.transform.SetParent(pool_projectile.parent, worldPositionStays: false);
            return;
        }

        if (lightning != null && projectilePoolDict.TryGetValue(lightning.definitionPrefab, out var pool_lightning))
        {
            _poolObj.SetActive(false);
            _poolObj.transform.SetParent(pool_lightning.parent, worldPositionStays: false);
            return;
        }

        Destroy(_poolObj);
    }
}
