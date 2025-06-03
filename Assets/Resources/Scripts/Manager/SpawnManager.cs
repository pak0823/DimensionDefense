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


    private readonly Dictionary<Rating, float> ratingWeights = new Dictionary<Rating, float>
{
    { Rating.Normal,    65f },   // 65%
    { Rating.Rare,      25f },   // 25%
    { Rating.Unique,      9f },   // 9%
    { Rating.Legendary,  1f }    //  1%
};

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

    // ��� �̱�
    // ���� ��޸����� ��� �ϳ� �̱�
    private Rating PickRating(IEnumerable<Rating> allowed)
    {
        // ���� ���� ���
        var filtered = ratingWeights
            .Where(kv => allowed.Contains(kv.Key))
            .ToList();
        float total = filtered.Sum(kv => kv.Value);

        float r = Random.Range(0f, total);
        float cum = 0f;
        foreach (var kv in filtered)
        {
            cum += kv.Value;
            if (r <= cum) return kv.Key;
        }
        return filtered.Last().Key;
    }

    // 3) �� ��� �ȿ��� Definition �ϳ� �̱�
    private CharacterDefinition PickDefinitionByRating(
        List<CharacterDefinition> list, Rating rating)
    {
        var defs = list.Where(d => d.rating == rating).ToList();
        if (defs.Count == 0) return null;
        return defs[Random.Range(0, defs.Count)];
    }


    public void PlayerDefaultSpawn()
    {
        if (totalSpawnTimer > 0)
        {
            ShowWarningPopup(1);
            return;
        }

        // 1) ����� ��� ���: ���( Normal, Rare, Unique, Legendary )
        var allowedRatings = ratingWeights.Keys;

        // 2) 2�ܰ� ����
        var pickRating = PickRating(allowedRatings);                           // ��� ����ġ �̱�
        var def = PickDefinitionByRating(player_Definitions, pickRating); // �� ��� Definition �� ����

        if (def == null)
            return;

        // 3) ��� ����
        if (!Shared.GameManager.TrySpendCost(def.defaultSpawnCost))
        {
            ShowWarningPopup(0);
            return;
        }

        // 4) ���� ��ȯ
        SpawnPlayer(def, playerSpawnPoint.position);

        totalSpawnTimer = playerSpawnTimer;
    }
    public void PlayerSpecialSpawn()
    {
        if (totalSpawnTimer > 0)
        {
            ShowWarningPopup(1);
            return;
        }

        // ��Unique�� Legendary���� ���
        var allowed = new[] { Rating.Unique, Rating.Legendary };

        // 1) ��� ���� ��: ���� ��ް� ����
        Rating pickRating = PickRating(allowed);
        var def = PickDefinitionByRating(player_Definitions, pickRating);
        if (def == null) return;

        // 2) ��� üũ
        if (!Shared.GameManager.TrySpendCost(def.specialSpawnCost))
        {
            ShowWarningPopup(0);
            return;
        }

        // 3) ���� ��ȯ
        SpawnPlayer(def, playerSpawnPoint.position);
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
        var allowedRatings = ratingWeights.Keys;

        // 2) 2�ܰ� ����
        var pickRating = PickRating(allowedRatings);                           // ��� ����ġ �̱�
        var def = PickDefinitionByRating(enemy_Definitions, pickRating); // �� ��� Definition �� ����

        if (def == null)
            return;

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
        enemySpawnMaxInterval = chargeTime * 1.2f;

        enemySpawnTimer = Random.Range(enemySpawnMinInterval, enemySpawnMaxInterval);
    }


    public void TestSpawn() //�׽�Ʈ��
    {
        var allowed = new[] {Rating.Legendary};
        Rating pickRating = PickRating(allowed);
        //var def = PickDefinitionByRating(player_Definitions, pickRating);
        var def = PickDefinitionByRating(enemy_Definitions, pickRating);
        if (def == null) return;

        SpawnPlayer(def, enemySpawnPoint.position);
        //SpawnPlayer(def, playerSpawnPoint.position);
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
