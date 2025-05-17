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
    [Tooltip("HP Bar Prefab (Canvas 하위에 추가)")]
    public GameObject hpBarPrefab;
    [Tooltip("HP Bar를 추가할 Canvas Transform")]
    public Transform uiCanvas;

    [Header("Enemy Spawn Interval Settings")]
    public float enemySpawnMinInterval = 10f;    //스폰 간격 최소(초)
    public float enemySpawnMaxInterval = 18f;   //스폰 간격 최대(초)
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
            .Where(d => d.rating != Rating.Normal)
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
        enemySpawnMaxInterval = chargeTime * 1.5f;   // ex. 15초

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

        var hp = ch.GetComponent<HPBarController>();
        hp.Initialize(ch, hpBarPrefab, uiCanvas);
    }
}
