using System;
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

    /// <summary>
    /// HP가 바뀔 때마다 호출되는 이벤트 (현재 HP 값을 인자로 넘깁니다)
    /// </summary>
    public event Action<int> OnHpChanged;
    public virtual void Initialize(CharacterStats stats) 
    {
        maxHp = stats.maxHp;
        currentHp = stats.maxHp;
        attackDamage = stats.attackDamage;
        moveSpeed = stats.moveSpeed;
        attackRange = stats.attackRange;

        // 초기 HP 상태를 구독자에게 알림
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
        // 기본 사망 처리 (비활성화, 풀 반환 등)
        gameObject.SetActive(false);
    }
}
