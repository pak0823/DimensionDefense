// SpawnManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CharacterDefinition 리스트에서 랜덤으로 정의를 골라
/// PoolManager를 통해 인스턴스를 꺼내고 초기화합니다.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("생성 트리거용 버튼")]
    public Button spawnButton;

    [Header("Character Definitions")]
    [Tooltip("풀링 및 생성에 사용할 정의 리스트")]
    public List<CharacterDefinition> definitions;

    [Header("Spawn Settings")]
    [Tooltip("스폰 위치 Transform")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    [Tooltip("AI 감지용 레이어 마스크")]
    public LayerMask detectionMask;

    private void Start()
    {
        spawnButton.onClick.AddListener(SpawnRandomCharacter);
    }

    /// <summary>
    /// 랜덤한 CharacterDefinition을 선택해 пул에서 인스턴스를 꺼내고 초기화
    /// </summary>
    private void SpawnRandomCharacter()
    {
        if (definitions == null || definitions.Count == 0)
        {
            Debug.LogWarning("SpawnManager: definitions 리스트가 비어 있습니다.");
            return;
        }

        // 1) 랜덤 정의 선택
        var def = definitions[Random.Range(0, definitions.Count)];
        Transform usePoint = (def.isEnemy ? enemySpawnPoint : playerSpawnPoint);

        // 2) 풀에서 꺼내기 (or Instantiate fallback)
        GameObject go = Shared.PoolManager.Spawn(def.prefab, usePoint.position);
        if (go == null)
        {
            Debug.LogError($"SpawnManager: PoolManager에서 {def.prefab.name}을(를) 찾을 수 없습니다.");
            return;
        }

        // 3) 스탯 초기화
        var character = go.GetComponent<Character>();
        if (character != null)
            character.Initialize(def.GetStats());

        // 4) AI 설정
        var ai = go.GetComponent<AutoAI>();
        if (ai != null)
        {
            ai.detectionLayerMask = def.detectionMask;   // definition마다 다른 마스크
            ai.attackStrategy = def.attackStrategy;
        }
    }
}
