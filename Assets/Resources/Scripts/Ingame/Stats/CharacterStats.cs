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

    // �ʿ信 ���� ����, ��ų ��ٿ�, �þ� �Ÿ� �� �ʵ� �߰�
}
