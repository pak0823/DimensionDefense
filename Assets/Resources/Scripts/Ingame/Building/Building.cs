using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour, IDamageable
{
    [Header("Building Stats")]
    public BuildingStats stats;    // �����Ϳ��� �Ҵ�
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
        currentHp -= amount;
        if (currentHp <= 0)
            DestroyBuilding();
            //StartCoroutine(DestroyBuilding());

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
        Shared.GameManager.GameOver();
        // �ı� ����Ʈ, ���� ���� ���� ��
        //yield return new WaitForSeconds(1f);
        Destroy(gameObject);

    }
}
