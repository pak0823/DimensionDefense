using UnityEngine;

public enum MeleeVariant { HighAttack, HighDefense, Balanced }

[CreateAssetMenu(menuName = "Spawn/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("기본 정보")]
    public string typeName;         // ex. "근접_공격형", "근접_방어형" 표시용

    [Header("프리팹")]
    public GameObject prefab;           // 스폰할 프리팹

    [Header("밸런스 프로필")]
    public MeleeVariant variant;          // 어떤 타입인지 선택

    [Header("감지할 레이어")]
    public LayerMask detectionMask;     //어떤 레이어를 감지할건지 선택

    [Header("스폰 지점 선택")]
    public bool isEnemy;            //타입에 따른 스폰위치 지정

    [Header("Attack Strategy")]
    public AttackStrategySO attackStrategy;     //타입에 따른 공격 방식 지정

    [Tooltip("공격형 스탯")]
    public CharacterStats statsHighAtk;

    [Tooltip("방어형 스탯")]
    public CharacterStats statsHighDef;

    [Tooltip("균형형 스탯")]
    public CharacterStats statsBalanced;

    /// <summary>
    /// SpawnManager에서 호출할 때, variant에 맞는 stats를 반환
    /// </summary>
    public CharacterStats GetStats()
    {
        switch (variant)
        {
            case MeleeVariant.HighAttack: return statsHighAtk;
            case MeleeVariant.HighDefense: return statsHighDef;
            default: return statsBalanced;
        }
    }
}
