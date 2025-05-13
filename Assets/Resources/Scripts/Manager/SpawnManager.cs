// SpawnManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

/// <summary>
/// CharacterDefinition 리스트에서 랜덤으로 정의를 골라
/// PoolManager를 통해 인스턴스를 꺼내고 초기화합니다.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("Player Definitions")]
    [Tooltip("풀링 및 생성에 사용할 정의 리스트")]
    public List<CharacterDefinition> player_Definitions;

    [Header("Enemy Definitions")]
    public List<CharacterDefinition> enemy_Definitions;


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
        //player_SpawnButton.onClick.AddListener(PlayerRandomSpawn);
        //enemy_SpawnButton.onClick.AddListener(EnemyRandomSpawn);
        Shared.SpawnManager = this;
    }

    /// <summary>
    /// 랜덤한 CharacterDefinition을 선택해 풀에서 인스턴스를 꺼내고 초기화
    /// </summary>
    public void PlayerRandomSpawn()
    {
        if (player_Definitions == null || player_Definitions.Count == 0)
        {
            Debug.LogWarning("SpawnManager: player_Definitions 리스트가 비어 있습니다.");
            return;
        }

        // 1) 랜덤 정의 선택
        var player_Def = player_Definitions[Random.Range(0, player_Definitions.Count)];
        Transform playerPoint = playerSpawnPoint;

        // 2) 풀에서 꺼내기
        GameObject player_Pool = Shared.PoolManager.SpawnCharacter(player_Def.prefab, playerPoint.position);

        if (player_Pool == null)
        {
            Debug.LogError($"SpawnManager: PoolManager에서 {player_Def.prefab.name}을(를) 찾을 수 없습니다.");
            return;
        }

        // 3) 스탯 초기화
        var character_player = player_Pool.GetComponent<Character>();
        if (character_player != null)
        {
            character_player.Initialize(player_Def.GetStats());
            character_player.Initialize(player_Def);
        }
            


        // 4) AI 설정
        var ai_Player = player_Pool.GetComponent<AutoAI>();
        if (ai_Player != null)
        {
            ai_Player.detectionLayerMask = player_Def.detectionMask;
            ai_Player.attackStrategy = player_Def.attackStrategy;
        }

        // 5) HP Bar 인스턴스화 및 초기화
        if (hpBarPrefab != null && uiCanvas != null && character_player != null)
        {
            var hpCtrl_player = character_player.GetComponent<HPBarController>();

            if (hpCtrl_player == null)
                Debug.LogError("SpawnManager: Player 프리팹에 HPBarController 컴포넌트를 붙여주세요.");
            else
            {
                // 올바른 Initialize 호출: character, hpBarPrefab, uiCanvas
                hpCtrl_player.Initialize(character_player, hpBarPrefab, uiCanvas);
            }
        }
    }

    public void EnemyRandomSpawn()
    {
        if (enemy_Definitions == null || enemy_Definitions.Count == 0)
        {
            Debug.LogWarning("SpawnManager: enemy_Definitions 리스트가 비어 있습니다.");
            return;
        }

        // 1) 랜덤 정의 선택
        var enemy_Def = enemy_Definitions[Random.Range(0, enemy_Definitions.Count)];
        Transform enemyPoint = enemySpawnPoint;

        // 2) 풀에서 꺼내기
        GameObject enemy_Pool = Shared.PoolManager.SpawnCharacter(enemy_Def.prefab, enemyPoint.position);

        if (enemy_Pool == null)
        {
            Debug.LogError($"SpawnManager: PoolManager에서 {enemy_Def.prefab.name}을(를) 찾을 수 없습니다.");
            return;
        }

        // 3) 스탯 초기화
        var character_Enemy = enemy_Pool.GetComponent<Character>();
        if (character_Enemy != null)
        {
            character_Enemy.Initialize(enemy_Def.GetStats());
            character_Enemy.Initialize(enemy_Def);
        }
            

        // 4) AI 설정
        var ai_Enemy = enemy_Pool.GetComponent<AutoAI>();
        if (ai_Enemy != null)
        {
            ai_Enemy.detectionLayerMask = enemy_Def.detectionMask;
            ai_Enemy.attackStrategy = enemy_Def.attackStrategy;
        }

        // 5) HP Bar 인스턴스화 및 초기화
        if (hpBarPrefab != null && uiCanvas != null && character_Enemy != null)
        {
            var hpCtrl_Enemy = character_Enemy.GetComponent<HPBarController>();

            if (hpCtrl_Enemy == null)
            {
                Debug.LogError("SpawnManager: Enemy 프리팹에 HPBarController 컴포넌트를 붙여주세요.");
            }
            else
            {
                // 올바른 Initialize 호출: character, hpBarPrefab, uiCanvas
                hpCtrl_Enemy.Initialize(character_Enemy, hpBarPrefab, uiCanvas);
            }
        }
    }
}
