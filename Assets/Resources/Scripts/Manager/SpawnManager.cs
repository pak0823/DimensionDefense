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
        const float total = 100f;                              // ① 총합 100%
        float r = Random.Range(0f, total);                     // ② 0~100 랜덤
        float cum = 0f;

        foreach (var def in list)
        {
            cum += def.spawnWeight;                            // spawnWeight를 퍼센트로 설정 (합=100)
            if (r <= cum)
                return def;                                    // 누적값 구간에 따라 반환
        }

        // 혹시 누적이 100을 못 채우면 마지막
        return list[list.Count - 1];
    }


    public void PlayerDefaultSpawn()
    {
        if(totalSpawnTimer <= 0)
        {
            // 1) 랜덤 정의 뽑기 (모든 등급)
            var def = GetRandomByWeight(player_Definitions);

            // 2) 비용 차감
            if (!Shared.GameManager.TrySpendCost(def.defaultSpawnCost))
            {
                ShowWarningPopup(0);
                return;
            }
                
            // 3) 실제 소환
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
            // 1) 레어 이상만 필터
            var special = player_Definitions
                .Where(d => d.rating != Rating.Normal && d.rating != Rating.Rare)
                .ToList();
            if (special.Count == 0) return;

            // 2) 랜덤 정의 뽑기
            var def = GetRandomByWeight(special);

            // 3) 비용 차감
            if (!Shared.GameManager.TrySpendCost(def.defaultSpawnCost))
            {
                ShowWarningPopup(0);
                return;
            }

            // 4) 실제 소환
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
            //팝업 띄우기
            ShowGradePopup(def.rating, pos);
        }
    }

    void EnemyRandomSpawn()
    {
        // 1) 랜덤 정의 뽑기
        var def = GetRandomByWeight(enemy_Definitions);

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
        enemySpawnMaxInterval = chargeTime * 1.5f;

        enemySpawnTimer = Random.Range(enemySpawnMinInterval, enemySpawnMaxInterval);
    }

    

    public void TestSpawn() //테스트용
    {
        // 1) 레어 이상만 필터
        var special = player_Definitions
            .Where(d => d.rating == Rating.Legendary)
            .ToList();
        if (special.Count == 0) return;

        // 2) 랜덤 정의 뽑기
        var def = GetRandomByWeight(special);

        // 4) 실제 소환
        SpawnPlayer(def, playerSpawnPoint.position);
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
