using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


/// <summary>
/// 모든 캐릭터(플레이어, 적)에 공통적인 스탯 초기화 및 데미지 처리
/// </summary>
public class Character : MonoBehaviour
{
    [Header("Identity")] // 공통 속성·메서드
    public string characterName;

    public int maxHp { get; private set; }
    public int currentHp { get; private set; }
    public int attackDamage { get; private set; }
    public float attackRange { get; private set; }
    public float attackCoolTime { get; private set; }
    public float moveSpeed { get; private set; }
    



    public event Action<int> OnHpChanged;   // HP가 바뀔 때마다 호출되는 이벤트 (현재 HP 값을 인자로 넘깁니다)


    public virtual void Initialize(CharacterStats stats) 
    {
        // 1) 스탯 복사
        maxHp = stats.maxHp;
        currentHp = stats.maxHp;
        attackDamage = stats.attackDamage;
        attackRange = stats.attackRange;
        attackCoolTime = stats.attackCoolTime;
        moveSpeed = stats.moveSpeed;

        // 초기 HP 상태를 구독자에게 알림
        OnHpChanged?.Invoke(currentHp);

        // 2) 근접(BoxCollider2D) 동기화
        var box = GetComponentsInChildren<BoxCollider2D>();
        foreach (var boxs in box)
        {
            // 자신(부모)의 콜라이더는 건너뛰고, 자식만 처리
            if (boxs.gameObject == this.gameObject)
                continue;

            var size = boxs.size;
            size.x = attackRange;
            boxs.size = size;

            //var off = boxs.offset;
            //off.x = attackRange / 2f;
            //boxs.offset = off;
        }

        // 3) 원거리(CircleCollider2D) 동기화
        var allCircles = GetComponentsInChildren<CircleCollider2D>();
        foreach (var circle in allCircles)
        {
            if (circle.gameObject == this.gameObject)
                continue;

            circle.radius = attackRange;
        }
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
