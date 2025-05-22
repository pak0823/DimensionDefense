using System.Xml.Linq;
using UnityEngine;

public class Building : MonoBehaviour, IDamageable
{
    [Header("Building Stats")]
    public BuildingStats stats;    // 에디터에서 할당
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
        // 파괴 이펙트, 게임 오버 로직 등
        Destroy(gameObject);
    }
}
