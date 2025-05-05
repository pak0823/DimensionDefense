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
    public virtual void Initialize(CharacterStats stats) 
    {
        currentHp = stats.maxHp;
        attackDamage = stats.attackDamage;
        moveSpeed = stats.moveSpeed;
        attackRange = stats.attackRange;
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // �⺻ ������ �ʿ��ϸ� �����, 
        // �ƴϸ� �ڽĿ��� override
    }
}
