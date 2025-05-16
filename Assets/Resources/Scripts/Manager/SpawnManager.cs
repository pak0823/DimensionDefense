// SpawnManager.cs
using System.Collections.Generic;
using System.Linq;
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

    private void Awake()
    {
        Shared.SpawnManager = this;
    }

    CharacterDefinition GetRandomByWeight(List<CharacterDefinition> list)
    {
        // 1) ���� ����ġ ���
        var weights = list.Select(def => def.spawnWeight).ToArray();
        float total = weights.Sum();
        float r = Random.value * total;
        float cum = 0f;
        for (int i = 0; i < list.Count; i++)
        {
            cum += weights[i];
            if (r <= cum)
                return list[i];
        }
        return list[list.Count - 1];
    }



    /// <summary>
    /// ������ CharacterDefinition�� ������ Ǯ���� �ν��Ͻ��� ������ �ʱ�ȭ
    /// </summary>
    /// 
    public void PlayerDefaultSpawn()
    {
        // 1) ���� ���� �̱� (��� ���)
        var def = GetRandomByWeight(player_Definitions);

        // 2) ��� ����
        if (!Shared.GameManager.TrySpendCost(def.defaultSpawnCost))
            return;

        // 3) ���� ��ȯ
        DoSpawn(def, playerSpawnPoint.position);
    }
    public void PlayerSpecialSpawn()
    {
        // 1) ���� �̻� ����
        var special = player_Definitions
            .Where(d => d.rating != Rating.Normal)
            .ToList();
        if (special.Count == 0) return;

        // 2) ���� ���� �̱�
        var def = GetRandomByWeight(special);

        // 3) ��� ����
        if (!Shared.GameManager.TrySpendCost(def.specialSpawnCost))
                return;

        // 4) ���� ��ȯ
        DoSpawn(def, playerSpawnPoint.position);
    }

    private void DoSpawn(CharacterDefinition def, Vector3 pos)
    {
        var go = Shared.PoolManager.SpawnCharacter(def.prefab, pos);
        var ch = go.GetComponent<Character>();
        ch.Initialize(def.GetStats());
        ch.Initialize(def);

        var ai = go.GetComponent<AutoAI>();
        ai.detectionLayerMask = def.detectionMask;
        ai.attackStrategy = def.attackStrategy;

        var hp = ch.GetComponent<HPBarController>();
        hp.Initialize(ch, hpBarPrefab, uiCanvas);
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
