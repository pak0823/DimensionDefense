// Assets/Scripts/Stats/CharacterStats.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    [Header("HP")]
    public int maxHp;

    [Header("Offense")]
    public int attackDamage;
    public float attackRange;
    public float attackCoolTime;

    [Header("Movement")]
    public float moveSpeed;

    // 필요에 따라 방어력, 스킬 쿨다운, 시야 거리 등 필드 추가
}
