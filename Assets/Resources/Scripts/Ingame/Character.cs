using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


/// <summary>
/// ��� ĳ����(�÷��̾�, ��)�� �������� ���� �ʱ�ȭ �� ������ ó��
/// </summary>
public class Character : MonoBehaviour
{
    [Header("Identity")] // ���� �Ӽ����޼���
    public string characterName;

    public int maxHp { get; private set; }
    public int currentHp { get; private set; }
    public int attackDamage { get; private set; }
    public float attackRange { get; private set; }
    public float attackCoolTime { get; private set; }
    public float moveSpeed { get; private set; }
    



    public event Action<int> OnHpChanged;   // HP�� �ٲ� ������ ȣ��Ǵ� �̺�Ʈ (���� HP ���� ���ڷ� �ѱ�ϴ�)


    public virtual void Initialize(CharacterStats stats) 
    {
        // 1) ���� ����
        maxHp = stats.maxHp;
        currentHp = stats.maxHp;
        attackDamage = stats.attackDamage;
        attackRange = stats.attackRange;
        attackCoolTime = stats.attackCoolTime;
        moveSpeed = stats.moveSpeed;

        // �ʱ� HP ���¸� �����ڿ��� �˸�
        OnHpChanged?.Invoke(currentHp);

        // 2) ����(BoxCollider2D) ����ȭ
        var box = GetComponentsInChildren<BoxCollider2D>();
        foreach (var boxs in box)
        {
            // �ڽ�(�θ�)�� �ݶ��̴��� �ǳʶٰ�, �ڽĸ� ó��
            if (boxs.gameObject == this.gameObject)
                continue;

            var size = boxs.size;
            size.x = attackRange;
            boxs.size = size;

            //var off = boxs.offset;
            //off.x = attackRange / 2f;
            //boxs.offset = off;
        }

        // 3) ���Ÿ�(CircleCollider2D) ����ȭ
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
        // �⺻ ��� ó�� (��Ȱ��ȭ, Ǯ ��ȯ ��)
        gameObject.SetActive(false);
    }
}
