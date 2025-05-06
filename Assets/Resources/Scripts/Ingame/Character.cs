using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Character : MonoBehaviour
{
    // ���� �Ӽ����޼���
    public string characterName;

    // ��Ÿ�ӿ� ���硤����� ���� ����
    public int maxHp { get; private set; }
    public int currentHp { get; private set; }
    public int attackDamage { get; private set; }
    public float moveSpeed { get; private set; }
    public  float attackRange { get; private set; }

    /// <summary>
    /// HP�� �ٲ� ������ ȣ��Ǵ� �̺�Ʈ (���� HP ���� ���ڷ� �ѱ�ϴ�)
    /// </summary>
    public event Action<int> OnHpChanged;
    public virtual void Initialize(CharacterStats stats) 
    {
        maxHp = stats.maxHp;
        currentHp = stats.maxHp;
        attackDamage = stats.attackDamage;
        moveSpeed = stats.moveSpeed;
        attackRange = stats.attackRange;

        // �ʱ� HP ���¸� �����ڿ��� �˸�
        OnHpChanged?.Invoke(currentHp);
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        OnHpChanged?.Invoke(currentHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // �⺻ ��� ó�� (��Ȱ��ȭ, Ǯ ��ȯ ��)
        gameObject.SetActive(false);
    }
}
