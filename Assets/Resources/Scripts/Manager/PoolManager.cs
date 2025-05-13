// PoolManager.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CharacterDefinition 리스트를 기반으로 자동으로 풀을 생성·관리하는 매니저
/// </summary>
public class PoolManager : MonoBehaviour
{

    [Header("Character Definitions for Pooling")]
    public List<CharacterDefinition> definitions;   // 풀링할 캐릭터 프리팹 목록

    [Header("Projectile Pooling")]
    public List<GameObject> projectilePrefabs;      // 풀링할 투사체 프리팹 목록
    

    [Header("Pool Containers")]
    public Transform playerPoolContainer;     // Player 풀 인스턴스들이 붙을 컨테이너
    public Transform enemyPoolContainer;      // Enemy 풀 인스턴스들이 붙을 컨테이너
    public Transform projectileContainer;     // Projectile 풀 인스턴스들이 붙을 컨테이너

    [Header("Default Pool Settings")]
    public int defaultPoolSize = 10;    //풀 생성 시 기본으로 할당할 초기 크기

    // prefab → [해당 Objedt] 전용 풀
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

    // CharacterDefinition 리스트를 순회하며 풀 생성
    private void CreatePools()
    {
        poolDict = new Dictionary<GameObject, ObjectPool<Character>>();

        foreach (var def in definitions)
        {
            var prefab = def.prefab;
            if (prefab == null) continue;

            // Character 컴포넌트가 있는 프리팹만 풀 생성
            var comp = prefab.GetComponent<Character>();
            if (comp != null && !poolDict.ContainsKey(prefab))
            {
                // 풀링 인스턴스이 배치될 부모 컨테이너 결정
                Transform parent = def.isEnemy ? enemyPoolContainer : playerPoolContainer;
                if (parent == null)
                {
                    // 컨테이너가 미할당된 경우 기본 PoolManager 오브젝트로
                    parent = this.transform;
                }

                // 해당 컨테이너를 부모로 풀 생성
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
    /// 풀에서 GameObject 인스턴스를 꺼내거나, 존재하지 않으면 Instantiate 합니다.
    /// </summary>
    public GameObject SpawnCharacter(GameObject prefab, Vector3 position)
    {
        if (poolDict.TryGetValue(prefab, out var pool))
        {
            var character = pool.Get(position);
            return character.gameObject;
        }
        // 풀에 없으면 일반 Instantiate
        var go = Instantiate(prefab, position, Quaternion.identity);
        return go;
    }

    /// <summary>
    /// 오브젝트를 풀로 반환(Deactivate 후 해당 컨테이너 하위에 보관)
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
                // 원래 풀 컨테이너(Parent)에 돌려놓기
                _poolObj.transform.SetParent(pool.parent);
                return;
            }
        }
        // 풀에 없으면 기본 PoolManager 아래로
        _poolObj.transform.SetParent(this.transform);
    }

    // 풀에서 꺼내기
    public GameObject SpawnProjectile(GameObject prefab, Vector3 pos)
    {
        //if (projectilePoolDict.TryGetValue(prefab, out var pool))
        //    return pool.Get(pos).gameObject;
        //return Instantiate(prefab, pos, Quaternion.identity);

        if (LightningPoolDict.TryGetValue(prefab, out var pool))
            return pool.Get(pos).gameObject;
        return Instantiate(prefab, pos, Quaternion.identity);
    }

    // 풀로 반환하기
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
