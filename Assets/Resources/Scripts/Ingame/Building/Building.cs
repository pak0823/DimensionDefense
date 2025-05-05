using System.Xml.Linq;
using UnityEngine;

public class Building : MonoBehaviour, IDamageable
{
    [Header("Building Stats")]
    public BuildingStats stats;    // �����Ϳ��� �Ҵ�
    private int currentHp;

    void Awake()
    {
        currentHp = stats.maxHp;
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        Debug.Log($"{name} took {amount} damage, HP = {currentHp}/{stats.maxHp}");
        if (currentHp <= 0)
            DestroyBuilding();
    }

    private void DestroyBuilding()
    {
        // �ı� ����Ʈ, ���� ���� ���� ��
        Destroy(gameObject);
    }
}
