    // SpawnManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private float playerSpawnTimer = 1f;
    public float totalSpawnTimer = 0;

    [Header("AI Settings")]
    [Tooltip("AI 감지용 레이어 마스크")]
    public LayerMask detectionMask;

    [Header("HP Bar Settings")]
    public GameObject hpBarPrefab;
    public Transform uiCanvas;

    [Header("Popup")]
    public GameObject popupPrefab; // 팝업 프리팹

    [Header("Enemy Spawn Interval Settings")]
    public float enemySpawnMinInterval = 10f;    //스폰 간격 최소(초)
    public float enemySpawnMaxInterval = 15f;   //스폰 간격 최대(초)
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

    // 등급 뽑기
    // 허용된 등급만으로 등급 하나 뽑기
    private Rating PickRating(IEnumerable<Rating> allowed)
    {
        // 실제 총합 계산
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

    // 3) 그 등급 안에서 Definition 하나 뽑기
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

        // 1) 허용할 등급 목록: 모두( Normal, Rare, Unique, Legendary )
        var allowedRatings = ratingWeights.Keys;

        // 2) 2단계 선택
        var pickRating = PickRating(allowedRatings);                           // 등급 가중치 뽑기
        var def = PickDefinitionByRating(player_Definitions, pickRating); // 그 등급 Definition 중 랜덤

        if (def == null)
            return;

        // 3) 비용 차감
        if (!Shared.GameManager.TrySpendCost(def.defaultSpawnCost))
        {
            ShowWarningPopup(0);
            return;
        }

        // 4) 실제 소환
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

        // “Unique와 Legendary만” 허용
        var allowed = new[] { Rating.Unique, Rating.Legendary };

        // 1) 비용 차감 전: 뽑은 등급과 정의
        Rating pickRating = PickRating(allowed);
        var def = PickDefinitionByRating(player_Definitions, pickRating);
        if (def == null) return;

        // 2) 비용 체크
        if (!Shared.GameManager.TrySpendCost(def.specialSpawnCost))
        {
            ShowWarningPopup(0);
            return;
        }

        // 3) 실제 소환
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
            //팝업 띄우기
            ShowGradePopup(def.rating, pos);
        }
    }

    void EnemyRandomSpawn()
    {
        // 1) 랜덤 정의 뽑기
        var allowedRatings = ratingWeights.Keys;

        // 2) 2단계 선택
        var pickRating = PickRating(allowedRatings);                           // 등급 가중치 뽑기
        var def = PickDefinitionByRating(enemy_Definitions, pickRating); // 그 등급 Definition 중 랜덤

        if (def == null)
            return;

        // 2) 난이도 보정된 스탯으로 스폰
        SpawnEnemy(def, enemySpawnPoint.position);

        // 3) 다음 소환 타이머 리셋
        ScheduleNextEnemySpawn();
    }
    private void SpawnEnemy(CharacterDefinition def, Vector3 pos)
    {
        // 공통 풀링
        var go = Shared.PoolManager.SpawnCharacter(def.prefab, pos);
        var ch = go.GetComponent<Character>();

        //기본 등급 보정 스탯
        var stats = def.GetStats();

        //난이도 보정 (적 전용)
        float diffMul = Shared.GameManager.DifficultyStatMultiplier[
                           (int)Shared.GameManager.CurrentDifficulty
                       ];
        stats.maxHp = Mathf.RoundToInt(stats.maxHp * diffMul);
        stats.attackDamage = Mathf.RoundToInt(stats.attackDamage * diffMul * 0.75f);

        //초기화
        ch.Initialize(stats);
        ch.SetDefinition(def);

        // AI 세팅
        var ai = go.GetComponent<AutoAI>();
        ai.detectionLayerMask = def.detectionMask;
        ai.attackStrategy = def.attackStrategy;
        if (def.subAttackStrategy != null)
            ai.subAttackStrategy = def.subAttackStrategy;

        // HP Bar 세팅
        var hp = ch.GetComponent<HPBarController>();
        hp.Initialize(ch, hpBarPrefab, uiCanvas);
    }
    private void ScheduleNextEnemySpawn()
    {
        float baseCharge = player_Definitions[0].defaultSpawnCost / Shared.GameManager.gainPerSec;

        // 난이도별 스폰 간격 계수 가져오기
        float rate = Shared.GameManager.DifficultySpawnRate[(int)Shared.GameManager.CurrentDifficulty];

        // 난이도 반영된 기준 시간
        float chargeTime = baseCharge * rate;

        enemySpawnMinInterval = chargeTime * 0.5f;
        enemySpawnMaxInterval = chargeTime * 1.2f;

        enemySpawnTimer = Random.Range(enemySpawnMinInterval, enemySpawnMaxInterval);
    }


    public void TestSpawn() //테스트용
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
        // 1) 팝업 인스턴스화 (부모는 UI Canvas)
        var popupGrade = Instantiate(popupPrefab, uiCanvas.transform, false);

        // 2) RectTransform 가져오기
        var rectTransform = popupGrade.GetComponent<RectTransform>();

        // 3) Anchor + Pivot → 좌하단 고정
        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0f, 0f);

        // 4) 화면 좌하단에서의 오프셋 (예: x=10, y=10)
        rectTransform.anchoredPosition = new Vector2(50f, 10f);

        // 5) 등급 텍스트 표시
        var gradePopup = popupGrade.GetComponent<GradePopup>();
        gradePopup.ShowRatingText(_rating);
    }

    private void ShowWarningPopup(int _warningcode)
    {
        // 1) 팝업 인스턴스화 (부모는 UI Canvas)
        var popupGrade = Instantiate(popupPrefab, uiCanvas.transform, false);

        // 2) RectTransform 가져오기
        var rectTransform = popupGrade.GetComponent<RectTransform>();

        // 3) Anchor + Pivot → 좌하단 고정
        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0f, 0f);

        // 4) 화면 좌하단에서의 오프셋 (예: x=10, y=10)
        rectTransform.anchoredPosition = new Vector2(50f, 10f);

        // 5) 등급 텍스트 표시
        var warningPopup = popupGrade.GetComponent<GradePopup>();


        warningPopup.ShowWarningText(_warningcode);
    }
}
