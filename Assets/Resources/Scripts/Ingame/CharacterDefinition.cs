using UnityEngine;

public enum MeleeVariant { HighAttack, HighDefense, Balanced }

[CreateAssetMenu(menuName = "Spawn/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("�⺻ ����")]
    public string typeName;         // ex. "����_������", "����_�����" ǥ�ÿ�

    [Header("������")]
    public GameObject prefab;           // ������ ������

    [Header("�뷱�� ������")]
    public MeleeVariant variant;          // � Ÿ������ ����

    [Header("������ ���̾�")]
    public LayerMask detectionMask;     //� ���̾ �����Ұ��� ����

    [Header("���� ���� ����")]
    public bool isEnemy;            //Ÿ�Կ� ���� ������ġ ����

    [Header("Attack Strategy")]
    public AttackStrategySO attackStrategy;     //Ÿ�Կ� ���� ���� ��� ����

    [Tooltip("������ ����")]
    public CharacterStats statsHighAtk;

    [Tooltip("����� ����")]
    public CharacterStats statsHighDef;

    [Tooltip("������ ����")]
    public CharacterStats statsBalanced;

    /// <summary>
    /// SpawnManager���� ȣ���� ��, variant�� �´� stats�� ��ȯ
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
