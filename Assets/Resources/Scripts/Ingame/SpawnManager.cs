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
    [Tooltip("스폰 위치 Transform (플레이어)")]
    public Transform playerSpawnPoint;
    [Tooltip("스폰 위치 Transform (적)")]
    public Transform enemySpawnPoint;

    [Header("AI Settings")]
    [Tooltip("AI 감지용 레이어 마스크")]
    public LayerMask detectionMask;

    [Header("HP Bar Settings")]
    [Tooltip("HP Bar Prefab (Canvas 하위에 추가)")]
    public GameObject hpBarPrefab;
    [Tooltip("HP Bar를 추가할 Canvas Transform")]
    public Transform uiCanvas;

    private void Start()
    {
        spawnButton.onClick.AddListener(SpawnRandomCharacter);
    }

    /// <summary>
    /// 랜덤한 CharacterDefinition을 선택해 풀에서 인스턴스를 꺼내고 초기화
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
            ai.detectionLayerMask = def.detectionMask;
            ai.attackStrategy = def.attackStrategy;
        }

        // 5) HP Bar 인스턴스화 및 초기화
        if (hpBarPrefab != null && uiCanvas != null && character != null)
        {
            var hpCtrl = character.GetComponent<HPBarController>();
            if (hpCtrl == null)
            {
                Debug.LogError("SpawnManager: 캐릭터 프리팹에 HPBarController 컴포넌트를 붙여주세요.");
            }
            else
            {
                // 올바른 Initialize 호출: character, hpBarPrefab, uiCanvas
                hpCtrl.Initialize(character, hpBarPrefab, uiCanvas);
            }
        }
    }
}
