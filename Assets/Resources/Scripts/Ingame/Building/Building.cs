using System.Xml.Linq;
using UnityEngine;

public class Building : MonoBehaviour, IDamageable
{
    [Header("Building Stats")]
    public BuildingStats stats;    // 에디터에서 할당
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
        // 파괴 이펙트, 게임 오버 로직 등
        Destroy(gameObject);
    }
}
