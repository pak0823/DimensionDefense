    // SpawnManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private float playerSpawnTimer = 1f;
    public float totalSpawnTimer = 0;

    [Header("AI Settings")]
    [Tooltip("AI ������ ���̾� ����ũ")]
    public LayerMask detectionMask;

    [Header("HP Bar Settings")]
    [Tooltip("HP Bar Prefab (Canvas ������ �߰�)")]
    public GameObject hpBarPrefab;
    [Tooltip("HP Bar�� �߰��� Canvas Transform")]
    public Transform uiCanvas;

    [Header("Enemy Spawn Interval Settings")]
    public float enemySpawnMinInterval = 10f;    //���� ���� �ּ�(��)
    public float enemySpawnMaxInterval = 18f;   //���� ���� �ִ�(��)
    private float enemySpawnTimer = 8f;

    private void Awake()
    {
        Shared.SpawnManager = this;
    }

    private void Update()
    {
        enemySpawnTimer -= Time.deltaTime;

        if (totalSpawnTimer <= 0)
            totalSpawnTimer = 0;
        else
            totalSpawnTimer -= Time.deltaTime;

        if (enemySpawnTimer <= 0f)
        {
            ScheduleNextEnemySpawn();
            EnemyRandomSpawn();
        }
            
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


    public void PlayerDefaultSpawn()
    {
        if(totalSpawnTimer <= 0)
        {
            // 1) ���� ���� �̱� (��� ���)
            var def = GetRandomByWeight(player_Definitions);

            // 2) ��� ����
            if (!Shared.GameManager.TrySpendCost(def.defaultSpawnCost))
                return;

            // 3) ���� ��ȯ
            DoSpawn(def, playerSpawnPoint.position);

            totalSpawnTimer = playerSpawnTimer;
        }
        else
        {
            Debug.Log("���� ��Ÿ�� �Դϴ�.");
        }
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

    void EnemyRandomSpawn()
    {
        // 1) ���� ���� �̱� (��� ���)
        var def = GetRandomByWeight(enemy_Definitions);
        DoSpawn(def, enemySpawnPoint.position);
    }
    private void ScheduleNextEnemySpawn()
    {
        // 1) �÷��̾� �ڽ�Ʈ ���� �ð� ����  
        float chargeTime = player_Definitions[0].defaultSpawnCost / Shared.GameManager.gainPerSec; // ex. 10��

        // 2) �� �ð� ���ķ� ���� ���� ���� (��: 0.5~1.5�� ����)
        enemySpawnMinInterval = chargeTime * 0.5f;   // ex. 5��
        enemySpawnMaxInterval = chargeTime * 1.5f;   // ex. 15��

        // 3) ���� ���� Ÿ�̸ӿ� ���� �Ҵ�
        enemySpawnTimer = Random.Range(enemySpawnMinInterval, enemySpawnMaxInterval);
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
}
