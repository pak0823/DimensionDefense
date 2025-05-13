using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


/// <summary>
/// ��� ĳ����(�÷��̾�, ��)�� �������� ���� �ʱ�ȭ �� ������ ó��
/// </summary>
public class Character : MonoBehaviour, IDamageable
{
    [Header("Identity")] // ���� �Ӽ����޼���
    public string characterName;


    [field: SerializeField] public int maxHp { get; private set; }
    [field: SerializeField] public int currentHp { get; private set; }
    [field: SerializeField] public int attackDamage { get; private set; }
    [field: SerializeField] public float attackRange { get; private set; }
    [field: SerializeField] public float attackCoolTime { get; private set; }
    [field: SerializeField] public float moveSpeed { get; private set; }

    // �� ������ ���� Prefab�� ������ �ʵ�
    public GameObject definitionPrefab { get; private set; }



    public event Action<int> OnHpChanged;   // HP�� �ٲ� ������ ȣ��Ǵ� �̺�Ʈ (���� HP ���� ���ڷ� �ѱ�ϴ�)
    public static event Action OnHit;   //�ǰ��� ���� �� ȣ��Ǵ� ���� �̺�Ʈ
    private Animator animator;

    // ���� Initialize�� �����ε��Ͽ� CharacterDefinition���� �޵���
    public void Initialize(CharacterDefinition def)
    {
        //�̸� ����
        characterName = def.typeName;

        // ������ Ű ����
        definitionPrefab = def.prefab;

        // ���� ���� �ʱ�ȭ ���� ȣ��
        Initialize(def.GetStats());
    }

    public virtual void Initialize(CharacterStats stats) 
    {
        animator = GetComponentInChildren<Animator>();

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
        OnHit?.Invoke();

        if (currentHp <= 0)
        {
            StartCoroutine(Die());
        }
    }

    protected virtual IEnumerator Die()
    {
        // AutoAI���� �ڽ��� ����Ʈ���� ���� �� �����ϵ��� �̺�Ʈ ȣ��
        foreach (var ai in FindObjectsOfType<AutoAI>())
            ai.ForceRemoveTarget(this.transform);

        animator.SetTrigger("Death");

        yield return new WaitForSeconds(0.2f);

        // �⺻ ��� ó�� (��Ȱ��ȭ, Ǯ ��ȯ ��)
        Shared.PoolManager.ReturnCharacter(gameObject);
        Shared.PoolManager.ReturnProjectile(gameObject);
    }
}
