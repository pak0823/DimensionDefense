// BasicAttack.cs
using System;
using UnityEngine;
[CreateAssetMenu(menuName = "AI/BasicAttack")]
public class MeleeAttack :AttackStrategySO
{
    [Tooltip("Animator���� ����� Ʈ���� �̸�")]
    public string attackTrigger = "Attack";

    //public static event Action OnAttack;


    public override void Attack(GameObject self, GameObject target)
    {
        // 1) �ִϸ��̼� ����
        var animator = self.GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger(attackTrigger);

        // 2) ���� ���� ��������
        var attacker = self.GetComponent<Character>();
        int damage = attacker != null ? attacker.attackDamage : 0;

        // 3) ����� ����
        var characterDamaged = target.GetComponentInParent<Character>();
        var BuildingDamaged = target.GetComponentInParent<IDamageable>();

        if (characterDamaged != null)
            characterDamaged.TakeDamage(damage);
        else if (BuildingDamaged != null)
            BuildingDamaged.TakeDamage(damage);
        else
            Debug.LogWarning($"����� ����: characterDamaged = ({characterDamaged}) \n BuildingDamaged = ({BuildingDamaged})");

        //OnAttack?.Invoke();


        //Debug.Log("���ݽ���: " + self.name + "���� ��: " + target.name + "�����: " + damage);
    }
}
