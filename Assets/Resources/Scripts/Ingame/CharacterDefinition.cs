using UnityEngine;




[CreateAssetMenu(menuName = "Definition/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("기본 정보")]
    public string typeName;      //캐릭터의 이름

    [Header("프리팹")]
    public GameObject prefab;   //소환될 캐릭터의 프리팹

    [Header("스폰 지점 선택")]
    public bool isEnemy;            //타입에 따른 스폰위치 지정

    [Header("공격 유형")]
    public AttackType attackType;   //근거리, 원거리 구분

    [Header("근접 세부 타입 (attackType = Melee 일 때)")]
    public MeleeType meleeType;

    [Header("원거리 세부 타입 (attackType = Range 일 때)")]
    public RangeType rangeType;

    [Header("밸런스 프로필")]
    public Rating rating;   //이 캐릭터의 등급
    public float spawnWeight;    //이 캐릭터가 소환될 확률 가중치

    [Header("Cost Settings")]
    public int defaultSpawnCost = 10;   //기본 소환 비용
    public int specialSpawnCost = 30;   //특별 소환 비용


    [Header("Attack Strategy")]
    public AttackStrategySO attackStrategy;     //타입에 따른 공격 방식 지정
    public AttackStrategySO subAttackStrategy;   //레전드리 등급 캐릭터 추가 공격

    [Header("감지할 레이어")]
    public LayerMask detectionMask;     //어떤 레이어를 감지할건지 선택

    [Header("배율 (Rating 보정용)")]
    [Tooltip("Rare 등급에 곱해질 배율")]
    public float rareMultiplier = 1.5f;
    [Tooltip("Unique 등급에 곱해질 배율")]
    public float uniqueMultiplier = 2.0f;
    [Tooltip("Legendary 등급에 곱해질 배율")]
    public float legendaryMultiplier = 3f;

    [Header("세부 스탯 (AttackType+Subtype)")]
    public CharacterStats statsMeleeSword;
    public CharacterStats statsMeleeDoubleShield;
    public CharacterStats statsMeleeDoubleSword;
    public CharacterStats statsMeleeAxe;
    public CharacterStats statsMeleeDoubleAxe;
    public CharacterStats statsMeleeSwordShield;
    public CharacterStats statsMeleeSpearShield;
    public CharacterStats statsMeleeAxeShield;
    public CharacterStats statsMeleeSpear;

    public CharacterStats statsRangeBow;
    public CharacterStats statsRangeFireBolt;
    public CharacterStats statsRangeLightningBolt;
    public CharacterStats statsRangeBoom;
    public CharacterStats statsRangeHeal;
    public CharacterStats statsRangeBuffer;



    /// <summary>
    /// SpawnManager나 PoolManager에서 호출하여
    /// AttackType + Subtype 에 따라 올바른 Stats를 돌려줍니다.
    /// </summary>
    public CharacterStats GetStats()
    {
        // 1) 기본 스탯 선택
        CharacterStats baseStats;
        switch (attackType)
        {
            case AttackType.Melee:
                switch (meleeType)
                {
                    case MeleeType.None: baseStats = statsMeleeSword; break;
                    case MeleeType.Sword: baseStats = statsMeleeSword; break;
                    case MeleeType.Axe: baseStats = statsMeleeAxe; break;
                    case MeleeType.Spear: baseStats = statsMeleeSpear; break;

                    case MeleeType.DoubleShield: baseStats = statsMeleeDoubleShield; break;
                    case MeleeType.DoubleSword: baseStats = statsMeleeDoubleSword; break;
                    case MeleeType.DoubleAxe: baseStats = statsMeleeDoubleAxe; break;

                    case MeleeType.SwordShield: baseStats = statsMeleeSwordShield; break;
                    case MeleeType.SpearShield: baseStats = statsMeleeSpearShield; break;
                    case MeleeType.AxeShield: baseStats = statsMeleeAxeShield; break;
                    default: baseStats = statsMeleeSword; break;
                }
                break;

            case AttackType.Range:
                switch (rangeType)
                {
                    case RangeType.None: baseStats = statsRangeBow; break;
                    case RangeType.Bow: baseStats = statsRangeBow; break;
                    case RangeType.FireBolt: baseStats = statsRangeFireBolt; break;
                    case RangeType.LightningBolt: baseStats = statsRangeLightningBolt; break;
                    case RangeType.Boom: baseStats = statsRangeBoom; break;
                    case RangeType.Heal: baseStats = statsRangeHeal; break;
                    case RangeType.WindBreath: baseStats = statsRangeBuffer; break;
                    default: baseStats = statsRangeBow; break;
                }
                break;

            default:
                baseStats = statsMeleeSword;
                break;
        }

        // 2) Rating 배율 계산
        float multiplier = GetMultiplier();

        // 3) 새 인스턴스 생성 및 보정
        var stats = ScriptableObject.CreateInstance<CharacterStats>();
        stats.maxHp = Mathf.RoundToInt(baseStats.maxHp * multiplier);
        stats.attackDamage = Mathf.RoundToInt(baseStats.attackDamage * multiplier);
        stats.attackCoolTime = baseStats.attackCoolTime;
        stats.moveSpeed = baseStats.moveSpeed;
        stats.attackRange = baseStats.attackRange;

        return stats;
    }

    /// <summary>
    /// Rating에 따른 배율을 반환
    /// </summary>
    private float GetMultiplier()
    {
        switch (rating)
        {
            case Rating.Rare: return rareMultiplier;
            case Rating.Unique: return uniqueMultiplier;
            case Rating.Legendary: return legendaryMultiplier;
            default: return 1f;  // Normal
        }
    }
}
