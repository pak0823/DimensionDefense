// SpawnManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

/// <summary>
/// CharacterDefinition ����Ʈ���� �������� ���Ǹ� ���
/// PoolManager�� ���� �ν��Ͻ��� ������ �ʱ�ȭ�մϴ�.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("Player Definitions")]
    [Tooltip("Ǯ�� �� ������ ����� ���� ����Ʈ")]
    public List<CharacterDefinition> player_Definitions;

    [Header("Enemy Definitions")]
    public List<CharacterDefinition> enemy_Definitions;


    [Header("Spawn Settings")]
    [Tooltip("���� ��ġ Transform (�÷��̾�)")]
    public Transform playerSpawnPoint;
    [Tooltip("���� ��ġ Transform (��)")]
    public Transform enemySpawnPoint;

    [Header("AI Settings")]
    [Tooltip("AI ������ ���̾� ����ũ")]
    public LayerMask detectionMask;

    [Header("HP Bar Settings")]
    [Tooltip("HP Bar Prefab (Canvas ������ �߰�)")]
    public GameObject hpBarPrefab;
    [Tooltip("HP Bar�� �߰��� Canvas Transform")]
    public Transform uiCanvas;

    private void Start()
    {
        //player_SpawnButton.onClick.AddListener(PlayerRandomSpawn);
        //enemy_SpawnButton.onClick.AddListener(EnemyRandomSpawn);
        Shared.SpawnManager = this;
    }

    /// <summary>
    /// ������ CharacterDefinition�� ������ Ǯ���� �ν��Ͻ��� ������ �ʱ�ȭ
    /// </summary>
    public void PlayerRandomSpawn()
    {
        if (player_Definitions == null || player_Definitions.Count == 0)
        {
            Debug.LogWarning("SpawnManager: player_Definitions ����Ʈ�� ��� �ֽ��ϴ�.");
            return;
        }

        // 1) ���� ���� ����
        var player_Def = player_Definitions[Random.Range(0, player_Definitions.Count)];
        Transform playerPoint = playerSpawnPoint;

        // 2) Ǯ���� ������
        GameObject player_Pool = Shared.PoolManager.SpawnCharacter(player_Def.prefab, playerPoint.position);

        if (player_Pool == null)
        {
            Debug.LogError($"SpawnManager: PoolManager���� {player_Def.prefab.name}��(��) ã�� �� �����ϴ�.");
            return;
        }

        // 3) ���� �ʱ�ȭ
        var character_player = player_Pool.GetComponent<Character>();
        if (character_player != null)
        {
            character_player.Initialize(player_Def.GetStats());
            character_player.Initialize(player_Def);
        }
            


        // 4) AI ����
        var ai_Player = player_Pool.GetComponent<AutoAI>();
        if (ai_Player != null)
        {
            ai_Player.detectionLayerMask = player_Def.detectionMask;
            ai_Player.attackStrategy = player_Def.attackStrategy;
        }

        // 5) HP Bar �ν��Ͻ�ȭ �� �ʱ�ȭ
        if (hpBarPrefab != null && uiCanvas != null && character_player != null)
        {
            var hpCtrl_player = character_player.GetComponent<HPBarController>();

            if (hpCtrl_player == null)
                Debug.LogError("SpawnManager: Player �����տ� HPBarController ������Ʈ�� �ٿ��ּ���.");
            else
            {
                // �ùٸ� Initialize ȣ��: character, hpBarPrefab, uiCanvas
                hpCtrl_player.Initialize(character_player, hpBarPrefab, uiCanvas);
            }
        }
    }

    public void EnemyRandomSpawn()
    {
        if (enemy_Definitions == null || enemy_Definitions.Count == 0)
        {
            Debug.LogWarning("SpawnManager: enemy_Definitions ����Ʈ�� ��� �ֽ��ϴ�.");
            return;
        }

        // 1) ���� ���� ����
        var enemy_Def = enemy_Definitions[Random.Range(0, enemy_Definitions.Count)];
        Transform enemyPoint = enemySpawnPoint;

        // 2) Ǯ���� ������
        GameObject enemy_Pool = Shared.PoolManager.SpawnCharacter(enemy_Def.prefab, enemyPoint.position);

        if (enemy_Pool == null)
        {
            Debug.LogError($"SpawnManager: PoolManager���� {enemy_Def.prefab.name}��(��) ã�� �� �����ϴ�.");
            return;
        }

        // 3) ���� �ʱ�ȭ
        var character_Enemy = enemy_Pool.GetComponent<Character>();
        if (character_Enemy != null)
        {
            character_Enemy.Initialize(enemy_Def.GetStats());
            character_Enemy.Initialize(enemy_Def);
        }
            

        // 4) AI ����
        var ai_Enemy = enemy_Pool.GetComponent<AutoAI>();
        if (ai_Enemy != null)
        {
            ai_Enemy.detectionLayerMask = enemy_Def.detectionMask;
            ai_Enemy.attackStrategy = enemy_Def.attackStrategy;
        }

        // 5) HP Bar �ν��Ͻ�ȭ �� �ʱ�ȭ
        if (hpBarPrefab != null && uiCanvas != null && character_Enemy != null)
        {
            var hpCtrl_Enemy = character_Enemy.GetComponent<HPBarController>();

            if (hpCtrl_Enemy == null)
            {
                Debug.LogError("SpawnManager: Enemy �����տ� HPBarController ������Ʈ�� �ٿ��ּ���.");
            }
            else
            {
                // �ùٸ� Initialize ȣ��: character, hpBarPrefab, uiCanvas
                hpCtrl_Enemy.Initialize(character_Enemy, hpBarPrefab, uiCanvas);
            }
        }
    }
}
