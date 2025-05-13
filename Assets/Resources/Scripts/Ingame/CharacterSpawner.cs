// CharacterSpawner.cs
using UnityEngine;

/// <summary>
/// 캐릭터 스패너의 기본 인터페이스와 레지스트리 풀링 헬퍼 제공
/// </summary>
public abstract class CharacterSpawner : MonoBehaviour
{
    /// <summary>
    /// 레지스트리 풀에서 프리팹 인스턴스를 꺼내어 Character 컴포넌트를 반환
    /// </summary>
    protected Character SpawnFromPool(GameObject prefab, Vector3 position)
    {
        var go = Shared.PoolManager.SpawnCharacter(prefab, position);
        return go.GetComponent<Character>();
    }

    /// <summary>
    /// 주어진 위치에 캐릭터를 생성하고 초기화해서 반환
    /// </summary>
    public abstract Character Spawn(Vector3 position);
}
