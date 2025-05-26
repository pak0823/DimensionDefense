using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour, IDamageable
{
    [Header("Building Stats")]
    public BuildingStats stats;    // 에디터에서 할당
    public int currentHp { get; private set; }
    public bool isEnemy { get; private set; }
    public event System.Action<int> OnHpChanged;

    void Awake()
    {
        currentHp = stats.maxHp;
        isEnemy = stats.isEnemy;
        Shared.Building = this;
    }

    public void TakeDamage(int amount)
    {
        if (Shared.GameManager.State == GameState.GameOver) return;

        currentHp -= amount;
        OnHpChanged?.Invoke(currentHp);
        Shared.SoundManager.PlaySound("BuildingHit_SFX");

        if (currentHp <= 0)
        {
            DestroyBuilding();
        }
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
        Debug.Log("기지 파괴");
        Shared.GameManager.GameOver();
        Destroy(gameObject);

    }
}
