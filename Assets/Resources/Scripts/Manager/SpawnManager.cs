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
    public GameObject hpBarPrefab;
    public Transform uiCanvas;

    [Header("Popup")]
    public GameObject popupPrefab; // �˾� ������

    [Header("Enemy Spawn Interval Settings")]
    public float enemySpawnMinInterval = 10f;    //���� ���� �ּ�(��)
    public float enemySpawnMaxInterval = 15f;   //���� ���� �ִ�(��)
    private float enemySpawnTimer = 5f;

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
            EnemyRandomSpawn();
    }

    CharacterDefinition GetRandomByWeight(List<CharacterDefinition> list)
    {
        const float total = 100f;                              // �� ���� 100%
        float r = Random.Range(0f, total);                     // �� 0~100 ����
        float cum = 0f;

        foreach (var def in list)
        {
            cum += def.spawnWeight;                            // spawnWeight�� �ۼ�Ʈ�� ���� (��=100)
            if (r <= cum)
                return def;                                    // ������ ������ ���� ��ȯ
        }

        // Ȥ�� ������ 100�� �� ä��� ������
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
            {
                ShowWarningPopup(0);
                return;
            }
                
            // 3) ���� ��ȯ
            SpawnPlayer(def, playerSpawnPoint.position);

            totalSpawnTimer = playerSpawnTimer;
        }
        else
        {
            ShowWarningPopup(1);
        }
    }
    public void PlayerSpecialSpawn()
    {
        if (totalSpawnTimer <= 0)
        {
            // 1) ���� �̻� ����
            var special = player_Definitions
                .Where(d => d.rating != Rating.Normal && d.rating != Rating.Rare)
                .ToList();
            if (special.Count == 0) return;

            // 2) ���� ���� �̱�
            var def = GetRandomByWeight(special);

            // 3) ��� ����
            if (!Shared.GameManager.TrySpendCost(def.defaultSpawnCost))
            {
                ShowWarningPopup(0);
                return;
            }

            // 4) ���� ��ȯ
            SpawnPlayer(def, playerSpawnPoint.position);
        }
        else
        {
            ShowWarningPopup(1);
        }
    }
    private void SpawnPlayer(CharacterDefinition def, Vector3 pos)
    {
        var go = Shared.PoolManager.SpawnCharacter(def.prefab, pos);
        var ch = go.GetComponent<Character>();
        ch.Initialize(def.GetStats());
        ch.Initialize(def);

        var ai = go.GetComponent<AutoAI>();
        ai.detectionLayerMask = def.detectionMask;
        ai.attackStrategy = def.attackStrategy;

        if (def.subAttackStrategy != null)
            ai.subAttackStrategy = def.subAttackStrategy;

        var hp = ch.GetComponent<HPBarController>();
        hp.Initialize(ch, hpBarPrefab, uiCanvas);

        if (!def.isEnemy)
        {
            //�˾� ����
            ShowGradePopup(def.rating, pos);
        }
    }

    void EnemyRandomSpawn()
    {
        // 1) ���� ���� �̱�
        var def = GetRandomByWeight(enemy_Definitions);

        // 2) ���̵� ������ �������� ����
        SpawnEnemy(def, enemySpawnPoint.position);

        // 3) ���� ��ȯ Ÿ�̸� ����
        ScheduleNextEnemySpawn();
    }
    private void SpawnEnemy(CharacterDefinition def, Vector3 pos)
    {
        // ���� Ǯ��
        var go = Shared.PoolManager.SpawnCharacter(def.prefab, pos);
        var ch = go.GetComponent<Character>();

        //�⺻ ��� ���� ����
        var stats = def.GetStats();

        //���̵� ���� (�� ����)
        float diffMul = Shared.GameManager.DifficultyStatMultiplier[
                           (int)Shared.GameManager.CurrentDifficulty
                       ];
        stats.maxHp = Mathf.RoundToInt(stats.maxHp * diffMul);
        stats.attackDamage = Mathf.RoundToInt(stats.attackDamage * diffMul * 0.75f);

        //�ʱ�ȭ
        ch.Initialize(stats);
        ch.SetDefinition(def);

        // AI ����
        var ai = go.GetComponent<AutoAI>();
        ai.detectionLayerMask = def.detectionMask;
        ai.attackStrategy = def.attackStrategy;
        if (def.subAttackStrategy != null)
            ai.subAttackStrategy = def.subAttackStrategy;

        // HP Bar ����
        var hp = ch.GetComponent<HPBarController>();
        hp.Initialize(ch, hpBarPrefab, uiCanvas);
    }
    private void ScheduleNextEnemySpawn()
    {
        float baseCharge = player_Definitions[0].defaultSpawnCost / Shared.GameManager.gainPerSec;

        // ���̵��� ���� ���� ��� ��������
        float rate = Shared.GameManager.DifficultySpawnRate[(int)Shared.GameManager.CurrentDifficulty];

        // ���̵� �ݿ��� ���� �ð�
        float chargeTime = baseCharge * rate;

        enemySpawnMinInterval = chargeTime * 0.5f;
        enemySpawnMaxInterval = chargeTime * 1.5f;

        enemySpawnTimer = Random.Range(enemySpawnMinInterval, enemySpawnMaxInterval);
    }

    

    public void TestSpawn() //�׽�Ʈ��
    {
        // 1) ���� �̻� ����
        var special = player_Definitions
            .Where(d => d.rating == Rating.Legendary)
            .ToList();
        if (special.Count == 0) return;

        // 2) ���� ���� �̱�
        var def = GetRandomByWeight(special);

        // 4) ���� ��ȯ
        SpawnPlayer(def, playerSpawnPoint.position);
    }

    private void ShowGradePopup(Rating _rating, Vector3 _worldPos)
    {
        // 1) �˾� �ν��Ͻ�ȭ (�θ�� UI Canvas)
        var popupGrade = Instantiate(popupPrefab, uiCanvas.transform, false);

        // 2) RectTransform ��������
        var rectTransform = popupGrade.GetComponent<RectTransform>();

        // 3) Anchor + Pivot �� ���ϴ� ����
        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0f, 0f);

        // 4) ȭ�� ���ϴܿ����� ������ (��: x=10, y=10)
        rectTransform.anchoredPosition = new Vector2(50f, 10f);

        // 5) ��� �ؽ�Ʈ ǥ��
        var gradePopup = popupGrade.GetComponent<GradePopup>();
        gradePopup.ShowRatingText(_rating);
    }

    private void ShowWarningPopup(int _warningcode)
    {
        // 1) �˾� �ν��Ͻ�ȭ (�θ�� UI Canvas)
        var popupGrade = Instantiate(popupPrefab, uiCanvas.transform, false);

        // 2) RectTransform ��������
        var rectTransform = popupGrade.GetComponent<RectTransform>();

        // 3) Anchor + Pivot �� ���ϴ� ����
        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0f, 0f);

        // 4) ȭ�� ���ϴܿ����� ������ (��: x=10, y=10)
        rectTransform.anchoredPosition = new Vector2(50f, 10f);

        // 5) ��� �ؽ�Ʈ ǥ��
        var warningPopup = popupGrade.GetComponent<GradePopup>();


        warningPopup.ShowWarningText(_warningcode);
    }
}
