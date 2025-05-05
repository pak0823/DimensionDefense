// BasicAttack.cs
using UnityEngine;
[CreateAssetMenu(menuName = "AI/BasicAttack")]
public class BasicAttack :AttackStrategySO
{
    [Tooltip("Animator���� ����� Ʈ���� �̸�")]
    public string attackTrigger = "Attack";


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
        var victim = target.GetComponentInParent<Character>();
        if (victim != null)
            victim.TakeDamage(damage);
        else
            Debug.LogWarning($"BasicAttack: ��� Character ���� ({target.name})");

        var dmgable = target.GetComponentInParent<IDamageable>();
        if (dmgable != null)
            dmgable.TakeDamage(damage);

        Debug.Log("���ݽ���: " + self.name + "���� ��: " + target.name + "�����: " + damage);
    }
}
