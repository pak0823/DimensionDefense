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
    private Dictionary<GameObject, ObjectPool<Character>> characterPoolDict;
    private Dictionary<GameObject, ObjectPool<Projectile>> projectilePoolDict;
    private Dictionary<GameObject, ObjectPool<Range>> rangePoolDict;
    private Dictionary<GameObject, ObjectPool<Buff>> buffPoolDict;

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
        characterPoolDict = new Dictionary<GameObject, ObjectPool<Character>>();

        foreach (var def in definitions)
        {
            var prefab = def.prefab;
            if (prefab == null) continue;

            // Character ������Ʈ�� �ִ� �����ո� Ǯ ����
            var comp = prefab.GetComponent<Character>();
            if (comp != null && !characterPoolDict.ContainsKey(prefab))
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
                characterPoolDict.Add(prefab, pool);
            }
        }

        projectilePoolDict = new Dictionary<GameObject, ObjectPool<Projectile>>();
        rangePoolDict = new Dictionary<GameObject, ObjectPool<Range>>();
        buffPoolDict = new Dictionary<GameObject, ObjectPool<Buff>>();

        if(projectilePoolDict != null)
        {
            foreach (var projPrefab in projectilePrefabs)
            {
                if (projPrefab == null) continue;
                var projectile = projPrefab.GetComponent<Projectile>();
                if (projectile == null) continue;

                projectilePoolDict[projPrefab] =
                  new ObjectPool<Projectile>(projectile, projectileSize, projectileContainer);
            }
        }
        else if (rangePoolDict != null)
        {
            foreach (var rangePrefab in projectilePrefabs)
            {
                if (rangePrefab == null) continue;
                var range = rangePrefab.GetComponent<Range>();
                if (range == null) continue;

                rangePoolDict[rangePrefab] =
                  new ObjectPool<Range>(range, projectileSize, projectileContainer);
            }
        }
        else if(buffPoolDict != null)
        {
            foreach (var buffPrefab in projectilePrefabs)
            {
                if (buffPrefab == null) continue;
                var buff = buffPrefab.GetComponent<Buff>();
                if (buff == null) continue;

                buffPoolDict[buffPrefab] =
                  new ObjectPool<Buff>(buff, projectileSize, projectileContainer);
            }
        }
            
    }

    /// <summary>
    /// Ǯ���� GameObject �ν��Ͻ��� �����ų�, �������� ������ Instantiate �մϴ�.
    /// </summary>
    public GameObject SpawnCharacter(GameObject prefab, Vector3 position)
    {
        if (characterPoolDict.TryGetValue(prefab, out var pool))
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
            if (prefab != null && characterPoolDict.TryGetValue(prefab, out var pool))
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

        if (rangePoolDict.TryGetValue(prefab, out var pool_range))
            return pool_range.Get(pos).gameObject;

        if (rangePoolDict.TryGetValue(prefab, out var pool_buff))
            return pool_buff.Get(pos).gameObject;

        return Instantiate(prefab, pos, Quaternion.identity);
    }

    // Ǯ�� ��ȯ�ϱ�
    public void ReturnProjectile(GameObject _poolObj)
    {
        if (_poolObj == null) return;

        var projectile = _poolObj.GetComponent<Projectile>();
        var range = _poolObj.GetComponent<Range>();
        var buff = _poolObj.GetComponent<Buff>();

        if (projectile != null && projectilePoolDict.TryGetValue(projectile.definitionPrefab, out var pool_projectile))
        {
            _poolObj.SetActive(false);
            _poolObj.transform.SetParent(pool_projectile.parent, worldPositionStays: false);
            return;
        }

        if (range != null && projectilePoolDict.TryGetValue(range.definitionPrefab, out var pool_range))
        {
            _poolObj.SetActive(false);
            _poolObj.transform.SetParent(pool_range.parent, worldPositionStays: false);
            return;
        }

        if (range != null && projectilePoolDict.TryGetValue(range.definitionPrefab, out var pool_buff))
        {
            _poolObj.SetActive(false);
            _poolObj.transform.SetParent(pool_buff.parent, worldPositionStays: false);
            return;
        }

        Destroy(_poolObj);
    }
}
