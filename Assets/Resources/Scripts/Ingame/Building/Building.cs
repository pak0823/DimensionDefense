using System.Xml.Linq;
using UnityEngine;

public class Building : MonoBehaviour, IDamageable
{
    [Header("Building Stats")]
    public BuildingStats stats;    // �����Ϳ��� �Ҵ�
    public int currentHp { get; private set; }
    public event System.Action<int> OnHpChanged;

    void Awake()
    {
        currentHp = stats.maxHp;
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
            DestroyBuilding();

        OnHpChanged?.Invoke(currentHp);
    }

    public void TakeBuff(int amount)
    {
        currentHp += amount;
        if (currentHp >= stats.maxHp)
            currentHp = stats.maxHp;

        OnHpChanged?.Invoke(currentHp);
    }

    private void DestroyBuilding()
    {
        // �ı� ����Ʈ, ���� ���� ���� ��
        Destroy(gameObject);
    }
}
