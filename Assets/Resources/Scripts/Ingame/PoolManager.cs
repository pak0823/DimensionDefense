// PoolManager.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CharacterDefinition 리스트를 기반으로 자동으로 풀을 생성·관리하는 매니저
/// </summary>
public class PoolManager : MonoBehaviour
{

    [Header("Character Definitions for Pooling")]
    [Tooltip("이 리스트에 등록된 모든 CharacterDefinition.prefab에 대해 풀을 생성합니다.")]
    public List<CharacterDefinition> definitions;

    [Header("Pool Containers")]
    [Tooltip("Player 전용 풀 인스턴스들이 붙을 Transform")]
    public Transform playerPoolContainer;
    [Tooltip("Enemy 전용 풀 인스턴스들이 붙을 Transform")]
    public Transform enemyPoolContainer;

    [Header("Default Pool Settings")]
    [Tooltip("풀 생성 시 기본으로 할당할 초기 크기입니다.")]
    public int defaultPoolSize = 10;

    // prefab → 해당 Character 전용 풀
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

    // CharacterDefinition 리스트를 순회하며 풀 생성
    private void CreatePools()
    {
        //foreach (var def in definitions)
        //{
        //    var prefab = def.prefab;
        //    if (prefab == null) continue;

        //    // Character 컴포넌트가 있는 프리팹만 풀 생성
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
    }

    /// <summary>
    /// 풀에서 GameObject 인스턴스를 꺼내거나, 존재하지 않으면 Instantiate 합니다.
    /// </summary>
    public GameObject Spawn(GameObject prefab, Vector3 position)
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
                // 원래 풀 컨테이너(Parent)에 돌려놓기
                go.transform.SetParent(pool.parent);
                return;
            }
        }
        // 풀에 없으면 기본 PoolManager 아래로
        go.transform.SetParent(this.transform);
    }
}
