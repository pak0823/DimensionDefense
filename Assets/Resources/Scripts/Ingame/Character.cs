using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Character : MonoBehaviour
{
    // 공통 속성·메서드
    public string characterName;

    // 런타임에 복사·저장될 실제 스탯
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
        // 기본 구현이 필요하면 남기고, 
        // 아니면 자식에서 override
    }
}
