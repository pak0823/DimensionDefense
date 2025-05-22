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

    [Header("Grade Popup")]
    public GameObject gradePopupPrefab; // 위에서 만든 프리팹

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
        {
            ScheduleNextEnemySpawn();
            EnemyRandomSpawn();
        }
            
    }

    CharacterDefinition GetRandomByWeight(List<CharacterDefinition> list)
    {
        // 1) 누적 가중치 계산
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
            // 1) 랜덤 정의 뽑기 (모든 등급)
            var def = GetRandomByWeight(player_Definitions);

            // 2) 비용 차감
            if (!Shared.GameManager.TrySpendCost(def.defaultSpawnCost))
                return;

            // 3) 실제 소환
            DoSpawn(def, playerSpawnPoint.position);

            totalSpawnTimer = playerSpawnTimer;
        }
        else
        {
            Debug.Log("아직 쿨타임 입니다.");
        }
    }
    public void PlayerSpecialSpawn()
    {
        // 1) 레어 이상만 필터
        var special = player_Definitions
            .Where(d => d.rating != Rating.Normal && d.rating != Rating.Rare)
            .ToList();
        if (special.Count == 0) return;

        // 2) 랜덤 정의 뽑기
        var def = GetRandomByWeight(special);

        // 3) 비용 차감
        if (!Shared.GameManager.TrySpendCost(def.specialSpawnCost))
                return;

        // 4) 실제 소환
        DoSpawn(def, playerSpawnPoint.position);
    }

    void EnemyRandomSpawn()
    {
        // 1) 랜덤 정의 뽑기 (모든 등급)
        var def = GetRandomByWeight(enemy_Definitions);
        DoSpawn(def, enemySpawnPoint.position);
    }
    private void ScheduleNextEnemySpawn()
    {
        // 1) 플레이어 코스트 충전 시간 기준  
        float chargeTime = player_Definitions[0].defaultSpawnCost / Shared.GameManager.gainPerSec; // ex. 10초

        // 2) 이 시간 전후로 랜덤 범위 설정 (예: 0.5~1.5배 구간)
        enemySpawnMinInterval = chargeTime * 0.5f;   // ex. 5초
        enemySpawnMaxInterval = chargeTime * 1.0f;   // ex. 10초

        // 3) 실제 스폰 타이머에 랜덤 할당
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

        if (def.subAttackStrategy != null)
            ai.subAttackStrategy = def.subAttackStrategy;

        var hp = ch.GetComponent<HPBarController>();
        hp.Initialize(ch, hpBarPrefab, uiCanvas);

        if(!def.isEnemy)
        {
            // 2) 팝업 띄우기
            ShowGradePopup(def.rating, pos);
        }
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
        DoSpawn(def, playerSpawnPoint.position);
    }

    private void ShowGradePopup(Rating rating, Vector3 worldPos)
    {
        // 1) 팝업 인스턴스화 (부모는 UI Canvas)
        var popupGO = Instantiate(gradePopupPrefab, uiCanvas.transform, false);

        // 2) RectTransform 가져오기
        var rt = popupGO.GetComponent<RectTransform>();

        // 3) Anchor + Pivot → 좌하단 고정
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 0f);
        rt.pivot = new Vector2(0f, 0f);

        // 4) 화면 좌하단에서의 오프셋 (예: x=10, y=10)
        rt.anchoredPosition = new Vector2(50f, 10f);

        // 5) 등급 텍스트 표시
        var ctrl = popupGO.GetComponent<GradePopup>();
        ctrl.ShowRatingText(rating);
    }
}
