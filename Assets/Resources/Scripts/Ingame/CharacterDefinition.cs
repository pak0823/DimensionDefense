using UnityEngine;


public enum AttackType { Melee, Range}
public enum MeleeType { None ,Sword, Axe, Spear, DoubleShield, DoubleSword, DoubleAxe, SwordShield, SpearShield, AxeShield }
public enum RangeType { None, Bow, FireBolt, LightningBolt, Boom, Heal, Buffer }
public enum Rating { Normal, Rare, Unique, Legendary}

[CreateAssetMenu(menuName = "Definition/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("�⺻ ����")]
    public string typeName;      // ex. "����_�˻�", "���Ÿ�_�ü�"

    [Header("������")]
    public GameObject prefab;

    [Header("���� ���� ����")]
    public bool isEnemy;            //Ÿ�Կ� ���� ������ġ ����

    [Header("���� ����")]
    public AttackType attackType;

    [Header("���� ���� Ÿ�� (attackType = Melee �� ��)")]
    public MeleeType meleeType;

    [Header("���Ÿ� ���� Ÿ�� (attackType = Range �� ��)")]
    public RangeType rangeType;

    [Header("�뷱�� ������")]
    public Rating rating;

    [Header("Attack Strategy")]
    public AttackStrategySO attackStrategy;     //Ÿ�Կ� ���� ���� ��� ����

    [Header("������ ���̾�")]
    public LayerMask detectionMask;     //� ���̾ �����Ұ��� ����

    [Header("���� (Rating ������)")]
    [Tooltip("Rare ��޿� ������ ����")]
    public float rareMultiplier = 1.2f;
    [Tooltip("Unique ��޿� ������ ����")]
    public float uniqueMultiplier = 1.5f;
    [Tooltip("Legendary ��޿� ������ ����")]
    public float legendaryMultiplier = 2.0f;

    [Header("���� ���� (AttackType+Subtype)")]
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
    /// SpawnManager�� PoolManager���� ȣ���Ͽ�
    /// AttackType + Subtype �� ���� �ùٸ� Stats�� �����ݴϴ�.
    /// </summary>
    public CharacterStats GetStats()
    {
        // 1) �⺻ ���� ����
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
                    case RangeType.Buffer: baseStats = statsRangeBuffer; break;
                    default: baseStats = statsRangeBow; break;
                }
                break;

            default:
                baseStats = statsMeleeSword;
                break;
        }

        // 2) Rating ���� ���
        float multiplier = GetMultiplier();

        // 3) �� �ν��Ͻ� ���� �� ����
        var stats = ScriptableObject.CreateInstance<CharacterStats>();
        stats.maxHp = Mathf.RoundToInt(baseStats.maxHp * multiplier);
        stats.attackDamage = Mathf.RoundToInt(baseStats.attackDamage * multiplier);
        stats.attackCoolTime = baseStats.attackCoolTime;
        stats.moveSpeed = baseStats.moveSpeed;
        stats.attackRange = baseStats.attackRange;

        return stats;
    }

    /// <summary>
    /// Rating�� ���� ������ ��ȯ
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
